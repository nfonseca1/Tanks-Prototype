using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    public bool grounded = true;

    AITank AITank;
    Vector3 hitPoint;
    PlayerTank[] players;
    Transform closestPlayer;
    float maxDistance = 35f;
    float minDistance = 25f;
    float neutralDistance = 30f;
    float maxShootDistance = 40f;
    bool isMoving = false;

    float axisX = 0;
    float axisY = 0;
    float lerpX = 0;
    float lerpY = 0;
    float lerpLimitX = 0.25f;
    float lerpLimitY = 0.5f;
    float fireRate = 2f;
    float timeUntilFire = 0f;
    bool playersExist = false;
    float randomDirection = 1;
    bool axisXOverriden = false;
    bool axisYOverriden = false;
    float flightTime;
    enum TrajectoryStatus { Success, Fail, Standby }
    TrajectoryStatus flightStatus = TrajectoryStatus.Standby;
    float repositionTime = 0f;
    bool engage = false;

    void Start()
    {
        AITank = GetComponent<AITank>();

        StartCoroutine(GetClosestPlayerWithDelay());
    }

    void Update()
    {
        if (engage && grounded)
        {
            GetPlayers();
            if (playersExist)
            {
                CalculateAimTarget();
            }

            if (closestPlayer)
            {
                CheckSensors();
                ManageAxisInput(axisX, ref lerpX, lerpLimitX);
                ManageAxisInput(axisY, ref lerpY, lerpLimitY);

                AITank.Move(lerpY / lerpLimitY); // lerp / lerpLimit dictates the percentage of movement
                AITank.Rotate(lerpX / lerpLimitX);
                AITank.Aim(hitPoint);
                if (playersExist)
                {
                    ManageFireInput();
                }
            }
            else
            {
                AITank.Move(0);
            }
        }
    }

    void GetPlayers()
    {
        players = FindObjectsOfType<PlayerTank>();
        if (players.Length == 0)
        {
            playersExist = false;
        }
        else
        {
            playersExist = true;
        }
    }

    void ManageFlightStatus()
    {
        if(flightStatus == TrajectoryStatus.Fail)
        {
            maxDistance = 5;
        }
    }

    void CheckSensors()
    {
        AITank.Sensor sensor = AITank.CheckSensors();
        switch (sensor)
        {
            case AITank.Sensor.Front:
                axisX = randomDirection;
                axisY = 0;
                axisXOverriden = true;
                axisYOverriden = true;
                break;
            case AITank.Sensor.FrontRight:
                axisX = -1;
                axisXOverriden = true;
                axisYOverriden = false;
                break;
            case AITank.Sensor.FrontLeft:
                axisX = 1;
                axisXOverriden = true;
                axisYOverriden = false;
                break;
            case AITank.Sensor.Left:
                if(axisX < 0)
                {
                    axisX = 0;
                    axisXOverriden = true;
                    axisYOverriden = false;
                }
                else
                {
                    axisXOverriden = false;
                    axisYOverriden = false;
                }
                break;
            case AITank.Sensor.Right:
                if (axisX > 0)
                {
                    axisX = 0;
                    axisXOverriden = true;
                    axisYOverriden = false;
                }
                else
                {
                    axisXOverriden = false;
                    axisYOverriden = false;
                }
                break;
            default:
                float num = Mathf.RoundToInt(UnityEngine.Random.Range(0, 2));
                if (num == 0) { num = -1; }
                randomDirection = num;
                
                axisXOverriden = false;
                axisYOverriden = false;
                break;
        }
        ManageMovement();
    }

    private void ManageMovement()
    {
        if ((hitPoint - transform.position).magnitude > maxDistance)
        {
            if (!axisYOverriden) { axisY = 1; }

            if (!axisXOverriden)
            {
                // Calculate direction of target for rotation
                float frontAngle = Vector3.Angle(transform.forward, hitPoint - transform.position);
                float rightAngle = Vector3.Angle(transform.right, hitPoint - transform.position);

                if (frontAngle > 10f && rightAngle > 90f)
                {
                    axisX = -1f;
                }
                else if (frontAngle > 10f && rightAngle < 90f)
                {
                    axisX = 1f;
                }
                else
                {
                    axisX = 0;
                }
            }
        }
        else
        {
            if(flightStatus == TrajectoryStatus.Fail)
            {
                ChangePosition();
                if (!axisXOverriden) { axisX = 0; }
                if (!axisYOverriden) { axisY = 1; }
            }
            else
            {
                if (!axisXOverriden) { axisX = 0; }
                if (!axisYOverriden) { axisY = 0; }
            }
        }
    }

    private void ChangePosition()
    {
        repositionTime += Time.deltaTime;
        if(repositionTime >= 1.5f)
        {
            repositionTime = 0;
            flightStatus = TrajectoryStatus.Standby;
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
                    bool clear = AITank.CheckBarrelClearance();
                    if (clear)
                    {
                        flightTime = AITank.GetTrajectoryTime();
                        Shell shell = AITank.Fire();
                        shell.SetAITankSource(this);
                        timeUntilFire = fireRate;
                    }
                    else
                    {
                        flightStatus = TrajectoryStatus.Fail;
                    }
                }
            }
        }
        else
        {
            timeUntilFire -= Time.deltaTime;

            AITank.UpdateBarrel();
        }

    }

    IEnumerator GetClosestPlayerWithDelay()
    {
        while (true)
        {
            GetPlayers();
            if(players.Length == 0)
            {
                playersExist = false;
            }
            else
            {
                playersExist = true;
                GetClosestPlayerNow();
            }
            yield return new WaitForSeconds(5f);
        }
    }

    private void GetClosestPlayerNow()
    {
        float closestDistance;
        if (players[0].GetComponent<PlayerController>().grounded)
        {
            closestPlayer = players[0].transform;
            closestDistance = (players[0].transform.position - transform.position).magnitude;
        }
        else
        {
            closestPlayer = null;
            closestDistance = 1000000f;
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
    }

    private bool CalculateAimTarget()
    {
        if (closestPlayer != null)
        {
            hitPoint = closestPlayer.position;
        }
        else
        {
            GetClosestPlayerNow();
        }

        return true;
    }

    public void SetProjectileTime(float time)
    {
        if(time / flightTime >= 0.95f)
        {
            flightStatus = TrajectoryStatus.Success;
        }
        else
        {
            flightStatus = TrajectoryStatus.Fail;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        Zone zone = other.GetComponent<Zone>();
        if (zone != null)
        {
            if (zone.readyToAttack == true)
            {
                engage = true;
            }
        }
    }
}
