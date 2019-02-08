using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Transform target;

    // Update is called once per frame
    void Update()
    {
        transform.position = target.position + new Vector3(-25, 30, 25);
        transform.LookAt(target);
    }
}
