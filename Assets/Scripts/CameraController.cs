using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public bool playerPosIsBase = true;
    [SerializeField] Transform target;
    [SerializeField] [Range(1, 15)] float baseDistance = 1f;
    float distance;
    float lerp;
    Vector3 basePos;

    private void Start()
    {
        distance = baseDistance;
        lerp = distance;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerPosIsBase) { basePos = target.position; }
        if(target != null)
        {
            lerp = Mathf.Lerp(lerp, distance, 0.1f);
            transform.position = basePos + new Vector3(-25, 50, -25) * lerp;
            transform.LookAt(basePos);
        }
    }

    public void SetPosition(Vector3 newPosition)
    {
        basePos = newPosition;
    }

    public void SetDistance(float distanceAdded)
    {
        distance = baseDistance + distanceAdded;
    }
}
