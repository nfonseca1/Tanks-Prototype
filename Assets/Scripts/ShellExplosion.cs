using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShellExplosion : MonoBehaviour
{
    [SerializeField] LayerMask layerMask;
    List<string> objectsHit = new List<string>();

    public void Explode(float explosionForce, float explosionRadius)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        for (var i = 0; i < colliders.Length; i++)
        {
            if (objectsHit.Contains(colliders[i].gameObject.name)) { continue; }
            objectsHit.Add(colliders[i].gameObject.name);
            PlayerHealth playerHealth = colliders[i].GetComponent<PlayerHealth>();
            TankHealth tankHealth = colliders[i].GetComponent<TankHealth>();
            DestructableObjectHealth desObjHealth = colliders[i].GetComponent<DestructableObjectHealth>();
            ExplosiveHealth expHealth = colliders[i].GetComponent<ExplosiveHealth>();

            if (playerHealth != null || tankHealth != null || desObjHealth != null || expHealth != null)
            {
                RaycastHit hitInfo;
                if(Physics.Raycast(
                    transform.position,
                    (colliders[i].transform.position - transform.position),
                    out hitInfo,
                    (colliders[i].transform.position - transform.position).magnitude,
                    layerMask
                    ))
                {
                    if (hitInfo.collider.GetComponent<TankHealth>() != null)
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
                    else if (hitInfo.collider.GetComponent<DestructableObjectHealth>() != null)
                    {
                        float damage = (explosionRadius - hitInfo.distance) / explosionRadius;
                        if (damage > 0.9f) { damage = 1f; }
                        desObjHealth.DecreaseHealth(damage);
                    }
                    else if (hitInfo.collider.GetComponent<ExplosiveHealth>() != null)
                    {
                        float damage = (explosionRadius - hitInfo.distance) / explosionRadius;
                        if (damage > 0.9f) { damage = 1f; }
                        expHealth.DecreaseHealth(damage);
                    }
                }
            }

            Rigidbody colliderRB = colliders[i].GetComponent<Rigidbody>();
            if (colliderRB != null)
            {
                colliderRB.AddExplosionForce(explosionForce, transform.position, explosionRadius, 0.5f);
            }
        }
    }
}
