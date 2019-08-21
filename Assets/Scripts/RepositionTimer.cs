using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepositionTimer : MonoBehaviour
{
    public enum RepositionStatus { DontReposition, ReadyToReposition, Repositioning }
    protected enum Timer { RepositionTimer, CooldownTimer, None }
    public AIVehicle.RepositionStatus status = AIVehicle.RepositionStatus.ReadyToReposition;
    Timer timer = Timer.None;

    float deltaTime = 0f;
    float timeLimit = 10f;

    // Update is called once per frame
    public void Update()
    {
        if (timer == Timer.RepositionTimer)
        {
            deltaTime += Time.deltaTime;
            if (deltaTime >= timeLimit)
            {
                startCooldownTimer();
            }
        }
        else if (timer == Timer.CooldownTimer)
        {
            deltaTime += Time.deltaTime;
            if (deltaTime >= timeLimit)
            {
                clearTimer();
            }
        }
        else
        {
            status = AIVehicle.RepositionStatus.ReadyToReposition;
        }
    }

    public AIVehicle.RepositionStatus getRepositionStatus()
    {
        return status;
    }

    public void startRepositionTimer()
    {
        status = AIVehicle.RepositionStatus.Repositioning;
        timer = Timer.RepositionTimer;
        deltaTime = 0;
    }

    public void startCooldownTimer()
    {
        status = AIVehicle.RepositionStatus.DontReposition;
        timer = Timer.CooldownTimer;
        deltaTime = 0;
    }

    public void clearTimer()
    {
        status = AIVehicle.RepositionStatus.ReadyToReposition;
        timer = Timer.None;
        deltaTime = 0;
    }
}
