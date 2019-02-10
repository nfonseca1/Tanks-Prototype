using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : MonoBehaviour
{
    [SerializeField] ParticleSystem explosion;
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
        rigidbody.AddRelativeForce(new Vector3(0, 0, velocity));

    }

    private void OnCollisionEnter(Collision collision)
    {
        ParticleSystem thisExplosion = Instantiate(explosion, transform.position, transform.rotation);
        Destroy(thisExplosion, 5f);
        Destroy(gameObject);
    }
}
