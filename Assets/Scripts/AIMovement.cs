using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIMovement : MonoBehaviour
{
    Transform sensorPointF, sensorPointFL, sensorPointFR, sensorPointL, sensorPointR;
    new Rigidbody rigidbody;
    float speed = 10;
    float torque = 180;

    public enum Sensor { Front, FrontRight, FrontLeft, Left, Right, None }

    public AIMovement(ref Rigidbody rigidbodyParam, float speedParam, float torqueParam, Transform[] sensors)
    {
        rigidbody = rigidbodyParam;
        speed = speedParam;
        torque = torqueParam;

        sensorPointF = sensors[0];
        sensorPointFL = sensors[1];
        sensorPointFR = sensors[2];
        sensorPointL = sensors[3];
        sensorPointR = sensors[4];
    }

    public void Move(float input, Transform currentTransform)
    {
        Vector3 newPosition = currentTransform.position + (currentTransform.forward * speed * input * Time.deltaTime);
        Vector3 newPositionXZ = new Vector3(newPosition.x, currentTransform.position.y, newPosition.z);
        rigidbody.MovePosition(newPositionXZ);
    }

    public void Rotate(float input, Transform currentTransform)
    {
        Quaternion turn = Quaternion.Euler(new Vector3(
            currentTransform.localEulerAngles.x,
            currentTransform.localEulerAngles.y + torque * input * Time.deltaTime,
            currentTransform.localEulerAngles.z));
        rigidbody.MoveRotation(turn);
    }

    public Sensor CheckSensors(float sensorLength)
    {
        Ray frontRay = new Ray(sensorPointF.position, sensorPointF.forward);
        Ray frontRightRay = new Ray(sensorPointFR.position, sensorPointFR.forward);
        Ray frontLeftRay = new Ray(sensorPointFL.position, sensorPointFL.forward);
        Ray leftRay = new Ray(sensorPointL.position, sensorPointL.forward);
        Ray rightRay = new Ray(sensorPointR.position, sensorPointR.forward);


        RaycastHit hitInfo;
        if (Physics.Raycast(frontRay, out hitInfo, sensorLength))
        {
            return Sensor.Front;
        }
        if (Physics.Raycast(frontRightRay, out hitInfo, sensorLength))
        {
            return Sensor.FrontRight;
        }
        if (Physics.Raycast(frontLeftRay, out hitInfo, sensorLength))
        {
            return Sensor.FrontLeft;
        }
        if (Physics.Raycast(leftRay, out hitInfo, sensorLength))
        {
            return Sensor.Left;
        }
        if (Physics.Raycast(rightRay, out hitInfo, sensorLength))
        {
            return Sensor.Right;
        }
        return Sensor.None;
    }

    public void ManageAxisInput(float input, ref float lerp, float lerpLimit)
    {
        if (input > 0) // If we are pressing the axis input, add or subtract from the lerp
        {
            lerp += Time.deltaTime;
            if (lerp > lerpLimit) { lerp = lerpLimit; }
        }
        else if (input < 0)
        {
            lerp -= Time.deltaTime;
            if (lerp < -lerpLimit) { lerp = -lerpLimit; }
        }
        else // Otherwise add or subtract the lerp back to zero
        {
            if (lerp < 0)
            {
                lerp += Time.deltaTime;
                if (lerp >= 0) { lerp = 0; }
            }
            else if (lerp > 0)
            {
                lerp -= Time.deltaTime;
                if (lerp <= 0) { lerp = 0; }
            }
        }
    }
}
