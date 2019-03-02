using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crate : MonoBehaviour
{
    [SerializeField] Transform particles;
    [SerializeField] Transform brokenCrate;
    [SerializeField] Transform[] powerups;

    private void OnCollisionEnter(Collision collision)
    {

        if (collision.collider.gameObject.GetComponent<Tank>() != null)
        {
            Vector3 particlePosition = new Vector3(transform.position.x, transform.position.y + 1.5f, transform.position.z);
            Transform currentParticles = Instantiate(particles, particlePosition, transform.rotation);
            //Transform currentBrokenCrate = Instantiate(brokenCrate, transform.position, transform.rotation);

            Destroy(currentParticles.gameObject, .5f);

            int random = Mathf.RoundToInt(Random.Range(0, powerups.Length));
            Instantiate(powerups[random], transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
