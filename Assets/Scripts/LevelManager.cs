using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] Transform player;
    Gate[] gates;
    int lives = 3;

    const float spawnDelay = 2f;


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

        Instantiate(player.gameObject, new Vector3(location.x, location.y + 50, location.z), Quaternion.identity);
        lives--;
    }
}
