using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    Gate[] gates;


    private void Start()
    {
        gates = FindObjectsOfType<Gate>();
    }

    public void OpenGates(int gateSet)
    {
        foreach(var gate in gates)
        {
            if (gate.GetGateSet() == gateSet)
            {
                gate.OpenGate();
            }
        }
    }
}
