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
    const float maxRepositionDistance = 10f;
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
    enum AxisType { TowardsPlayer = 3, TowardsPosition = 3, SensorResponse = 1, Evasion = 2, Stop = 4 }
    AxisType axisXType = AxisType.TowardsPlayer;
    AxisType axisYType = AxisType.TowardsPlayer;
    enum Axis { X, Y }
    float repositionTime = 0;
    PlayerTank[] players;
    Transform closestPlayer;
    PlayerController playerController;
    Vector3 playerTarget, repositionPoint;
    float randomDirection = 1;
    float axisX = 0;
    float axisY = 0;
    bool axisXOverriden = false;
    bool axisYOverriden = false;
    bool engage = false;
    Coroutine lastCoroutine;
    bool priorityIsPlayer = true;
    bool repositionOn = false;

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
        //print("X: " + axisXType + " : " + axisX);
        //print("Y: " + axisYType + " : " + axisY);
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
                aiAiming.AimTurret(playerTarget);
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
                SetAxis(-0.5f, AxisType.SensorResponse, Axis.X);
                axisYOverriden = false;
                break;
            case AIMovement.Sensor.FrontLeft:
                SetAxis(0.5f, AxisType.SensorResponse, Axis.X);
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
        SelectMovementManagement();
    }

    private void SelectMovementManagement()
    {
        if ((playerTarget - transform.position).magnitude > maxDistance || repositionOn == false)
        {
            ManageMovement();
        }
        else
        {
            ManageMovementTowardsReposition();
        }

    }

    private void ManageMovement()
    {
        if ((playerTarget - transform.position).magnitude > maxDistance)
        {
            SetAxis(1, AxisType.TowardsPlayer, Axis.Y);

            // Calculate direction of target for rotation
            float frontAngle = Vector3.Angle(transform.forward, playerTarget - transform.position);
            float rightAngle = Vector3.Angle(transform.right, playerTarget - transform.position);

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
        else if (flightStatus == TrajectoryStatus.Fail)
        {
            bool xCheck = SetAxis(0, AxisType.TowardsPlayer, Axis.X);
            bool yCheck = SetAxis(1, AxisType.TowardsPlayer, Axis.Y);
            if (xCheck && yCheck)
            {
                if (lastCoroutine != null)
                {
                    StopCoroutine(lastCoroutine);
                }
                lastCoroutine = StartCoroutine(StopAxesAfterDuration(AxisType.TowardsPlayer, 1.5f));
                flightStatus = TrajectoryStatus.Standby;
            }
        }
        else if (flightStatus == TrajectoryStatus.Success)
        {
            StopAxis(AxisType.TowardsPlayer, Axis.X);
            StopAxis(AxisType.TowardsPlayer, Axis.Y);
        }
    }

    private void ManageMovementTowardsReposition()
    {
        if ((repositionPoint - transform.position).magnitude > maxRepositionDistance)
        {
            SetAxis(1, AxisType.TowardsPosition, Axis.Y);

            // Calculate direction of target for rotation
            float frontAngle = Vector3.Angle(transform.forward, repositionPoint - transform.position);
            float rightAngle = Vector3.Angle(transform.right, repositionPoint - transform.position);

            if (frontAngle > 6f && rightAngle > 90f)
            {
                SetAxis(-1, AxisType.TowardsPosition, Axis.X);
            }
            else if (frontAngle > 6f && rightAngle < 90f)
            {
                SetAxis(1, AxisType.TowardsPosition, Axis.X);
            }
            else
            {
                SetAxis(0, AxisType.TowardsPosition, Axis.X);
            }
        }
        else
        {
            StopAxis(AxisType.TowardsPosition, Axis.X);
            StopAxis(AxisType.TowardsPosition, Axis.Y);
            repositionOn = false;
        }
    }

    private void ManageFireInput()
    {
        bool aimStatus = false;

        if (axisYType == AxisType.Stop || axisYType == AxisType.TowardsPosition)
        {
            float aimAngle = aiController.CalculateAimAngle(playerTarget, barrelWheel, transform);
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

    IEnumerator StopAxesAfterDuration(AxisType axisType, float duration)
    {
        yield return new WaitForSeconds(duration);

        StopAxis(axisType, Axis.X);
        StopAxis(axisType, Axis.Y);

        if (axisType == AxisType.TowardsPlayer)
        {
            flightStatus = TrajectoryStatus.Success;
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
                GetTarget();
                repositionOn = true;
                repositionPoint = GetRepositionPoint();
            }
            yield return new WaitForSeconds(10f);
        }
    }

    private bool GetTarget()
    {
        if (closestPlayer != null)
        {
            playerTarget = closestPlayer.position;
            playerController = closestPlayer.GetComponent<PlayerController>();
            if ((playerTarget - transform.position).magnitude < 10f && axisYType == AxisType.Stop)
            {
                repositionOn = true;
                repositionPoint = GetRepositionPoint();
            }
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
            if (angle <= 9f)
            {
                if (axisXType != AxisType.Evasion)
                {
                    Evade();
                }
            }
        }
    }

    public void Evade()
    {
        float num = Mathf.RoundToInt(UnityEngine.Random.Range(0, 2));
        if (num == 0) { num = -1; }
        bool xCheck = SetAxis(num, AxisType.Evasion, Axis.X);
        bool yCheck = SetAxis(1, AxisType.Evasion, Axis.Y);
        if (xCheck && yCheck)
        {
            if (lastCoroutine != null)
            {
                StopCoroutine(lastCoroutine);
            }
            lastCoroutine = StartCoroutine(StopAxesAfterDuration(AxisType.Evasion, 0.75f));
        }
    }

    public Vector3 GetRepositionPoint()
    {
        int layerMask = 1 << 10;
        int inverseLayer = ~(layerMask);

        float xVal = UnityEngine.Random.Range(-40f, 40f);
        float zVal = UnityEngine.Random.Range(-40f, 40f);
        Vector3 vectorAdd = new Vector3(xVal, playerTarget.y + 1f, zVal);
        Vector3 point = playerTarget + vectorAdd;
        Vector3 repositionPoint = point;

        Ray ray = new Ray(playerTarget, point - playerTarget);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, (point - playerTarget).magnitude, ~(layerMask)))
        {
            repositionPoint = hit.point;
            print(hit.collider.name + " hit");
        }

        return repositionPoint;
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
