using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trajectory : MonoBehaviour
{
    LineRenderer lineRenderer;

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    public void Calculate(float angle, float launchVelocity)
    {
        float time = (2 * launchVelocity * Mathf.Sin(angle * Mathf.Deg2Rad)) / Mathf.Abs(Physics.gravity.y);

        print(time);
    }
}
