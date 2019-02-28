using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnedEnemy : MonoBehaviour
{
    [SerializeField] Transform parachute;
    CapsuleCollider collider;
    Rigidbody rigidbody;
    float fallSpeed = -10f;

    private void Start()
    {
        collider = parachute.GetComponent<CapsuleCollider>();
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.useGravity = false;
    }

    // Update is called once per frame
    void Update()
    {
        rigidbody.MovePosition(transform.position + new Vector3(0, fallSpeed * Time.deltaTime, 0));
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == 11)
        {
            
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(parachute.gameObject, .1f);
        rigidbody.useGravity = true;
        Destroy(this);
    }
}
