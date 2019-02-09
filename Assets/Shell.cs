using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : MonoBehaviour
{
    Rigidbody rigidbody;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }
    
    public void ApplyForce(float velocity)
    {
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.velocity = transform.forward * velocity;
    }
}
