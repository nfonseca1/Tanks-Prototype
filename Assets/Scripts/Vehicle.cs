using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vehicle : MonoBehaviour
{
    Rigidbody rb;
    float torque = 180f;
    float speed = 20f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Move(float input, Transform currentTransform)
    {
        Vector3 newPosition = currentTransform.position + (currentTransform.forward * speed * input * Time.deltaTime);
        Vector3 newPositionXZ = new Vector3(newPosition.x, currentTransform.position.y, newPosition.z);
        rb.MovePosition(newPositionXZ);
    }

    public void Rotate(float input, Transform currentTransform)
    {
        Quaternion turn = Quaternion.Euler(new Vector3(
            currentTransform.localEulerAngles.x,
            currentTransform.localEulerAngles.y + torque * input * Time.deltaTime,
            currentTransform.localEulerAngles.z));
        rb.MoveRotation(turn);
    }
}
