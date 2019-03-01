using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zone : MonoBehaviour
{
    public bool readyToAttack = false;
    [SerializeField] AIController enemy;
    [SerializeField] Transform[] spawnPoints;

    CapturePoint capturePoint;
    List<AIController> AI = new List<AIController>();
    float time = 0f;
    float timeUntilSpawn = 0f;

    const float spawnTime = 5f;

    private void Update()
    {
        time += Time.deltaTime;
        if (time >= 1f && capturePoint != null && !capturePoint.isCaptured)
        {
            if (timeUntilSpawn <= 0)
            {
                CheckForAI();
            }
            else
            {
                timeUntilSpawn -= Time.deltaTime;
            }
        }
    }

    private void CheckForAI()
    {
        foreach (var tank in AI)
        {
            if (tank == null)
            {
                SpawnEnemy(tank);
                break;
            }
        }
    }

    private void SpawnEnemy(AIController tank)
    {
        int randomPoint = Mathf.RoundToInt(Random.Range(0, spawnPoints.Length));

        Vector3 spawnPositionRaw = spawnPoints[randomPoint].position;
        Vector3 spawnPosition = new Vector3(spawnPositionRaw.x, 50f, spawnPositionRaw.z);
        AIController currentEnemy = Instantiate(enemy, spawnPosition, Quaternion.identity);
        AI.Remove(tank);
        AI.Add(currentEnemy);

        timeUntilSpawn = spawnTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>() != null)
        {
            readyToAttack = true;
        }

        if (other.GetComponent<AIController>() != null)
        {
            if (!AI.Contains(other.GetComponent<AIController>()))
            {
                AI.Add(other.GetComponent<AIController>());
            }
        }

        if (other.GetComponent<CapturePoint>() != null)
        {
            capturePoint = other.GetComponent<CapturePoint>();
        }
    }
}
