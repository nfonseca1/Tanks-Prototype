using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public bool playerPosIsBase = true;
    [SerializeField] [Range(1, 15)] float baseDistance = 1f;
    Transform target;
    float distance;
    float lerp;
    Vector3 basePos;

    private void Start()
    {
        target = FindObjectOfType<PlayerController>().transform;
        distance = baseDistance;
        lerp = distance;
    }

    // Update is called once per frame
    void Update()
    {
        if(target != null)
        {
            if (playerPosIsBase) { basePos = target.position; }

            lerp = Mathf.Lerp(lerp, distance, 0.1f);
            transform.position = basePos + new Vector3(-25, 50, -25) * lerp;
            transform.LookAt(basePos);
        }
        else
        {
            if (FindObjectOfType<PlayerController>() != null)
            {
                target = FindObjectOfType<PlayerController>().transform;
            }
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
