using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : Shell
{
    new Rigidbody rigidbody;
    [SerializeField] Transform explosion;
    

    new public void ApplyForce(float velocity)
    {
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.velocity = transform.forward * velocity;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject, 0.05f);
    }

    private void OnDestroy()
    {
        Transform currentExplosion = Instantiate(explosion, transform.position, transform.rotation);
        Destroy(currentExplosion.gameObject, 0.2f);
    }
}
