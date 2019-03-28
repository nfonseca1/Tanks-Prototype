using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTank : Tank
{
    [SerializeField] ParticleSystem particleSystem1;
    [SerializeField] ParticleSystem particleSystem2;
    ParticleSystem.EmissionModule frontEmission;
    ParticleSystem.EmissionModule backEmission;
    Trajectory trajectory;

    Vector3 targetRotPositive;
    Vector3 targetRotNegative;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        trajectory = turret.GetComponent<Trajectory>();
        frontEmission = particleSystem1.emission;
        backEmission = particleSystem2.emission;
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

    public void ElevateBarrel()
    {
        barrelWheel.Rotate(new Vector3(elevateSpeed * Time.deltaTime, 0, 0), Space.Self);
        if (barrelWheel.localEulerAngles.x >= maxBarrelHeight)
        {
            barrelWheel.localEulerAngles = new Vector3(maxBarrelHeight, defaultBarrelRot.y, defaultBarrelRot.z);
        }
        //trajectory.Calculate(barrelWheel.localEulerAngles.x, launchVelocity, emitter);
    }

    public void ManageGrounding()
    {
        //bool grounded = CheckIfGrounded();

        //if (!grounded)
        //{
        //    Move(0);
        //    print("fixing");
        //    float angleX = transform.localEulerAngles.x;
        //    angleX = (angleX > 180) ? angleX - 360 : angleX;
        //    float angleY = transform.localEulerAngles.y;
        //    angleY = (angleY > 180) ? angleY - 360 : angleY;
        //    float angleZ = transform.localEulerAngles.z;
        //    angleZ = (angleZ > 180) ? angleZ - 360 : angleZ;

        //    targetRotPositive = new Vector3(18f, angleY, 18f);
        //    targetRotNegative = new Vector3(-18f, angleY, -18f);

        //    Vector3 angles = new Vector3(angleX, angleY, angleZ);
        //    if (angles.x < -20f || angles.z < -20f)
        //    {
        //        Vector3 fixedRot = Vector3.Lerp(angles, targetRotNegative, 0.1f);
        //        Quaternion fixedQuat = Quaternion.Euler(fixedRot.x, fixedRot.y, fixedRot.z);
        //        rigidbody.MoveRotation(fixedQuat);
        //    }
        //    else if (angles.x > 20f || angles.z > 20f)
        //    {
        //        Vector3 fixedRot = Vector3.Lerp(angles, targetRotPositive, 0.1f);
        //        Quaternion fixedQuat = Quaternion.Euler(fixedRot.x, fixedRot.y, fixedRot.z);
        //        rigidbody.MoveRotation(fixedQuat);
        //    }
        //}
    }

    bool CheckIfGrounded()
    {
        Ray ray = new Ray(transform.position, -transform.up);
        Debug.DrawRay(transform.position, -transform.up * 2);
        if (Physics.Raycast(ray, 2f))
        {
            return true;
        }
        return false;
    }

    public Transform GetEmitter()
    {
        return emitter;
    }
}
