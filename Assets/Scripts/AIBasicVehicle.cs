using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBasicVehicle : AIVehicle
{
    // Weapon instances go here 
    

    void Start()
    {
        players = null;
        StartCoroutine(SetGetPlayerTag(getPlayerInterval));
    }

    void Update()
    {
        if (evadeMechanism != null)
        {
            ManageLastEvasion();
        }
        if (getPlayerNow)
        {
            GetClosestPlayer(transform);
        }

        if (playerTarget != null)
        {
            DeterminePriority();
            if (axisYPriority == Priority.TowardsTarget)
            {
                DetermineTarget();
                ManagePlayerDistance();
            }
            if (CheckIfReadyToManageAiming() == true)
            {
                ManageAiming();
            }
        }
    }

    void ManageAiming()
    {
        // Rotate turret

        // Elevate barrel

    }
}
