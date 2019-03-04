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
            PlayerHealth playerHealth = collider.GetComponent<PlayerHealth>();
            TankHealth tankHealth = collider.GetComponent<TankHealth>();
            if (playerHealth != null || tankHealth != null)
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
                    if(hitInfo.collider.GetComponent<TankHealth>() != null)
                    {
                        float damage = (explosionRadius - hitInfo.distance) / explosionRadius;
                        if(damage > 0.9f) { damage = 1f; }
                        tankHealth.DecreaseHealth(damage);
                    }
                    else if (hitInfo.collider.GetComponent<PlayerHealth>() != null)
                    {
                        float damage = (explosionRadius - hitInfo.distance) / explosionRadius;
                        if (damage > 0.9f) { damage = 0.85f; }
                        playerHealth.DecreaseHealth(damage);
                    }
                }
            }

            Tower tb = collider.GetComponent<Tower>();
            Door door = collider.GetComponent<Door>();

            if (tb != null)
            {
                tb.ActivatePhysics();
            }
            else if (door != null)
            {
                door.ActivatePhysics();
            }

            Rigidbody colliderRB = collider.GetComponent<Rigidbody>();
            if (colliderRB != null)
            {
                colliderRB.AddExplosionForce(explosionForce, transform.position, explosionRadius, 0.5f);
            }

            if (tb != null)
            {
                Destroy(tb.GetComponent<BoxCollider>(), 0.1f);
            }
            else if (door != null)
            {
                Destroy(door.GetComponent<BoxCollider>(), 0.1f);
            }
        }
    }
}
