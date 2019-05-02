using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    [SerializeField] float health = 100f;
    [SerializeField] GameObject destroyedWall;
    [SerializeField] GameObject debris;
    [SerializeField] Transform debrisPoint;
    [SerializeField] float debrisSize = 1f;
    float explodeForce;
    float explodeRadius;
    Vector3 explodePoint;

    private void Start()
    {
        if (debrisPoint == null)
        {
            debrisPoint = transform;
        }
    }

    public void DecreaseHealth(float damage)
    {
        health -= (100f * damage);
        if (health <= 0)
        {
            Instantiate(destroyedWall, transform.position, transform.rotation);
            GameObject currentDebris = Instantiate(debris, debrisPoint.position, debrisPoint.rotation);
            currentDebris.transform.localScale = new Vector3(debrisSize, debrisSize, debrisSize);

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
