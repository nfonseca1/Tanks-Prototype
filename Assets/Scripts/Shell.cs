using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : MonoBehaviour
{
    [SerializeField] ShellExplosion explosion;
    [SerializeField] Transform explosionPoint;
    [SerializeField] float explosionForce = 10f;
    [SerializeField] float explosionRadius = 5f;

    new Rigidbody rigidbody;
    AIBasicTank ai;

    float totalFlightTime = 0f;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        transform.LookAt(transform.position + rigidbody.velocity);

        totalFlightTime += Time.deltaTime;
    }

    public void ApplyForce(float velocity)
    {
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.velocity = transform.forward * velocity;
    }

    public void SetAITankSource(AIBasicTank controller)
    {
        ai = controller;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(ai != null)
        {
            ai.SetProjectileTime(totalFlightTime);
        }
        ShellExplosion thisExplosion = Instantiate(explosion, explosionPoint.position, explosionPoint.rotation);
        Destroy(thisExplosion.gameObject, 5f);
        if (ai != null) { thisExplosion.SetExplosionFromAI(); }
        thisExplosion.Explode(explosionForce, explosionRadius);
        Destroy(gameObject);
    }
}
