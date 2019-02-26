using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    [SerializeField] ShellExplosion explosion;
    Rigidbody rigidbody;
    [SerializeField] float explosionForce = 10f;
    [SerializeField] float explosionRadius = 5f;
    float speed = 12.5f;
    float time = 0f;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.velocity = transform.forward * speed;
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;

        if (time > 0.1f)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(1.5f, 1.5f, 1.5f), 0.5f);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        ParticleSystem particles = GetComponentInChildren<ParticleSystem>();
        particles.transform.parent = null;
        Destroy(particles.gameObject, 1f);

        ShellExplosion currentExplosion = Instantiate(explosion, transform.position, Quaternion.identity);
        Destroy(currentExplosion.gameObject, 3f);
        currentExplosion.Explode(explosionForce, explosionRadius);
        Destroy(gameObject);
    }
}
