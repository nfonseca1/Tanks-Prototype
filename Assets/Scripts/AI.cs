using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    protected PlayerController[] players;

    protected Transform GetClosestPlayer(Transform currentTransform)
    {
        players = FindObjectsOfType<PlayerController>();
        Transform closestPlayer = null;
        float closestDistance = 1000000f;
        if (players[0].grounded)
        {
            closestPlayer = players[0].transform;
            closestDistance = (players[0].transform.position - currentTransform.position).magnitude;
        }

        for (var i = 0; i < players.Length; i++)
        {
            if (i == 0) { continue; }

            if (players[i].grounded)
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
}
