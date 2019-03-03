using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] Transform player;
    Gate[] gates;
    int lives = 3;


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

    public void Respawn(Vector3 location)
    {
        Instantiate(player.gameObject, new Vector3(location.x, location.y + 50, location.z), Quaternion.identity);
    }
}
