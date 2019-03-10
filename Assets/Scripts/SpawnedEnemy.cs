using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnedEnemy : MonoBehaviour
{
    [SerializeField] Transform parachute;
    [SerializeField] MeshRenderer[] renderingTargets;
    CapsuleCollider collider;
    Rigidbody rigidbody;
    AIBasicTank aiBasicTank;
    MeshRenderer[] renderers;
    float fallSpeed = -10f;
    bool meshVisibilityChecked = false;
    bool isGrounded = false;

    private void Start()
    {
        collider = parachute.GetComponent<CapsuleCollider>();
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.useGravity = false;
        aiBasicTank = GetComponent<AIBasicTank>();
        aiBasicTank.SetGrounded(false);

        renderers = GetComponentsInChildren<MeshRenderer>();
        foreach (var renderer in renderers)
        {
            renderer.enabled = false;
        }
        foreach (var renderingTarget in renderingTargets)
        {
            renderingTarget.enabled = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!meshVisibilityChecked)
        {
            foreach (var renderingTarget in renderingTargets)
            {
                if (renderingTarget.isVisible)
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y + 100, transform.position.z);
                    break;
                }
            }
            TurnOnRenderers();
            meshVisibilityChecked = true;
        }
        if (!isGrounded)
        {
            rigidbody.MovePosition(transform.position + new Vector3(0, fallSpeed * Time.deltaTime, 0));
        }
    }

    private void TurnOnRenderers()
    {
        foreach (var renderer in renderers)
        {
            renderer.enabled = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(parachute.gameObject, .1f);
        rigidbody.useGravity = true;
        Destroy(this);
        aiBasicTank.SetGrounded(true);
    }
}
