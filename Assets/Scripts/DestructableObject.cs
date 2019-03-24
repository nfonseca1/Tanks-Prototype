using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructableObject : MonoBehaviour
{
    public void ActivatePhysics()
    {
        Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>();
        Collider[] colliders = GetComponentsInChildren<Collider>();

        foreach (var collider in colliders)
        {
            collider.enabled = true;
        }
        foreach (var rb in rigidbodies)
        {
            rb.isKinematic = false;
        }

        Destroy(GetComponent<Collider>(), 0.1f);
    }
}
