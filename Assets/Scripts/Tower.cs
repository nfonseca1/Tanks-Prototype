using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public void activatePhysics()
    {
        Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>();

        foreach(var rb in rigidbodies)
        {
            rb.isKinematic = false;
        }
    }
}
