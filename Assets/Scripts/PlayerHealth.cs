using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] float health = 100f;
    [SerializeField] Transform explosion;
    Slider healthBar;

    Powerup currentPowerup;
    bool onPowerup = false;
    bool actionPressed = false;

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
        actionPressed = false;
        onPowerup = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<Bullet>() != null)
        {
            DecreaseHealth(.3f);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.GetComponent<Powerup>() != null)
        {
            if (currentPowerup == null)
            {
                currentPowerup = other.GetComponent<Powerup>();
                currentPowerup.TurnOnButtonPrompt();
            }
            if (Input.GetButtonDown("Jump") && !actionPressed)
            {
                if (other.gameObject.tag == "Health") { IncreaseHealth(0.3f); }
                Destroy(currentPowerup.gameObject);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<Powerup>() != null)
        {
            currentPowerup = null;
        }
    }

    private void BlowUp()
    {
        FindObjectOfType<LevelManager>().Respawn(transform.position);

        Transform currentExplosion = Instantiate(explosion, transform.position, Quaternion.identity);
        Destroy(currentExplosion.gameObject, 3f);
        Destroy(gameObject);
    }

}
