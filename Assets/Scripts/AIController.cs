using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    public PlayerTank[] GetPlayers()
    {
        return FindObjectsOfType<PlayerTank>();
    }

    public Transform GetClosestPlayer(PlayerTank[] players, Transform currentTransform)
    {
        Transform closestPlayer = null;
        float closestDistance = 1000000f;
        if (players[0].GetComponent<PlayerController>().grounded)
        {
            closestPlayer = players[0].transform;
            closestDistance = (players[0].transform.position - currentTransform.position).magnitude;
        }

        for (var i = 0; i < players.Length; i++)
        {
            if (i == 0) { continue; }

            if (players[i].GetComponent<PlayerController>().grounded)
            {
                float distanceToCheck = (players[i].transform.position - currentTransform.position).magnitude;
                if (distanceToCheck < closestDistance)
                {
                    closestPlayer = players[i].transform;
                    closestDistance = distanceToCheck;
                }
            }
            else
            {
                continue;
            }
        }

        return closestPlayer;
    }

    public float CalculateAimAngle(Vector3 hitPoint, float launchVelocity, bool aimByLowArc, Transform currentTransform)
    {
        if (aimByLowArc)
        {
            float aimDistance = (new Vector3(hitPoint.x, currentTransform.position.y, hitPoint.z) - currentTransform.position).magnitude;
            float aimAngle = 0.5f * (Mathf.Asin((Physics.gravity.y * aimDistance) / Mathf.Pow(launchVelocity, 2)) * Mathf.Rad2Deg);
            return -aimAngle;
        }
        else
        {
            float aimDistance = (new Vector3(hitPoint.x, currentTransform.position.y, hitPoint.z) - currentTransform.position).magnitude;
            float aimAngle = (0.5f * Mathf.Asin((Physics.gravity.y * aimDistance) / Mathf.Pow(launchVelocity, 2))) * Mathf.Rad2Deg;
            return (90 - -aimAngle);
        }
        
    }

    public float CalculateAimAngle(Vector3 hitPoint, Transform barrelWheel, Transform currentTransform)
    {
        Vector3 reference = new Vector3(hitPoint.x, barrelWheel.position.y, hitPoint.z);
        float angle = Vector3.Angle(hitPoint - barrelWheel.position, reference - barrelWheel.position);
        return angle;
    }
}
