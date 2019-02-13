using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : MonoBehaviour
{
    [SerializeField] ShellExplosion explosion;
    [SerializeField] Transform explosionPoint;
    [SerializeField] float explosionForce = 10f;
    [SerializeField] float explosionRadius = 5f;

    Rigidbody rigidbody;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        transform.LookAt(transform.position + rigidbody.velocity);
    }

    public void ApplyForce(float velocity)
    {
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.velocity = transform.forward * velocity;

    }

    private void OnCollisionEnter(Collision collision)
    {
        ShellExplosion thisExplosion = Instantiate(explosion, explosionPoint.position, explosionPoint.rotation);
        Destroy(thisExplosion, 5f);
        thisExplosion.Explode(explosionForce, explosionRadius);
        Destroy(gameObject);
    }
}
