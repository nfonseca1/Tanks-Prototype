using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    [SerializeField] float health = 100f;
    [SerializeField] GameObject destroyedWall;
    [SerializeField] GameObject debris;
    float explodeForce;
    float explodeRadius;
    Vector3 explodePoint;

    public void DecreaseHealth(float damage)
    {
        health -= (100f * damage);
        if (health <= 0)
        {
            Instantiate(destroyedWall, this.transform.position, this.transform.rotation);
            GameObject currentDebris = Instantiate(debris, transform.position, transform.rotation);

            GetComponent<BoxCollider>().enabled = false;

            foreach(Rigidbody rb in currentDebris.GetComponentsInChildren<Rigidbody>())
            {
                rb.AddExplosionForce(explodeForce, explodePoint, explodeRadius);
                rb.useGravity = true;
            }

            Destroy(gameObject);
        }
    }

    public void SetExplodeInfo(float force, Vector3 position, float radius)
    {
        explodeForce = force;
        explodePoint = position;
        explodeRadius = radius;
    }
}
