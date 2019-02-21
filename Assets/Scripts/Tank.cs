using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tank : MonoBehaviour
{
    [SerializeField] protected float speed = 25f;
    [SerializeField] protected float torque = 45f;
    [SerializeField] protected float launchVelocity = 25f;
    [SerializeField] protected float elevateSpeed = 60f;

    [SerializeField] protected float explosionForce = 200f;
    [SerializeField] protected float explosionLift = 1f;

    [SerializeField] protected Transform turret;
    [SerializeField] protected Transform barrelWheel;
    [SerializeField] protected Transform barrel;
    [SerializeField] protected Transform emitter;
    [SerializeField] protected Shell shell;
    [SerializeField] protected Transform explosionPoint;

    protected Rigidbody rigidbody;
    protected Vector3 barrelScale = new Vector3(1, 1, 1);
    protected Vector3 defaultBarrelRot = new Vector3(0, 180, 0);
    protected float maxBarrelHeight = 45f;

    public void Rotate(float input)
    {
        Quaternion turn = Quaternion.Euler(new Vector3(
            transform.localEulerAngles.x,
            transform.localEulerAngles.y + torque * input * Time.deltaTime,
            transform.localEulerAngles.z));
        rigidbody.MoveRotation(turn);
    }

    public Shell Fire()
    {
        Shell currentShell = Instantiate(shell, emitter.position, emitter.rotation);
        currentShell.ApplyForce(launchVelocity);
        Destroy(currentShell.gameObject, 10f);

        rigidbody.AddExplosionForce(explosionForce, explosionPoint.position, 100f, explosionLift);
        barrel.localScale = new Vector3(barrel.localScale.x, barrel.localScale.y, barrel.localScale.z * 0.7f);

        return currentShell;
    }

    public void Aim(Vector3 hitPoint)
    {
        Vector3 turretLookPoint = new Vector3(hitPoint.x, turret.position.y, hitPoint.z);
        Quaternion targetRotation = Quaternion.LookRotation(turretLookPoint - turret.position, turret.up);
        turret.rotation = Quaternion.Lerp(turret.rotation, targetRotation, .3f);
        turret.localEulerAngles = new Vector3(0, turret.localEulerAngles.y, 0);
    }

    public bool UpdateBarrel()
    {
        barrel.localScale = Vector3.Lerp(barrel.localScale, barrelScale, 0.2f);

        barrelWheel.Rotate(new Vector3(-elevateSpeed * Time.deltaTime, 0, 0), Space.Self);
        if (barrelWheel.localEulerAngles.x > 180)
        {
            barrelWheel.localEulerAngles = defaultBarrelRot;

            if (barrel.localScale.z >= 0.95f)
            {
                barrel.localScale = barrelScale;
                return true;
            }
        }
        return false;
    }
}
