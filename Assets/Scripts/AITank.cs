﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AITank : Tank
{
    [SerializeField] Transform sensorPointF, sensorPointFR, sensorPointFL, sensorPointL, sensorPointR;

    public enum Sensor { Front, FrontRight, FrontLeft, Left, Right, None }
    float rayLength = 6f;
    Trajectory trajectory;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        trajectory = turret.GetComponent<Trajectory>();
    }

    public Sensor CheckSensors()
    {
        Ray frontRay = new Ray(sensorPointF.position, sensorPointF.forward);
        Ray frontRightRay = new Ray(sensorPointFR.position, sensorPointFR.forward);
        Ray frontLeftRay = new Ray(sensorPointFL.position, sensorPointFL.forward);
        Ray leftRay = new Ray(sensorPointL.position, sensorPointL.forward);
        Ray rightRay = new Ray(sensorPointR.position, sensorPointR.forward);

        Debug.DrawRay(sensorPointF.position, sensorPointF.forward * rayLength);
        Debug.DrawRay(sensorPointFR.position, sensorPointFR.forward * rayLength);
        Debug.DrawRay(sensorPointFL.position, sensorPointFL.forward * rayLength);
        Debug.DrawRay(sensorPointL.position, sensorPointL.forward * rayLength);
        Debug.DrawRay(sensorPointR.position, sensorPointR.forward * rayLength);

        
        RaycastHit hitInfo;
        if (Physics.Raycast(frontRay, out hitInfo, rayLength))
        {
            return Sensor.Front;
        }
        if (Physics.Raycast(frontRightRay, out hitInfo, rayLength))
        {
            return Sensor.FrontRight;
        }
        if (Physics.Raycast(frontLeftRay, out hitInfo, rayLength))
        {
            return Sensor.FrontLeft;
        }
        if (Physics.Raycast(leftRay, out hitInfo, rayLength))
        {
            return Sensor.Left;
        }
        if (Physics.Raycast(rightRay, out hitInfo, rayLength))
        {
            return Sensor.Right;
        }
        return Sensor.None;
    }

    public bool ElevateBarrel(Vector3 aimEuler)
    {
        barrelWheel.localEulerAngles = Vector3.Lerp(barrelWheel.localEulerAngles, aimEuler, 0.2f);

        if(aimEuler.x - barrelWheel.localEulerAngles.x < 1)
        {
            barrelWheel.localEulerAngles = aimEuler;
            return true;
        }
        return false;
    }

    public Vector3 CalculateAimAngle(Vector3 hitPoint)
    {
        float aimDistance = (new Vector3(hitPoint.x, transform.position.y, hitPoint.z) - transform.position).magnitude;
        float aimAngle = 0.5f * (Mathf.Asin((Physics.gravity.y * aimDistance) / Mathf.Pow(launchVelocity, 2)) * Mathf.Rad2Deg);
        return new Vector3(-aimAngle, defaultBarrelRot.y, defaultBarrelRot.z);
    }

}
