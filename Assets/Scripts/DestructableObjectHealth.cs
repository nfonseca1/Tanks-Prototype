using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructableObjectHealth : MonoBehaviour
{
    [SerializeField] float health = 100f;

    public void DecreaseHealth(float damage)
    {
        health -= (100f * damage);
        if (health <= 0)
        {
            GetComponent<DestructableObject>().ActivatePhysics();
        }
    }
}
