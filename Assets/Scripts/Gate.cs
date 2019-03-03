using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
    [SerializeField] CapturePoint[] capturePoints;
    Animation animation;

    const int gateSet = 1;

    // Start is called before the first frame update
    void Start()
    {
        animation = GetComponent<Animation>();
    }

    public void OpenGate()
    {
        animation.Play();
    }

    public int GetGateSet()
    {
        return gateSet;
    }
}
