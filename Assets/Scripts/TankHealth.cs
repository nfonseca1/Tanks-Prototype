using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankHealth : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void DecreaseHealth(float damage)
    {
        print(damage);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.GetComponent<Shell>() != null)
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        print("dead");
    }
}
