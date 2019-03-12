using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] Transform player;
    Gate[] gates;
    CapturePoint[] capturePoints;
    int lives = 3;

    const float spawnDelay = 2f;


    private void Start()
    {
        gates = FindObjectsOfType<Gate>();
        capturePoints = FindObjectsOfType<CapturePoint>();
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

    public void CaptureZone(int gateSet)
    {
        int totalZones = 0;
        int capturedZones = 0;
        foreach (var zone in capturePoints)
        {
            if (zone.GetGateSet() == gateSet)
            {
                totalZones++;
            }
            if (zone.isCaptured)
            {
                capturedZones++;
            }
        }

        if (totalZones == capturedZones)
        {
            OpenGates(gateSet);
        }
    }

    public void Respawn(Vector3 location)
    {
        if (lives > 0)
        {
            StartCoroutine(SpawnNow(location));
        }
        else
        {
            print("No more lives");
        }
    }

    private IEnumerator SpawnNow(Vector3 location)
    {
        yield return new WaitForSeconds(spawnDelay);
        print("spawn");
        Instantiate(player.gameObject, new Vector3(location.x, location.y + 50, location.z), Quaternion.identity);
        lives--;
    }
}
