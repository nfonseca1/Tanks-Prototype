using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnedPlayer : MonoBehaviour
{
    [SerializeField] Transform parachute;
    Rigidbody rigidbody;
    PlayerController playerController;
    float fallSpeed = -10f;
    bool meshVisibilityChecked = false;
    bool isGrounded = false;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.useGravity = false;
        playerController = GetComponent<PlayerController>();
        playerController.grounded = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isGrounded)
        {
            rigidbody.MovePosition(transform.position + new Vector3(0, fallSpeed * Time.deltaTime, 0));
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(parachute.gameObject, .1f);
        rigidbody.useGravity = true;
        Destroy(this);
        playerController.grounded = true;
    }
}
