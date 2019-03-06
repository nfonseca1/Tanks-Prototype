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
    [SerializeField] protected Transform barrel2;
    [SerializeField] protected Transform emitter;
    [SerializeField] protected Transform emitter2;
    [SerializeField] protected Shell shell;
    [SerializeField] protected Transform explosionPoint;

    protected Rigidbody rigidbody;
    protected Vector3 barrelScale = new Vector3(1, 1, 1);
    protected Vector3 defaultBarrelRot = new Vector3(0, 180, 0);
    protected float maxBarrelHeight = 45f;
    bool barrel2IsNext = false;

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
        Transform thisBarrel = barrel;
        Transform thisEmitter = emitter;
        if (barrel2 != null)
        {
            if (barrel2IsNext)
            {
                thisBarrel = barrel2;
                thisEmitter = emitter2;
            }
        }
        Shell currentShell = Instantiate(shell, thisEmitter.position, thisEmitter.rotation);
        currentShell.ApplyForce(launchVelocity);
        Destroy(currentShell.gameObject, 10f);

        rigidbody.AddExplosionForce(explosionForce, explosionPoint.position, 100f, explosionLift);
        if (barrel != null)
        {
            thisBarrel.localScale = new Vector3(thisBarrel.localScale.x, thisBarrel.localScale.y, thisBarrel.localScale.z * 0.7f);
            barrel2IsNext = !barrel2IsNext;
        }
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
        Transform thisBarrel = barrel;
        if (barrel == null) { return true; }
        if (barrel2 != null)
        {
            if (barrel2IsNext) { thisBarrel = barrel2; }
        }

        thisBarrel.localScale = Vector3.Lerp(thisBarrel.localScale, barrelScale, 0.2f);

        barrelWheel.Rotate(new Vector3(-elevateSpeed * Time.deltaTime, 0, 0), Space.Self);
        if (barrelWheel.localEulerAngles.x > 180)
        {
            barrelWheel.localEulerAngles = defaultBarrelRot;

            if (thisBarrel.localScale.z >= 0.95f)
            {
                thisBarrel.localScale = barrelScale;
                return true;
            }
        }
        return false;
    }
}
