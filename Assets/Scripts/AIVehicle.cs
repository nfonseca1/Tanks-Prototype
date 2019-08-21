using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIVehicle : AI
{
    protected const float getPlayerInterval = 5f;
    protected const float sensorLength = 5f;
    protected bool getPlayerNow = false;
    protected int randomDirection = 1;
    protected Transform playerTarget;
    protected Vector3 target;

    [SerializeField] protected Transform sensorPointF, sensorPointFL, sensorPointFR, sensorPointL, sensorPointR;
    protected Vehicle vehicle;

    protected float axisXMin = -1f, axisXMax = 1f, axisYMin = -1f, axisYMax = 1f;
    protected float axisX = 0, axisY = 0;
    protected float evadeDistance = 10f;
    // Rotate angle variables
    protected float angleToRotateAt = 90f;
    protected Vector3 lastPlayerFirePos;
    protected Vector3 playerFireTarget;
    protected AIEvadeMechanism evadeMechanism;

    protected const float maxDistanceFromPlayer = 40f;
    protected const float maxDistanceFromRepositionPoint = 6f;
    protected const float minDistanceFromPlayer = 15f;
    protected const float minDistanceFromRepositionPoint = -1f; // There is no min for reposition point. -1 prevents new reposition.
    protected float maxDistanceFromTarget = 15f;
    protected float minDistanceFromTarget = 10f;

    protected enum Sensor { Front, FrontRight, FrontLeft, Left, Right, None }
    protected enum AxisX { Left, X, Right}
    protected enum AxisY { Up, Y, Down}
    protected enum AxisMessage { InvalidPriority = 0, AxisOutOfRange = 1, Success = 2}
    protected enum Priority { None = 0, TowardsTarget = 1, Evasion = 2, SensorResponse = 3 }
    protected Priority axisXPriority = Priority.None, axisYPriority = Priority.TowardsTarget;
    public enum RepositionStatus { DontReposition, ReadyToReposition, Repositioning }
    protected RepositionStatus repositionStatus = RepositionStatus.ReadyToReposition;
    protected RepositionTimer repositionTimer;
    protected enum Target { Player, RepositionPoint }
    protected Target targetType = Target.Player;
    

    protected IEnumerator SetGetPlayerTag(float interval)
    {
        while (true)
        {
            getPlayerNow = true;
            yield return new WaitForSeconds(interval);
        }
    }


    protected void ManageLastEvasion()
    {
        float angle = Vector3.Angle(lastPlayerFirePos - transform.position, transform.forward);
        if (evadeMechanism.CheckToClearXAxis(angle, transform.position))
        {
            ClearXAxis(Priority.Evasion); //TODO allow for all priorities
        }
        if (evadeMechanism.CheckToClearYAxis(Time.deltaTime, transform.position))
        {
            ClearYAxis(Priority.Evasion);
        }
    }

    protected void RespondToSensors()
    {
        bool[] sensors = CheckSensors(sensorLength);

        if (sensors != null) // If there is at least one sensor detected, check which ones and respond
        {
            bool front = sensors[0], frontLeft = sensors[1], frontRight = sensors[2], left = sensors[3], right = sensors[4];

            if (left && right)
            {
                RestrictAxis(AxisX.X, Priority.SensorResponse);
            }
            else if (left)
            {
                RestrictAxis(AxisX.Left, Priority.SensorResponse);
            }
            else if (right)
            {
                RestrictAxis(AxisX.Right, Priority.SensorResponse);
            }

            if (front || (frontLeft && frontRight))
            {
                SetXAxis(randomDirection, Priority.SensorResponse);
            }
            else if (frontLeft)
            {
                SetXAxis(1f, Priority.SensorResponse);
            }
            else if (frontRight)
            {
                SetXAxis(-1f, Priority.SensorResponse);
            }
        }
        else
        {
            int num = Mathf.RoundToInt(UnityEngine.Random.Range(0, 2));
            if (num == 0) { num = -1; }
            randomDirection = num;

            ClearXAxis(Priority.SensorResponse);
        }
    }

    protected bool[] CheckSensors(float sensorLength)
    {
        Ray frontRay = new Ray(sensorPointF.position, sensorPointF.forward);
        Ray frontRightRay = new Ray(sensorPointFR.position, sensorPointFR.forward);
        Ray frontLeftRay = new Ray(sensorPointFL.position, sensorPointFL.forward);
        Ray leftRay = new Ray(sensorPointL.position, sensorPointL.forward);
        Ray rightRay = new Ray(sensorPointR.position, sensorPointR.forward);

        bool[] sensors = { false, false, false, false, false };
        bool detected = false;

        RaycastHit hitInfo;
        if (Physics.Raycast(frontRay, out hitInfo, sensorLength))
        {
            sensors[0] = true;
            detected = true;
        }
        if (Physics.Raycast(frontLeftRay, out hitInfo, sensorLength))
        {
            sensors[1] = true;
            detected = true;
        }
        if (Physics.Raycast(frontRightRay, out hitInfo, sensorLength))
        {
            sensors[2] = true;
            detected = true;
        }
        if (Physics.Raycast(leftRay, out hitInfo, sensorLength))
        {
            sensors[3] = true;
            detected = true;
        }
        if (Physics.Raycast(rightRay, out hitInfo, sensorLength))
        {
            sensors[4] = true;
            detected = true;
        }
        // If at least one sensor is detected, return the array. Otherwise return null.
        if (detected) { return sensors; }
        return null;
    }

    // Check to see if the axisType can be overriden. If so, set it to the appropriate value and return success status
    protected AxisMessage SetXAxis(float newAxisVal, Priority priority)
    {
        if (priority >= axisXPriority)
        {
            if (newAxisVal >= axisXMin && newAxisVal <= axisXMax)
            {
                axisX = newAxisVal;
                axisXPriority = priority;
                return AxisMessage.Success;
            }
            return AxisMessage.AxisOutOfRange;
        }
        return AxisMessage.InvalidPriority;
    }

    protected AxisMessage SetYAxis(float newAxisVal, Priority priority)
    {
        if (priority >= axisYPriority)
        {
            if (newAxisVal >= axisYMin && newAxisVal <= axisYMax)
            {
                axisY = newAxisVal;
                axisYPriority = priority;
                return AxisMessage.Success;
            }
            return AxisMessage.AxisOutOfRange;
        }
        return AxisMessage.InvalidPriority;
    }

    protected bool RestrictAxis(AxisX axis, Priority priority)
    {
        if (priority >= axisXPriority)
        {
            if (axis == AxisX.Left)
            {
                axisXMin = 0;
                axisXMax = 1f;
            }
            else if (axis == AxisX.Right)
            {
                axisXMin = -1f;
                axisXMax = 0;
            }
            else
            {
                axisXMin = 0;
                axisXMax = 0;
            }
            return true;
        }
        return false;
    }

    protected bool RestrictAxis(AxisY axis, Priority priority)
    {
        if (priority >= axisXPriority)
        {
            if (axis == AxisY.Down)
            {
                axisYMin = 0;
                axisYMax = 1f;
            }
            if (axis == AxisY.Up)
            {
                axisYMin = -1f;
                axisYMax = 0;
            }
            else
            {
                axisYMin = 0;
                axisYMax = 0;
            }
            return true;
        }
        return false;
    }

    protected AxisMessage ClearXAxis(Priority priority)
    {
        if (priority == axisXPriority)
        {
            axisXPriority = Priority.TowardsTarget;
            axisX = 0;
            evadeMechanism = null;
            axisXMin = -1;
            axisXMax = 1;
            return AxisMessage.Success;
        }
        return AxisMessage.InvalidPriority;
    }

    protected AxisMessage ClearYAxis(Priority priority)
    {
        if (priority == axisYPriority)
        {
            axisYPriority = Priority.TowardsTarget;
            axisY = 0;
            evadeMechanism = null;
            axisYMin = -1;
            axisYMax = 1;
            return AxisMessage.Success;
        }
        return AxisMessage.InvalidPriority;
    }

    protected PlayerController CheckToEvade()
    {
        for(var i = 0; i < players.Length; i++)
        {
            if (players[i].CheckIfFiring())
            {
                //TODO Check distance based on explosion radius instead of arbitrary number
                Vector3 pos = players[i].GetTargetPoint();
                if ((players[i].GetTargetPoint() - transform.position).magnitude < evadeDistance)
                {
                    playerFireTarget = pos;
                    return players[i];
                }
            }
        }
        return null;
    }

    protected void Evade(PlayerController player)
    {
        float forwardAngle = Vector3.Angle(player.transform.position - transform.position, transform.forward);
        float rightAngle = Vector3.Angle(player.transform.position - transform.position, transform.right);
        float numX, numY;
        // Depending on the angle from the player firing, attempt to first set axis in a certain direction
        if (rightAngle < 90f)
        {
            numX = -1f;
        }
        else
        {
            numX = 1f;
        }
        if (forwardAngle < 90f)
        {
            numY = 1f;
        }
        else
        {
            numY = -1f;
            numX = -numX;
        }

        // Rotate left, right or not at all depending on X Axis Restrictions
        AxisMessage responseX = SetXAxis(numX, Priority.Evasion);
        if (responseX == AxisMessage.AxisOutOfRange)
        {
            responseX = SetXAxis(-numX, Priority.Evasion);
            if (responseX == AxisMessage.AxisOutOfRange)
            {
                responseX = SetXAxis(0, Priority.Evasion);
            }
        }
        // Move forward or backward depending on Y Axis Restrictions
        AxisMessage responseY = SetYAxis(numY, Priority.Evasion);
        if (responseY == AxisMessage.AxisOutOfRange)
        {
            SetYAxis(-numY, Priority.Evasion);
        }
        // Rotate until tank is 90 degrees from player
        evadeMechanism = new AIEvadeMechanism(90f, 1f, playerFireTarget, evadeDistance);
        lastPlayerFirePos = player.transform.position;
    }

    private float GetDistanceFromPlayer()
    {
        return (playerTarget.position - transform.position).magnitude;
    }

    protected void DetermineTarget()
    {
        if (GetDistanceFromPlayer() > maxDistanceFromPlayer)
        {
            repositionStatus = RepositionStatus.DontReposition;
            SetTarget(Target.Player);
        }
        else if (GetDistanceFromPlayer() < maxDistanceFromPlayer && repositionStatus == RepositionStatus.ReadyToReposition)
        {
            repositionStatus = RepositionStatus.Repositioning;
            SetTarget(Target.RepositionPoint);
            repositionTimer.startRepositionTimer();
        }
        else if (GetDistanceFromPlayer() < maxDistanceFromPlayer && repositionStatus == RepositionStatus.Repositioning)
        {
            repositionStatus = repositionTimer.getRepositionStatus();
        }
        else if (GetDistanceFromPlayer() < maxDistanceFromPlayer && repositionStatus == RepositionStatus.DontReposition)
        {
            repositionStatus = repositionTimer.getRepositionStatus();
        }
        if (GetDistanceFromPlayer() < minDistanceFromPlayer && repositionStatus != RepositionStatus.Repositioning)
        {
            repositionStatus = RepositionStatus.ReadyToReposition;
            repositionTimer.clearTimer();
        }
    }

    private void SetTarget(Target targetType)
    {
        this.targetType = targetType;
        if (targetType == Target.Player)
        {
            repositionTimer.clearTimer();
            target = playerTarget.position;
            maxDistanceFromTarget = maxDistanceFromPlayer;
            minDistanceFromTarget = minDistanceFromPlayer;
        }
        else if (targetType == Target.RepositionPoint)
        {
            target = GetRepositionPoint();
            print("Target: " + target);
            maxDistanceFromTarget = maxDistanceFromRepositionPoint;
            minDistanceFromTarget = minDistanceFromRepositionPoint;
        }
    }

    public Vector3 GetRepositionPoint()
    {
        int layerMask = 1 << 10;
        int inverseLayer = ~(layerMask);

        float xVal = UnityEngine.Random.Range(-40f, 40f);
        float zVal = UnityEngine.Random.Range(-40f, 40f);
        Vector3 vectorAdd = new Vector3(xVal, playerTarget.position.y + 1f, zVal);
        Vector3 point = playerTarget.position + vectorAdd;
        Vector3 repositionPoint = point;

        Ray ray = new Ray(playerTarget.position, point - playerTarget.position);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, (point - playerTarget.position).magnitude, ~(layerMask)))
        {
            repositionPoint = hit.point;
        }

        return repositionPoint;
    }

    protected void ManageTargetDistance()
    {
        if ((target - transform.position).magnitude > maxDistanceFromTarget)
        {
            MoveTowardsTarget();
        }
        else if (targetType == Target.RepositionPoint 
            && (target - transform.position).magnitude < maxDistanceFromTarget
            && repositionStatus == RepositionStatus.Repositioning)
        {
            repositionTimer.startCooldownTimer();
            repositionStatus = repositionTimer.getRepositionStatus();
        }
    }

    protected void MoveTowardsTarget()
    {
        SetYAxis(1f, Priority.TowardsTarget);
        float frontAngle = Vector3.Angle((target - transform.position), transform.forward);
        float angle = Vector3.Angle((target - transform.position), transform.right);
        Debug.DrawLine(transform.position, target);
        Debug.DrawLine(transform.position, (transform.position - transform.forward));
        if (angle < 80f)
        {
            SetXAxis(1f, Priority.TowardsTarget);
        }
        else if (angle > 100f)
        {
            SetXAxis(-1f, Priority.TowardsTarget);
        }
        else
        {
            ClearXAxis(Priority.TowardsTarget);
        }

        if (angle > 100f)
        {
            RestrictAxis(AxisY.Y, Priority.TowardsTarget);
        }
        else
        {
            ClearYAxis(Priority.TowardsTarget);
        }
        
        vehicle.Move(axisY, transform);
        vehicle.Rotate(axisX, transform);
    }

    protected bool CheckIfReadyToManageAiming()
    {
        if (targetType == Target.Player)
        {
            return false;
        }

        return true;
    }
}
