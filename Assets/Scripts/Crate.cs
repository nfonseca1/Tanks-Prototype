using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crate : MonoBehaviour
{
    [SerializeField] Transform particles;
    [SerializeField] Transform brokenCrate;

    private void OnCollisionEnter(Collision collision)
    {

        if (collision.collider.gameObject.GetComponent<Tank>() != null)
        {
            Vector3 particlePosition = new Vector3(transform.position.x, transform.position.y + 1.5f, transform.position.z);
            Transform currentParticles = Instantiate(particles, particlePosition, transform.rotation);
            Transform currentBrokenCrate = Instantiate(brokenCrate, transform.position, transform.rotation);

            Destroy(currentParticles.gameObject, .5f);
            Destroy(gameObject);
        }
    }
}
