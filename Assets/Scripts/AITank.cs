using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AITank : Tank
{
    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
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
        float aimDistance = (hitPoint - transform.position).magnitude;
        float aimAngle = 0.5f * (Mathf.Asin((Physics.gravity.y * aimDistance) / Mathf.Pow(launchVelocity, 2)) * Mathf.Rad2Deg);
        return new Vector3(-aimAngle, defaultBarrelRot.y, defaultBarrelRot.z);
    }
}
