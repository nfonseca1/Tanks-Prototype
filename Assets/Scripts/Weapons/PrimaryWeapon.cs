using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrimaryWeapon : MonoBehaviour, IPrimaryWeapon
{
    Transform barrelWheel;
    float elevateSpeed = 15f;
    float maxBarrelHeight = 50f;
    Vector3 defaultBarrelRot;

    public bool Fire() // To be overridden in child
    {
        return false;
    }

    public Vector3 GetSecondarySocketPoint()
    {
        return Vector3.zero;
    }

    public bool RotateBarrel(float input)
    {
        barrelWheel.Rotate(new Vector3(elevateSpeed * Time.deltaTime, 0, 0), Space.Self);
        if (barrelWheel.localEulerAngles.x >= maxBarrelHeight)
        {
            barrelWheel.localEulerAngles = new Vector3(maxBarrelHeight, defaultBarrelRot.y, defaultBarrelRot.z);
            return true;
        }
        return false;
    }

    public bool RotateBarrelToAngle(float angle)
    {
        Vector3 aimEuler = new Vector3(angle, barrelWheel.localEulerAngles.y, barrelWheel.localEulerAngles.z);
        barrelWheel.localEulerAngles = Vector3.Lerp(barrelWheel.localEulerAngles, aimEuler, 0.2f);

        if (aimEuler.x - barrelWheel.localEulerAngles.x < 1)
        {
            barrelWheel.localEulerAngles = aimEuler;
            return true;
        }
        return false;
    }

    public bool RotateTurret(float input)
    {
        return false;
    }

    public bool RotateTurretTowardsTarget(Vector3 target)
    {
        // TODO check angles and rotate left or right rather than through quaternion look
        Vector3 turretLookPoint = new Vector3(target.x, transform.position.y, target.z);
        Quaternion targetRotation = Quaternion.LookRotation(turretLookPoint - transform.position, transform.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, .3f);
        transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);

        return true;
    }
}
