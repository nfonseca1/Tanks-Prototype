using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIEvadeMechanism : MonoBehaviour
{
    float targetAngle = 90f;
    float duration = 1f;
    float timeSinceEvade = 0f;
    float distanceFromEvadePoint = 1f;
    Vector3 positionToAvoid;
    
    public AIEvadeMechanism(float xAxisAngle, float yAxisDuration, Vector3 playerFireTarget, float evadeDistance)
    {
        targetAngle = xAxisAngle;
        duration = yAxisDuration;
        positionToAvoid = playerFireTarget;
        distanceFromEvadePoint = evadeDistance;
    }

    private bool CheckToClearAxes(Vector3 currentPosition)
    {
        if ((currentPosition - positionToAvoid).magnitude >= distanceFromEvadePoint)
        {
            return true;
        }
        return false;
    }

    public bool CheckToClearXAxis(float currentAngle, Vector3 currentPosition)
    {
        if (CheckToClearAxes(currentPosition)) { return true; }
        if (currentAngle > targetAngle - 5f && currentAngle < targetAngle + 5f)
        {
            return true;
        }
        return false;
    }

    public bool CheckToClearYAxis(float deltaTime, Vector3 currentPosition)
    {
        if (CheckToClearAxes(currentPosition)) { return true; }
        // TODO fix magic number 5 (number for approximation)
        timeSinceEvade += deltaTime;
        if (timeSinceEvade >= duration)
        {
            return true;
        }
        return false;
    }
}
