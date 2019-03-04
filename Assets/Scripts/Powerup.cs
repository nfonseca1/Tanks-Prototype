using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    Canvas canvas;
    Camera camera;

    // Start is called before the first frame update
    void Start()
    {
        canvas = GetComponentInChildren<Canvas>();
        camera = FindObjectOfType<Camera>();
    }

    private void Update()
    {
        transform.Rotate(new Vector3(0, 180 * Time.deltaTime, 0));
    }

    public void TurnOnButtonPrompt()
    {
        canvas.enabled = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerController>() != null)
        {
            canvas.enabled = false;
        }
    }
}
