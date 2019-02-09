using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TankController : MonoBehaviour
{
    [SerializeField] float speed = 8000f;
    [SerializeField] float torque = 4000f;
    [SerializeField] float launchVelocity = 25f;
    [SerializeField] float elevateSpeed = 75f;

    [SerializeField] Transform turret;
    [SerializeField] Transform barrelWheel;
    [SerializeField] Transform barrel;
    [SerializeField] Transform emitter;
    [SerializeField] Shell shell;

    Rigidbody rigidbody;
    Vector3 hitPoint;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = gameObject.GetComponent<Rigidbody>();
        barrelWheel.localEulerAngles = new Vector3(0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        bool aimResult = CalculateAimTarget();
        Aim(aimResult);

        if (Input.GetMouseButtonUp(0))
        {
            Fire();
        }
        if (Input.GetMouseButton(0))
        {
            ElevateBarrel();
        }
    }

    private void Move()
    {
        if (Input.GetKey(KeyCode.W))
        {
            rigidbody.AddRelativeForce(new Vector3(0, 0, speed * Time.deltaTime));
        }
        if (Input.GetKey(KeyCode.S))
        {
            rigidbody.AddRelativeForce(new Vector3(0, 0, -speed * 0.66f * Time.deltaTime));
        }
        if (Input.GetKey(KeyCode.A))
        {
            rigidbody.AddRelativeTorque(new Vector3(0, -torque * Time.deltaTime, 0));
        }
        if (Input.GetKey(KeyCode.D))
        {
            rigidbody.AddRelativeTorque(new Vector3(0, torque * Time.deltaTime, 0));
        }
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
            Vector3 turretLookPoint = new Vector3(hitPoint.x, turret.transform.position.y, hitPoint.z);
            turret.LookAt(turretLookPoint);
        }
    }

    private void ElevateBarrel()
    {
        barrelWheel.Rotate(new Vector3(-elevateSpeed * Time.deltaTime, 0, 0), Space.Self);
        float clampedRotation = Mathf.Clamp(barrelWheel.localEulerAngles.x - 360, -50f, 0);
        barrelWheel.localEulerAngles = new Vector3(clampedRotation, 0, 0);
    }

    private void Fire()
    {
        Shell currentShell = Instantiate(shell, emitter.position, emitter.rotation);
        currentShell.ApplyForce(launchVelocity);
        Destroy(currentShell, 10f);

        barrelWheel.localEulerAngles = new Vector3(0, 0, 0);
    }
}
