using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    int shotsBeforeCooloff = 0;
    float cooloff = 4f;
    float fireRate = 2f;

    int shotsFired = 0;
    float currentCooloff = 0;
    float fireRateTime = 0;

    
    public AIController(int shotsParam, float cooloffParam, float fireRateParam)
    {
        shotsBeforeCooloff = shotsParam;
        cooloff = cooloffParam;
        fireRate = fireRateParam;
    }

    public PlayerTank[] GetPlayers()
    {
        return FindObjectsOfType<PlayerTank>();
    }

    public Transform GetClosestPlayer(PlayerTank[] players)
    {
        Transform closestPlayer = null;
        float closestDistance = 1000000f;
        if (players[0].GetComponent<PlayerController>().grounded)
        {
            closestPlayer = players[0].transform;
            closestDistance = (players[0].transform.position - transform.position).magnitude;
        }

        for (var i = 0; i < players.Length; i++)
        {
            if (i == 0) { continue; }

            if (players[i].GetComponent<PlayerController>().grounded)
            {
                float distanceToCheck = (players[i].transform.position - transform.position).magnitude;
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

    public float CalculateAimAngle(Vector3 hitPoint, float launchVelocity, bool aimByLowArc)
    {
        if (aimByLowArc)
        {
            float aimDistance = (new Vector3(hitPoint.x, transform.position.y, hitPoint.z) - transform.position).magnitude;
            float aimAngle = 0.5f * (Mathf.Asin((Physics.gravity.y * aimDistance) / Mathf.Pow(launchVelocity, 2)) * Mathf.Rad2Deg);
            return -aimAngle;
        }
        else
        {
            float aimDistance = (new Vector3(hitPoint.x, transform.position.y, hitPoint.z) - transform.position).magnitude;
            float aimAngle = 0.5f * (Mathf.Asin((Physics.gravity.y * aimDistance) / Mathf.Pow(launchVelocity, 2)) * Mathf.Rad2Deg);
            return -(90 - aimAngle);
        }
        
    }

    public float CalculateAimAngle(Vector3 hitPoint, Transform barrel)
    {
        return Vector3.Angle(barrel.forward, (hitPoint - transform.position));
    }

    public bool CheckIfReadyToFire()
    {
        if (shotsFired >= shotsBeforeCooloff)
        {
            currentCooloff += Time.deltaTime;
            if (currentCooloff >= cooloff)
            {
                currentCooloff = 0;
                shotsFired = 0;
            }
        }
        if (fireRateTime <= 0 && currentCooloff == 0)
        {
            return true;
        }
        else
        {
            fireRateTime -= Time.deltaTime;
        }
        return false;
    }
}
