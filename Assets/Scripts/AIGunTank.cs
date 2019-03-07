using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIGunTank : Tank
{
    [SerializeField] Transform sensorPointF, sensorPointFR, sensorPointFL, sensorPointL, sensorPointR;
    [SerializeField] ParticleSystem particleSystem1;
    [SerializeField] ParticleSystem particleSystem2;
    ParticleSystem.EmissionModule frontEmission;
    ParticleSystem.EmissionModule backEmission;

    public enum Sensor { Front, FrontRight, FrontLeft, Left, Right, None }
    float rayLength = 6f;
    Trajectory trajectory;
    Vector3 currentHitpoint;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        trajectory = turret.GetComponent<Trajectory>();
        frontEmission = particleSystem1.emission;
        backEmission = particleSystem2.emission;
        frontEmission.enabled = false;
        backEmission.enabled = false;
    }

    public void Move(float input)
    {
        Vector3 newPosition = transform.position + (transform.forward * speed * input * Time.deltaTime);
        Vector3 newPositionXZ = new Vector3(newPosition.x, transform.position.y, newPosition.z);
        rigidbody.MovePosition(newPositionXZ);

        if (input > 0)
        {
            frontEmission.enabled = true;
            backEmission.enabled = false;
        }
        else if (input < 0)
        {
            frontEmission.enabled = false;
            backEmission.enabled = true;
        }
        else
        {
            frontEmission.enabled = false;
            backEmission.enabled = false;
        }
    }

    public Sensor CheckSensors()
    {
        Ray frontRay = new Ray(sensorPointF.position, sensorPointF.forward);
        Ray frontRightRay = new Ray(sensorPointFR.position, sensorPointFR.forward);
        Ray frontLeftRay = new Ray(sensorPointFL.position, sensorPointFL.forward);
        Ray leftRay = new Ray(sensorPointL.position, sensorPointL.forward);
        Ray rightRay = new Ray(sensorPointR.position, sensorPointR.forward);


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
        barrelWheel.LookAt(aimEuler);
        barrelWheel.localEulerAngles = new Vector3(barrelWheel.localEulerAngles.x, 0, 0);

        if (barrelWheel.localEulerAngles.x >= 40)
        {
            barrelWheel.localEulerAngles = new Vector3(40, 0, 0);
        }
        return true;
    }

    public Vector3 CalculateAimAngle(Vector3 hitPoint)
    {
        currentHitpoint = hitPoint;
        return hitPoint;
    }

    public float GetTrajectoryTime()
    {
        return 1;
    }

    public bool CheckBarrelClearance()
    {
        Ray ray = new Ray(emitter.position, emitter.forward);
        if (Physics.Raycast(ray, (currentHitpoint - transform.position).magnitude))
        {
            return false;
        }
        return true;
    }
}
