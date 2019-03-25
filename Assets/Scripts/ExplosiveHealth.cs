using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveHealth : MonoBehaviour
{
    [SerializeField] float health = 20f;
    [SerializeField] ShellExplosion explosion;
    [SerializeField] float explosionForce = 10f;
    [SerializeField] float explosionRadius = 7f;


    public void DecreaseHealth(float damage)
    {
        print(gameObject.name + " Damage : " + damage);
        health -= (100f * damage);
        if (health <= 0)
        {
            BlowUp();
        }
    }

    private void BlowUp()
    {
        ShellExplosion currentExplosion = Instantiate(explosion, transform.position, Quaternion.identity);
        currentExplosion.Explode(explosionForce, explosionRadius);
        Destroy(currentExplosion.gameObject, 3f);
        Destroy(gameObject);
    }
}
