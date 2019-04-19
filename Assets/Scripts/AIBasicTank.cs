using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBasicTank : AIEnemy
{
    const int maxShots = 4;
    const float coolDown = 4f;
    const float fireRate = 3f;
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
    [SerializeField] Shell shell;
    [SerializeField] ParticleSystem frontParticles;
    [SerializeField] ParticleSystem backParticles;
    ParticleSystem.EmissionModule frontEmission;
    ParticleSystem.EmissionModule backEmission;

    AIController aiController;
    AIMovement aiMovement;
    AIAiming aiAiming;

    enum TrajectoryStatus { Success, Fail, Standby }
    TrajectoryStatus flightStatus = TrajectoryStatus.Standby;
    enum AxisType { TowardsPlayer = 3, TowardsPosition = 3, SensorResponse = 1, Evasion = 2, Stop = 4}
    AxisType axisXType = AxisType.TowardsPlayer;
    AxisType axisYType = AxisType.TowardsPlayer;
    enum Axis { X, Y}
    float repositionTime = 0;
    PlayerTank[] players;
    Transform closestPlayer;
    PlayerController playerController;
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
                CheckToEvade();
            }
            else
            {
                aiMovement.Move(0, transform);
                ManageMovementParticles(0);
            }
        }
    }

    // Check to see if the axisType can be overriden. If so, set it to the appropriate value and return success status
    bool SetAxis(float newAxisVal, AxisType axisType, Axis axisToModify)
    {
        if (axisToModify == Axis.X)
        {
            if ((int)axisType <= (int)axisXType)
            {
                axisXType = axisType;
                axisX = newAxisVal;
                return true;
            }
        }
        else if (axisToModify == Axis.Y)
        {
            if ((int)axisType <= (int)axisXType)
            {
                axisYType = axisType;
                axisY = newAxisVal;
                return true;
            }
        }

        return false;
    }

    // If the axisToCheck is the current axis type, set that axis type to stop and its value to 0
    // Otherwise ignore it. We don't want to stop an axis that is in the middle of another process
    bool StopAxis(AxisType axisToCheck, Axis axisToModify)
    {
        if (axisToModify == Axis.X)
        {
            if (axisToCheck == axisXType)
            {
                axisXType = AxisType.Stop;
                axisX = 0;
                return true;
            }
        }
        else if (axisToModify == Axis.Y)
        {
            if (axisToCheck == axisYType)
            {
                axisYType = AxisType.Stop;
                axisY = 0;
                return true;
            }
        }

        return false;
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
                SetAxis(randomDirection, AxisType.SensorResponse, Axis.X);
                SetAxis(0, AxisType.SensorResponse, Axis.Y);
                break;
            case AIMovement.Sensor.FrontRight:
                SetAxis(-1, AxisType.SensorResponse, Axis.X);
                axisYOverriden = false;
                break;
            case AIMovement.Sensor.FrontLeft:
                SetAxis(1, AxisType.SensorResponse, Axis.X);
                axisYOverriden = false;
                break;
            case AIMovement.Sensor.Left:
                if (axisX < 0)
                {
                    SetAxis(0, AxisType.SensorResponse, Axis.X);
                    StopAxis(AxisType.SensorResponse, Axis.Y);
                }
                else
                {
                    StopAxis(AxisType.SensorResponse, Axis.X);
                    StopAxis(AxisType.SensorResponse, Axis.Y);
                }
                break;
            case AIMovement.Sensor.Right:
                if (axisX > 0)
                {
                    SetAxis(0, AxisType.SensorResponse, Axis.X);
                    StopAxis(AxisType.SensorResponse, Axis.Y);
                }
                else
                {
                    StopAxis(AxisType.SensorResponse, Axis.X);
                    StopAxis(AxisType.SensorResponse, Axis.Y);
                }
                break;
            default:
                float num = Mathf.RoundToInt(UnityEngine.Random.Range(0, 2));
                if (num == 0) { num = -1; }
                randomDirection = num;

                StopAxis(AxisType.SensorResponse, Axis.X);
                StopAxis(AxisType.SensorResponse, Axis.Y);
                break;
        }
        ManageMovement();
    }

    private void ManageMovement()
    {
        if ((target - transform.position).magnitude > maxDistance)
        {
            SetAxis(1, AxisType.TowardsPlayer, Axis.Y);
            
            // Calculate direction of target for rotation
            float frontAngle = Vector3.Angle(transform.forward, target - transform.position);
            float rightAngle = Vector3.Angle(transform.right, target - transform.position);

            if (frontAngle > 10f && rightAngle > 90f)
            {
                SetAxis(-1, AxisType.TowardsPlayer, Axis.X);
            }
            else if (frontAngle > 10f && rightAngle < 90f)
            {
                SetAxis(1, AxisType.TowardsPlayer, Axis.X);
            }
            else
            {
                SetAxis(0, AxisType.TowardsPlayer, Axis.X);
            }
        }
        else
        {
            if (flightStatus == TrajectoryStatus.Fail)
            {
                DelayAutoMovement();
                SetAxis(0, AxisType.TowardsPlayer, Axis.X);
                SetAxis(1, AxisType.TowardsPlayer, Axis.Y);
            }
            else
            {
                StopAxis(AxisType.TowardsPlayer, Axis.X);
                StopAxis(AxisType.TowardsPlayer, Axis.Y);
            }
        }
    }

    private void ManageFireInput()
    {
        bool aimStatus = false;

        if (lerpY == 0)
        {
            float aimAngle = aiController.CalculateAimAngle(target, launchVelocity, true, transform);
            aimStatus = aiAiming.AimBarrel(new Vector3(aimAngle, barrelWheel.localEulerAngles.y, barrelWheel.localEulerAngles.z));
            
            if (aimStatus == true)
            {
                bool clear = aiAiming.CheckBarrelClearance(sensorLength);
                if (clear)
                {
                    flightTime = aiAiming.GetTrajectoryTime(launchVelocity);
                    Shell projectile = aiAiming.Fire(launchVelocity, shell);
                    projectile.SetAITankSource(this);
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
            playerController = closestPlayer.GetComponent<PlayerController>();
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

    public void CheckToEvade()
    {
        if (playerController.CheckIfFiring())
        {
            float angle = playerController.GetTurretAngle(transform.position);
            if (angle <= 5f)
            {
                print("evade");
            }
        }
    }

    public void Evade()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        if (!engage)
        {
            CPZone zone = other.GetComponent<CPZone>();
            SequentialZone sZone = other.GetComponent<SequentialZone>();
            if (zone != null && zone.readyToAttack == true)
            {
                engage = true;
            }
            else if (sZone != null && sZone.readyToAttack == true)
            {
                engage = true;
            }
        }
    }
}
