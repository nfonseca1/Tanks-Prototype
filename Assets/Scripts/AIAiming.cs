using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAiming : MonoBehaviour
{
    Transform turret;
    Transform barrelWheel;
    Transform[] barrels;
    Transform[] emitters;
    int currentBarrel = 0;
    Shell shell;

    int shotsBeforeCooloff = 0;
    float cooloff = 4f;
    float fireRate = 2f;

    int shotsFired = 0;
    float currentCooloff = 0;
    float fireRateTime = 0;


    public AIAiming(Transform turretParam, Transform barrelWheelParam, Transform[] barrelsParam, 
        Transform[] emittersParam, Shell shellParam, int shotsBeforeCooloffParam, float cooloffParam, float fireRateParam)
    {
        turret = turretParam;
        barrelWheel = barrelWheelParam;
        barrels = barrelsParam;
        emitters = emittersParam;
        shell = shellParam;
        shotsBeforeCooloff = shotsBeforeCooloffParam;
        cooloff = cooloffParam;
        fireRate = fireRateParam;
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
        //Transform thisEmitter = barrels[currentBarrel].GetComponentInChildren<Transform>();
        Shell currentShell = Instantiate(shell, emitters[currentBarrel].position, emitters[currentBarrel].rotation);
        currentShell.ApplyForce(launchVelocity);
        Destroy(currentShell.gameObject, 10f);
        shotsFired++;
        fireRateTime = fireRate;

        //rigidbody.AddExplosionForce(explosionForce, explosionPoint.position, 100f, explosionLift);
        currentBarrel++;
        if (currentBarrel > emitters.Length - 1)
        {
            currentBarrel = 0;
        }
        
        return currentShell;
    }

    public bool CheckBarrelClearance(float sensorLength)
    {
        Transform emitter = emitters[currentBarrel].GetComponentInChildren<Transform>();
        Ray ray = new Ray(emitter.position, emitter.forward);
        if (Physics.Raycast(ray, sensorLength))
        {
            return false;
        }
        return true;
    }

    public bool CheckIfReadyToFire()
    {
        if (shotsFired >= shotsBeforeCooloff)
        {
            currentCooloff += Time.deltaTime;
            if (currentCooloff >= cooloff)
            {
                currentCooloff = 0;
                shotsFired = 0;
            }
        }
        if (fireRateTime <= 0 && currentCooloff == 0)
        {
            return true;
        }
        else
        {
            fireRateTime -= Time.deltaTime;
        }
        return false;
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
