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
    AIController AIController;

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

    public void SetAITankSource(AIController controller)
    {
        AIController = controller;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(AIController != null)
        {
            AIController.SetProjectileTime(totalFlightTime);
        }
        
        ShellExplosion thisExplosion = Instantiate(explosion, explosionPoint.position, explosionPoint.rotation);
        Destroy(thisExplosion, 5f);
        thisExplosion.Explode(explosionForce, explosionRadius);
        Destroy(gameObject);
    }
}
