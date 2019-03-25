using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TNT : MonoBehaviour
{
    [SerializeField] ShellExplosion explosion;
    [SerializeField] float explosionForce = 10f;
    [SerializeField] float explosionRadius = 7f;
    Rigidbody rigidbody;
    float forceToExplode = 1.5f;
    float velocity = 0;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        velocity = rigidbody.velocity.magnitude;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();

        if (collision.gameObject.GetComponent<Rigidbody>() != null)
        {
            if (rb.velocity.magnitude + velocity > forceToExplode)
            {
                ShellExplosion currentExplosion = Instantiate(explosion, transform.position, transform.rotation);
                currentExplosion.Explode(explosionForce, explosionRadius);
                Destroy(currentExplosion.gameObject, 5f);
                Destroy(gameObject);
            }
        }
    }
}
