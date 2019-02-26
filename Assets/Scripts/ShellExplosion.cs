using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShellExplosion : MonoBehaviour
{
    int layerMask = 1 << 9;

    public void Explode(float explosionForce, float explosionRadius)
    {
        Collider[] overlapColliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (var collider in overlapColliders)
        {
            TankHealth tankHealth = collider.GetComponent<TankHealth>();
            if (tankHealth != null)
            {
                layerMask = ~layerMask;
                RaycastHit hitInfo;
                if(Physics.Raycast(
                    transform.position,
                    (collider.transform.position - transform.position),
                    out hitInfo,
                    (collider.transform.position - transform.position).magnitude,
                    layerMask
                    ))
                {
                    if(hitInfo.collider.GetComponent<TankHealth>() == tankHealth)
                    {
                        float damage = (explosionRadius - hitInfo.distance) / explosionRadius;
                        if(damage > 0.9f) { damage = 1f; }
                        tankHealth.DecreaseHealth(damage);
                    }
                }
            }

            Tower tb = collider.GetComponent<Tower>();

            if (tb != null)
            {
                tb.activatePhysics();
            }

            Rigidbody colliderRB = collider.GetComponent<Rigidbody>();
            if (colliderRB != null)
            {
                print(colliderRB.name);
                colliderRB.AddExplosionForce(explosionForce, transform.position, explosionRadius, 0.5f);
            }

            if (tb != null)
            {
                Destroy(tb.GetComponent<BoxCollider>(), 0.1f);
            }
        }
    }
}
