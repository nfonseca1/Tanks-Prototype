using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIGunTank : AIEnemy
{
    const int maxShots = 8;
    const float coolDown = 5f;
    const float fireRate = .2f;
    const float sensorLength = 5f;
    const float maxDistance = 35f;
    const float minDistance = 25f;
    const float neutralDistance = 30f;
    const float maxShootDistance = 40f;
    const float lerpLimitX = 0.25f;
    const float lerpLimitY = 0.5f;

    float lerpX = 0;
    float lerpY = 0;
    float flightTime;
    new Rigidbody rigidbody;

    [SerializeField] float speed = 25f;
    [SerializeField] float torque = 180f;
    [SerializeField] float launchVelocity = 25f;
    [SerializeField] Transform[] sensors;
    [SerializeField] Transform turret;
    [SerializeField] Transform barrelWheel;
    [SerializeField] Transform[] barrels;
    [SerializeField] Transform[] emitters;
    [SerializeField] Bullet bullet;
    [SerializeField] ParticleSystem frontParticles;
    [SerializeField] ParticleSystem backParticles;
    ParticleSystem.EmissionModule frontEmission;
    ParticleSystem.EmissionModule backEmission;

    AIController aiController;
    AIMovement aiMovement;
    AIAiming aiAiming;

    enum TrajectoryStatus { Success, Fail, Standby }
    TrajectoryStatus flightStatus = TrajectoryStatus.Standby;
    float repositionTime = 0;
    PlayerTank[] players;
    Transform closestPlayer;
    Vector3 target;
    float randomDirection = 1;
    float axisX = 0;
    float axisY = 0;
    bool axisXOverriden = false;
    bool axisYOverriden = false;
    bool engage = false;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        aiController = new AIController();
        aiMovement = new AIMovement(ref rigidbody, speed, torque, sensors);
        aiAiming = new AIAiming(turret, barrelWheel, barrels, emitters, maxShots, coolDown, fireRate);

        frontEmission = frontParticles.emission;
        backEmission = backParticles.emission;
        frontEmission.enabled = false;
        backEmission.enabled = false;

        StartCoroutine(GetClosestPlayerInterval());
    }

    // Update is called once per frame
    void Update()
    {
        if (engage && grounded)
        {
            players = aiController.GetPlayers();
            if (players.Length > 0)
            {
                GetTarget();
            }

            if (closestPlayer != null)
            {
                ManageSensors();
                aiMovement.ManageAxisInput(axisX, ref lerpX, lerpLimitX);
                aiMovement.ManageAxisInput(axisY, ref lerpY, lerpLimitY);

                float inputY = lerpY / lerpLimitY;
                aiMovement.Move(inputY, transform); // lerp / lerpLimit dictates the percentage of movement
                ManageMovementParticles(inputY);
                aiMovement.Rotate(lerpX / lerpLimitX, transform);
                aiAiming.AimTurret(target);
                if (players.Length > 0 && aiAiming.CheckIfReadyToFire())
                {
                    ManageFireInput();
                }
            }
            else
            {
                aiMovement.Move(0, transform);
                ManageMovementParticles(0);
            }
        }
    }

    void ManageMovementParticles(float input)
    {
        if (input > 0)
        {
            frontEmission.enabled = true;
            backEmission.enabled = false;
        }
        else if (input < 0)
        {
            frontEmission.enabled = false;
            backEmission.enabled = true;
        }
        else
        {
            frontEmission.enabled = false;
            backEmission.enabled = false;
        }
    }

    void ManageSensors()
    {
        AIMovement.Sensor sensor = aiMovement.CheckSensors(sensorLength);
        switch (sensor)
        {
            case AIMovement.Sensor.Front:
                axisX = randomDirection;
                axisY = 0;
                axisXOverriden = true;
                axisYOverriden = true;
                break;
            case AIMovement.Sensor.FrontRight:
                axisX = -1;
                axisXOverriden = true;
                axisYOverriden = false;
                break;
            case AIMovement.Sensor.FrontLeft:
                axisX = 1;
                axisXOverriden = true;
                axisYOverriden = false;
                break;
            case AIMovement.Sensor.Left:
                if (axisX < 0)
                {
                    axisX = 0;
                    axisXOverriden = true;
                    axisYOverriden = false;
                }
                else
                {
                    axisXOverriden = false;
                    axisYOverriden = false;
                }
                break;
            case AIMovement.Sensor.Right:
                if (axisX > 0)
                {
                    axisX = 0;
                    axisXOverriden = true;
                    axisYOverriden = false;
                }
                else
                {
                    axisXOverriden = false;
                    axisYOverriden = false;
                }
                break;
            default:
                float num = Mathf.RoundToInt(UnityEngine.Random.Range(0, 2));
                if (num == 0) { num = -1; }
                randomDirection = num;

                axisXOverriden = false;
                axisYOverriden = false;
                break;
        }
        ManageMovement();
    }

    private void ManageMovement()
    {
        if ((target - transform.position).magnitude > maxDistance)
        {
            if (!axisYOverriden) { axisY = 1; }

            if (!axisXOverriden)
            {
                // Calculate direction of target for rotation
                float frontAngle = Vector3.Angle(transform.forward, target - transform.position);
                float rightAngle = Vector3.Angle(transform.right, target - transform.position);

                if (frontAngle > 10f && rightAngle > 90f)
                {
                    axisX = -1f;
                }
                else if (frontAngle > 10f && rightAngle < 90f)
                {
                    axisX = 1f;
                }
                else
                {
                    axisX = 0;
                }
            }
        }
        else
        {
            if (flightStatus == TrajectoryStatus.Fail)
            {
                DelayAutoMovement();
                if (!axisXOverriden) { axisX = 0; }
                if (!axisYOverriden) { axisY = 1; }
            }
            else
            {
                if (!axisXOverriden) { axisX = 0; }
                if (!axisYOverriden) { axisY = 0; }
            }
        }
    }

    private void ManageFireInput()
    {
        bool aimStatus = false;

        if (lerpY == 0)
        {
            float aimAngle = aiController.CalculateAimAngle(target, barrelWheel, transform);
            aimStatus = aiAiming.AimBarrel(new Vector3(aimAngle, barrelWheel.localEulerAngles.y, barrelWheel.localEulerAngles.z));

            if (aimStatus == true)
            {
                bool clear = aiAiming.CheckBarrelClearance(sensorLength);
                if (clear)
                {
                    flightTime = aiAiming.GetTrajectoryTime(launchVelocity);
                    aiAiming.Fire(launchVelocity, bullet);
                }
                else
                {
                    flightStatus = TrajectoryStatus.Fail;
                }
            }
        }
    }

    private void DelayAutoMovement()
    {
        repositionTime += Time.deltaTime;
        if (repositionTime >= 1.5f)
        {
            repositionTime = 0;
            flightStatus = TrajectoryStatus.Standby;
        }
    }

    IEnumerator GetClosestPlayerInterval()
    {
        while (true)
        {
            players = aiController.GetPlayers();
            if (players.Length > 0)
            {
                closestPlayer = aiController.GetClosestPlayer(players, transform);
            }
            yield return new WaitForSeconds(5f);
        }
    }

    private bool GetTarget()
    {
        if (closestPlayer != null)
        {
            target = closestPlayer.position;
        }
        else
        {
            closestPlayer = aiController.GetClosestPlayer(players, transform);
        }

        return true;
    }

    public void SetProjectileTime(float time)
    {
        if (time / flightTime >= 0.95f)
        {
            flightStatus = TrajectoryStatus.Success;
        }
        else
        {
            flightStatus = TrajectoryStatus.Fail;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        CPZone zone = other.GetComponent<CPZone>();
        if (zone != null)
        {
            if (zone.readyToAttack == true)
            {
                engage = true;
            }
        }
    }
}
