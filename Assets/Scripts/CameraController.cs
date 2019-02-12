using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] [Range(1, 10)] float distance = 1f;

    // Update is called once per frame
    void Update()
    {
        transform.position = target.position + new Vector3(-25, 30, 25) * distance;
        transform.LookAt(target);
    }
}
