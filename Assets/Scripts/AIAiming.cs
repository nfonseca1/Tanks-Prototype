using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAiming : MonoBehaviour
{
    Transform turret;
    Transform barrelWheel;
    Transform[] barrels;
    int currentBarrel = 0;
    Shell shell;


    public AIAiming(Transform turretParam, Transform barrelWheelParam, Transform[] barrelsParam)
    {
        turret = turretParam;
        barrelWheel = barrelWheelParam;
        barrels = barrelsParam;
        shell = new Shell();
    }

    public void AimTurret(Vector3 hitPoint)
    {
        Vector3 turretLookPoint = new Vector3(hitPoint.x, turret.position.y, hitPoint.z);
        Quaternion targetRotation = Quaternion.LookRotation(turretLookPoint - turret.position, turret.up);
        turret.rotation = Quaternion.Lerp(turret.rotation, targetRotation, .3f);
        turret.localEulerAngles = new Vector3(0, turret.localEulerAngles.y, 0);
    }

    public bool AimBarrel(Vector3 aimEuler)
    {
        barrelWheel.localEulerAngles = Vector3.Lerp(barrelWheel.localEulerAngles, aimEuler, 0.2f);

        if (aimEuler.x - barrelWheel.localEulerAngles.x < 1)
        {
            barrelWheel.localEulerAngles = aimEuler;
            return true;
        }
        return false;
    }

    public Shell Fire(float launchVelocity)
    {
        Transform thisEmitter = barrels[currentBarrel].GetComponentInChildren<Transform>();
        Shell currentShell = Instantiate(shell, thisEmitter.position, thisEmitter.rotation);
        currentShell.ApplyForce(launchVelocity);
        Destroy(currentShell.gameObject, 10f);

        //rigidbody.AddExplosionForce(explosionForce, explosionPoint.position, 100f, explosionLift);
        currentBarrel++;
        if (currentBarrel > barrels.Length - 1)
        {
            currentBarrel = 0;
        }
        
        return currentShell;
    }

    public bool CheckBarrelClearance(float sensorLength)
    {
        Transform emitter = barrels[currentBarrel].GetComponentInChildren<Transform>();
        Ray ray = new Ray(emitter.position, emitter.forward);
        if (Physics.Raycast(ray, sensorLength))
        {
            return false;
        }
        return true;
    }

    public float GetTrajectoryTime(float launchVelocity)
    {
        float gravity = Mathf.Abs(Physics.gravity.y);
        float time = (2 * launchVelocity * Mathf.Sin(barrelWheel.eulerAngles.x * Mathf.Deg2Rad)) / gravity;
        return time;
    }

    public int GetCurrentBarrelIndex()
    {
        return currentBarrel;
    }
}
