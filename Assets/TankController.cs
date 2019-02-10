using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TankController : MonoBehaviour
{
    [SerializeField] float speed = 25f;
    [SerializeField] float torque = 45f;
    [SerializeField] float launchVelocity = 25f;
    [SerializeField] float elevateSpeed = 75f;

    [SerializeField] float explosionForce = 200f;
    [SerializeField] float explosionLift = 1f;

    [SerializeField] Transform turret;
    [SerializeField] Transform barrelWheel;
    [SerializeField] Transform barrel;
    [SerializeField] Transform emitter;
    [SerializeField] Shell shell;
    [SerializeField] Transform explosionPoint;

    Rigidbody rigidbody;
    Vector3 hitPoint;
    float acceleration = 0;
    bool barrelUpdated = true;

    Vector3 barrelScale;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = gameObject.GetComponent<Rigidbody>();

        barrelWheel.localEulerAngles = new Vector3(0, 0, 0);
        barrelScale = barrel.localScale;
    }

    private void Update()
    {
        if (!barrelUpdated)
        {
            UpdateBarrel();
        }
        else
        {
            if (Input.GetMouseButtonUp(0))
            {
                Fire();
                UpdateBarrel();
            }
            if (Input.GetMouseButton(0))
            {
                ElevateBarrel();
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Move();
        bool aimResult = CalculateAimTarget();
        Aim(aimResult);
        
        
    }

    private void Move()
    {
        float axisX = Input.GetAxis("Horizontal");
        float axisY = Input.GetAxis("Vertical");

        Vector3 newPosition = transform.position + (transform.forward * speed * axisY * Time.deltaTime);
        Vector3 newPositionXZ = new Vector3(newPosition.x, transform.position.y, newPosition.z);
        rigidbody.MovePosition(newPositionXZ);

        Quaternion turn = Quaternion.Euler(new Vector3(
            transform.localEulerAngles.x,
            transform.localEulerAngles.y + torque * axisX * Time.deltaTime,
            transform.localEulerAngles.z));
        rigidbody.MoveRotation(turn);
    }

    private bool CalculateAimTarget()
    {
        Camera mainCamera = Camera.main;
        Vector3 mousePosition = Input.mousePosition;

        Vector3 aimPoint = mainCamera.ScreenToWorldPoint(new Vector3(
            mousePosition.x,
            mousePosition.y,
            5f
            ));

        Vector3 aimDirection = aimPoint - mainCamera.gameObject.transform.position;
        Ray aimRay = new Ray(aimPoint, aimDirection);
        RaycastHit hitInfo;
        if(Physics.Raycast(aimRay, out hitInfo))
        {
            hitPoint = hitInfo.point;
            return true;
        }
        else
        {
            return false;
        }
    }

    private void Aim(bool aimResult)
    {
        if (aimResult)
        {
            Vector3 turretLookPoint = new Vector3(hitPoint.x, turret.position.y, hitPoint.z);
            Quaternion targetRotation = Quaternion.LookRotation(turretLookPoint - turret.position, turret.up);
            turret.rotation = Quaternion.Lerp(turret.rotation, targetRotation, .3f);
            turret.localEulerAngles = new Vector3(0, turret.localEulerAngles.y, 0);
        }
    }

    private void ElevateBarrel()
    {
        barrelWheel.Rotate(new Vector3(-elevateSpeed * Time.deltaTime, 0, 0), Space.Self);
        if(barrelWheel.localEulerAngles.x <= 310f)
        {
            barrelWheel.localEulerAngles = new Vector3(310f, 0, 0);
        }
    }

    private void Fire()
    {
        Shell currentShell = Instantiate(shell, emitter.position, emitter.rotation);
        currentShell.ApplyForce(launchVelocity);
        Destroy(currentShell, 10f);

        rigidbody.AddExplosionForce(explosionForce, explosionPoint.position, 100f, explosionLift);
        barrel.localScale = new Vector3(barrel.localScale.x, barrel.localScale.y, barrel.localScale.z * 0.7f);
    }

    private void UpdateBarrel()
    {
        barrelUpdated = false;
        barrel.localScale = Vector3.Lerp(barrel.localScale, barrelScale, 0.2f);

        barrelWheel.Rotate(new Vector3(elevateSpeed * Time.deltaTime, 0, 0), Space.Self);
        if(barrelWheel.localEulerAngles.x < 90)
        {
            barrelWheel.localEulerAngles = new Vector3(0, 0, 0);

            if(barrel.localScale.z >= 0.95f)
            {
                barrel.localScale = barrelScale;
                barrelUpdated = true;
            }
        }
    }
}
