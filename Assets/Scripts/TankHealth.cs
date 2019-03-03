using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TankHealth : MonoBehaviour
{
    [SerializeField] float health = 100f;
    [SerializeField] Transform explosion;
    [SerializeField] Slider healthBar;
    

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
    }

    private void BlowUp()
    {
        Transform currentExplosion = Instantiate(explosion, transform.position, Quaternion.identity);
        Destroy(currentExplosion.gameObject, 3f);
        Destroy(gameObject);
    }
}
