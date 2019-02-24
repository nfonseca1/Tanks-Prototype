using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public bool playerPosIsBase = true;
    [SerializeField] Transform target;
    [SerializeField] [Range(1, 10)] float distance = 1f;
    Vector3 basePos;

    // Update is called once per frame
    void Update()
    {
        if (playerPosIsBase) { basePos = target.position; }
        if(target != null)
        {
            transform.position = basePos + new Vector3(-25, 50, -25) * distance;
            transform.LookAt(basePos);
        }
    }

    public void SetPosition(Vector3 newPosition)
    {
        basePos = newPosition;
    }
}
