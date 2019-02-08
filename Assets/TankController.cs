using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TankController : MonoBehaviour
{
    [SerializeField] float speed = 8000f;
    [SerializeField] float torque = 4000f;
    [SerializeField] float launchVelocity = 1000f;

    [SerializeField] Transform turret;
    [SerializeField] Transform barrelWheel;
    [SerializeField] Transform barrel;
    [SerializeField] Transform emitter;

    Rigidbody rigidbody;
    Vector3 hitPoint;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        bool aimResult = CalculateAimTarget();
        Aim(aimResult);
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

            float range = (new Vector3(hitPoint.x, 0, hitPoint.z) - new Vector3(emitter.position.x, 0, emitter.position.z)).magnitude;
            float height = hitPoint.y - emitter.position.y;

            float angle = GetTrajectoryAngle(range, height);
            print(angle);
            barrelWheel.eulerAngles = new Vector3(angle, barrelWheel.eulerAngles.y, barrelWheel.eulerAngles.z);
        }
    }

    private float GetTrajectoryAngle(float range, float height)
    {
        float gravity = Physics.gravity.y;
        float numerator = Mathf.Pow(launchVelocity, 2) - Mathf.Sqrt(
            Mathf.Pow(launchVelocity, 4) - gravity * (gravity * Mathf.Pow(range, 2) + 2 * height *
            Mathf.Pow(launchVelocity, 2)));

        float denominator = gravity * range;
        float result = Mathf.Atan(numerator / denominator);

        return result * 57.2958f; //Radians to degrees
    }
}
