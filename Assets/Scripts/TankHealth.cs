using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankHealth : MonoBehaviour
{
    [SerializeField] float health = 100f;
    [SerializeField] Transform explosion;
    

    public void DecreaseHealth(float damage)
    {
        health -= (100f * damage);
        if (health <= 0)
        {
            BlowUp();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.GetComponent<Shell>() != null)
        {
            BlowUp();
        }
        else if(collision.gameObject.GetComponent<Missile>() != null)
        {
            BlowUp();
        }
        else if(collision.gameObject.GetComponent<Bullet>() != null)
        {
            DecreaseHealth(.3f);
        }
    }

    private void BlowUp()
    {
        Transform currentExplosion = Instantiate(explosion, transform.position, Quaternion.identity);
        Destroy(currentExplosion.gameObject, 3f);
        Destroy(gameObject);
    }
}
