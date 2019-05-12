using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIVehicle : AI
{
    const float getPlayerInterval = 5f;
    const float sensorLength = 5f;
    bool getPlayerNow = false;
    int randomDirection = 1;
    Transform playerTarget;
    Transform sensorPointF, sensorPointFL, sensorPointFR, sensorPointL, sensorPointR;
    float axisXMin = -1f, axisXMax = 1f, axisYMin = -1f, axisYMax = 1f;
    float axisX = 0, axisY = 0;

    protected enum Sensor { Front, FrontRight, FrontLeft, Left, Right, None }
    protected enum AxisX { Left, X, Right}
    protected enum AxisY { Up, Y, Down}
    protected enum AxisMessage { InvalidPriority = 0, AxisOutOfRange = 1, Success = 2}
    protected enum Priority { None = 0, TowardsTarget = 1, Evasion = 2, SensorResponse = 3 }
    Priority axisXPriority = Priority.None, axisYPriority = Priority.None;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SetGetPlayerTag(getPlayerInterval));
    }

    IEnumerator SetGetPlayerTag(float interval)
    {
        while (true)
        {
            getPlayerNow = true;
            yield return new WaitForSeconds(interval);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (getPlayerNow)
        {
            GetClosestPlayer(transform);
        }

        if (playerTarget != null)
        {
            DeterminePriority();
        }
    }

    void DeterminePriority()
    {
        RespondToSensors();
    }

    void RespondToSensors()
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
    AxisMessage SetXAxis(float newAxisVal, Priority priority)
    {
        if (priority >= axisXPriority)
        {
            if (newAxisVal >= axisXMin && newAxisVal <= axisXMax)
            {
                axisX = newAxisVal;
                return AxisMessage.Success;
            }
            return AxisMessage.AxisOutOfRange;
        }
        return AxisMessage.InvalidPriority;
    }

    AxisMessage SetYAxis(float newAxisVal, Priority priority)
    {
        if (priority >= axisYPriority)
        {
            if (newAxisVal >= axisYMin && newAxisVal <= axisYMax)
            {
                axisY = newAxisVal;
                return AxisMessage.Success;
            }
            return AxisMessage.AxisOutOfRange;
        }
        return AxisMessage.InvalidPriority;
    }

    bool RestrictAxis(AxisX axis, Priority priority)
    {
        if (priority >= axisXPriority)
        {
            if (axis == AxisX.Left)
            {
                axisXMin = 0;
                axisXMax = 1f;
            }
            if (axis == AxisX.Right)
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

    bool RestrictAxis(AxisY axis, Priority priority)
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

    AxisMessage ClearXAxis(Priority priority)
    {
        if (priority == axisXPriority)
        {
            axisXPriority = Priority.TowardsTarget;
            axisX = 0;
            return AxisMessage.Success;
        }
        return AxisMessage.InvalidPriority;
    }

    AxisMessage ClearYAxis(Priority priority)
    {
        if (priority == axisYPriority)
        {
            axisYPriority = Priority.TowardsTarget;
            axisY = 0;
            return AxisMessage.Success;
        }
        return AxisMessage.InvalidPriority;
    }
}
