using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public void ActivatePhysics()
    {
        Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>();

        foreach(var rb in rigidbodies)
        {
            rb.isKinematic = false;
        }
    }
}
