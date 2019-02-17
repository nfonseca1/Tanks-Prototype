using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trajectory : MonoBehaviour
{
    LineRenderer lineRenderer;
    float points = 11;
    GameObject root;
    int layerMask = 1 << 10;

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        root = new GameObject();
        layerMask = ~layerMask;
    }

    public void Calculate(float angle, float launchVelocity, Transform emitter)
    {
        float gravity = Mathf.Abs(Physics.gravity.y);
        float time = (2 * launchVelocity * Mathf.Sin(angle * Mathf.Deg2Rad)) / gravity;

        float timeDiv = time / points;
        
        root.transform.position = emitter.position;
        root.transform.eulerAngles = transform.eulerAngles;
        Vector3 start = transform.InverseTransformPoint(root.transform.position);

        lineRenderer.SetPosition(0, start);

        for (var i = 1; i < points; i++)
        {
            float timeInt = timeDiv * i;
            float xPos = launchVelocity * (timeInt) * Mathf.Cos(angle * Mathf.Deg2Rad);
            float yPos = launchVelocity * (timeInt) * Mathf.Sin(angle * Mathf.Deg2Rad) - 0.5f * gravity * Mathf.Pow(timeInt, 2);
            
            lineRenderer.SetPosition(i, new Vector3(0, start.y + yPos, start.z + xPos));
        }
    }

    public bool CalculateHit (float angle, float launchVelocity, Transform emitter)
    {
        List<Vector3> trajectoryPoints = new List<Vector3>();

        float gravity = Mathf.Abs(Physics.gravity.y);
        float time = (2 * launchVelocity * Mathf.Sin(angle * Mathf.Deg2Rad)) / gravity;

        float timeDiv = time / points;

        root.transform.position = emitter.position;
        root.transform.eulerAngles = transform.eulerAngles;
        Vector3 start = transform.InverseTransformPoint(root.transform.position);
        trajectoryPoints.Add(start);

        for (var i = 1; i < points; i++)
        {
            float timeInt = timeDiv * i;
            float xPos = launchVelocity * (timeInt) * Mathf.Cos(angle * Mathf.Deg2Rad);
            float yPos = launchVelocity * (timeInt) * Mathf.Sin(angle * Mathf.Deg2Rad) - 0.5f * gravity * Mathf.Pow(timeInt, 2);
            trajectoryPoints.Add(new Vector3(0, start.y + yPos, start.z + xPos));

            Ray ray = new Ray(trajectoryPoints[i - 1], trajectoryPoints[i]);
            RaycastHit hitInfo;
            if(Physics.Raycast(ray, out hitInfo, (trajectoryPoints[i] - trajectoryPoints[i-1]).magnitude, layerMask))
            {
                return false;
            }
        }
        return true;
    }
}
