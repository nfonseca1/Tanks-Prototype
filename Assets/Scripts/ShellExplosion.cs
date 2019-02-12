using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShellExplosion : MonoBehaviour
{

    public void Explode(float explosionForce, float explosionRadius)
    {
        Collider[] overlapColliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (var collider in overlapColliders)
        {
            Rigidbody colliderRB = collider.GetComponent<Rigidbody>();
            if (colliderRB != null)
            {
                colliderRB.AddExplosionForce(explosionForce, transform.position, explosionRadius, 0.5f);
            }
        }
    }
}
