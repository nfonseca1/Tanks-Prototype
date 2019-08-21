using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBasicVehicle : AIVehicle
{
    // Weapon instances go here 
    [SerializeField] IPrimaryWeapon primary;
    

    void Start()
    {
        players = null;
        StartCoroutine(SetGetPlayerTag(getPlayerInterval));
        vehicle = GetComponent<Vehicle>();
        repositionTimer = GetComponent<RepositionTimer>();
    }

    void Update()
    { 
        if (evadeMechanism != null)
        {
            ManageLastEvasion();
        }
        if (getPlayerNow)
        {
            playerTarget = GetClosestPlayer(transform);
        }

        if (playerTarget != null)
        {
            RespondToSensors();
            PlayerController playerToEvade = CheckToEvade();
            if(playerToEvade != null)
            {
                Evade(playerToEvade);
            }
            if (axisYPriority == Priority.TowardsTarget)
            {
                DetermineTarget();
                ManageTargetDistance();
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
