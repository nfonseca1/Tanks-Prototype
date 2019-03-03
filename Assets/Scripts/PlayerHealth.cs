using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] float health = 100f;
    [SerializeField] Transform explosion;
    Slider healthBar;


    private void Awake()
    {
        healthBar = GetComponentInChildren<Slider>();
    }

    public void DecreaseHealth(float damage)
    {
        health -= (100f * damage);
        if (health <= 0)
        {
            BlowUp();
        }
    }

    public void IncreaseHealth(float increase)
    {
        health += (100f * increase);
        if (health >= 100)
        {
            health = 100;
        }
    }

    private void Update()
    {
        healthBar.value = health / 100;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<Shell>() != null)
        {
            DecreaseHealth(.85f);
        }
        else if (collision.gameObject.GetComponent<Missile>() != null)
        {
            DecreaseHealth(.85f);
        }
        else if (collision.gameObject.GetComponent<Bullet>() != null)
        {
            DecreaseHealth(.3f);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Health")
        {
            if (Input.GetAxis("Jump") == 1)
            {
                Destroy(other.gameObject);
                IncreaseHealth(0.3f);
            }
        }
    }

    private void BlowUp()
    {
        print("blow up");
        FindObjectOfType<LevelManager>().Respawn(transform.position);

        Transform currentExplosion = Instantiate(explosion, transform.position, Quaternion.identity);
        Destroy(currentExplosion.gameObject, 3f);
        Destroy(gameObject);
    }
}
