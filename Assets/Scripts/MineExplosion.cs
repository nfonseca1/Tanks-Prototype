using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineExplosion : MonoBehaviour
{
    int layerMask = 10;
    List<string> objectsHit = new List<string>();

    public void Explode(float explosionForce, float explosionRadius)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        for (var i = 0; i < colliders.Length; i++)
        {
            if (objectsHit.Contains(colliders[i].gameObject.name)) { continue; }
            objectsHit.Add(colliders[i].gameObject.name);
            PlayerHealth playerHealth = colliders[i].GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                float damage = 0;

                RaycastHit hitInfo;
                if (Physics.Raycast(
                    transform.position,
                    (colliders[i].transform.position - transform.position),
                    out hitInfo,
                    (colliders[i].transform.position - transform.position).magnitude,
                    layerMask
                    ))
                {
                    if (hitInfo.collider.GetComponent<PlayerHealth>() != null)
                    {
                        damage = (explosionRadius - hitInfo.distance) / explosionRadius;
                    }
                }
                else
                {
                    float playerDistance = (colliders[i].gameObject.transform.position - transform.position).magnitude;
                    damage = ((explosionRadius - playerDistance) / explosionRadius);
                }

                if (damage > 0.9f) { damage = 0.85f; }
                playerHealth.DecreaseHealth(damage);
            }

            Rigidbody colliderRB = colliders[i].GetComponent<Rigidbody>();
            if (colliderRB != null)
            {
                colliderRB.AddExplosionForce(explosionForce, transform.position, explosionRadius, 0.5f);
            }
        }
    }
}
