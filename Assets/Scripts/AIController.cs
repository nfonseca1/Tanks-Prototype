using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    AITank AITank;
    Vector3 hitPoint;
    PlayerTank[] players;
    Transform closestPlayer;
    float maxDistance = 45f;
    float minDistance = 25f;
    float neutralDistance = 30f;
    float maxShootDistance = 50f;
    bool isMoving = false;

    float axisX = 0;
    float axisY = 0;
    float lerpX = 0;
    float lerpY = 0;
    float lerpLimitX = 0.25f;
    float lerpLimitY = 0.5f;
    float fireRate = 2f;
    float timeUntilFire = 0f;

    void Start()
    {
        AITank = GetComponent<AITank>();
        GetPlayers();
    }

    void Update()
    {
        StartCoroutine(GetClosestPlayer());
        CalculateAimTarget();
        ManageMovement();
        ManageAxisInput(axisX, ref lerpX, lerpLimitX);
        ManageAxisInput(axisY, ref lerpY, lerpLimitY);

        AITank.Move(lerpY / lerpLimitY); // lerp / lerpLimit dictates the percentage of movement
        AITank.Rotate(lerpX / lerpLimitX);
        AITank.Aim(hitPoint);
        ManageFireInput();
    }

    void GetPlayers()
    {
        players = FindObjectsOfType<PlayerTank>();
    }

    private void ManageMovement()
    {
        if ((hitPoint - transform.position).magnitude > maxDistance)
        {
            axisY = 1;

            // Calculate direction of target for rotation
            float frontAngle = Vector3.Angle(transform.forward, hitPoint - transform.position);
            float rightAngle = Vector3.Angle(transform.right, hitPoint - transform.position);

            if(frontAngle > 10f && rightAngle > 90f)
            {
                axisX = -1f;
            }
            else if(frontAngle > 10f && rightAngle < 90f)
            {
                axisX = 1f;
            }
            else
            {
                axisX = 0;
            }
        }
        else
        {
            axisX = 0;
            axisY = 0;
        }
    }

    private void ManageAxisInput(float input, ref float lerp, float lerpLimit)
    {
        if (input > 0) // If we are pressing the axis input, add or subtract from the lerp
        {
            lerp += Time.deltaTime;
            if (lerp > lerpLimit) { lerp = lerpLimit; }
        }
        else if (input < 0)
        {
            lerp -= Time.deltaTime;
            if (lerp < -lerpLimit) { lerp = -lerpLimit; }
        }
        else // Otherwise add or subtract the lerp back to zero
        {
            if (lerp < 0)
            {
                lerp += Time.deltaTime;
                if (lerp >= 0) { lerp = 0; }
            }
            else if (lerp > 0)
            {
                lerp -= Time.deltaTime;
                if (lerp <= 0) { lerp = 0; }
            }
        }
    }

    private void ManageFireInput()
    {
        bool aimStatus = false;

        if (timeUntilFire <= 0)
        {
            if (lerpY == 0)
            {
                Vector3 aimEuler = AITank.CalculateAimAngle(hitPoint);
                aimStatus = AITank.ElevateBarrel(aimEuler);

                if (aimStatus == true)
                {
                    AITank.Fire();
                    timeUntilFire = fireRate;
                }
            }
        }
        else
        {
            timeUntilFire -= Time.deltaTime;

            AITank.UpdateBarrel();
        }

    }

    IEnumerator GetClosestPlayer()
    {
        while (true)
        {
            closestPlayer = players[0].transform;
            float closestDistance = (players[0].transform.position - transform.position).magnitude;

            for (var i = 0; i < players.Length; i++)
            {
                if (i == 0) { continue; }

                float distanceToCheck = (players[i].transform.position - transform.position).magnitude;
                if (distanceToCheck < closestDistance)
                {
                    closestPlayer = players[i].transform;
                    closestDistance = distanceToCheck;
                }
            }
            yield return new WaitForSeconds(5f);
        }
    }

    private bool CalculateAimTarget()
    {
        hitPoint = closestPlayer.position;

        return true;
    }
}
