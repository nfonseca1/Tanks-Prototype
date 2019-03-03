using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    Camera camera;

    // Start is called before the first frame update
    void Start()
    {
        camera = FindObjectOfType<Camera>();
    }

    private void Update()
    {
        transform.Rotate(new Vector3(0, 180 * Time.deltaTime, 0));
    }
}
