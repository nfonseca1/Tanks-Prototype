using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour
{
    PlayerController player = null;
    bool canBePickedUp = true;

    // Update is called once per frame
    void Update()
    {
        if (player)
        {
            Transform cargoPoint = player.GetCargoTransform();

            if (cargoPoint && canBePickedUp)
            {
                transform.position = cargoPoint.position;
                transform.rotation = cargoPoint.rotation;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>() && canBePickedUp)
        {
            player = collision.gameObject.GetComponent<PlayerController>();
            GetComponent<Collider>().isTrigger = true;
            GetComponent<Rigidbody>().isKinematic = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Resource Dropoff")
        {
            player.ClearCargoSlot();
            canBePickedUp = false;
            player = null;
            GetComponent<Collider>().isTrigger = false;
            GetComponent<Rigidbody>().isKinematic = false;
        }
    }
}
