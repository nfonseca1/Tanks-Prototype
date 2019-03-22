using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequentialZone : MonoBehaviour
{
    public bool readyToAttack = false;
    [SerializeField] AIEnemy[] enemies;
    [SerializeField] Transform[] spawnPoints;

    CapturePoint capturePoint;
    List<AIEnemy> AI = new List<AIEnemy>();
    float time = 0f;
    float timeUntilSpawn = 0f;
    int nextEnemy = 0;

    const float spawnTime = 5f;

    private void Update()
    {
        time += Time.deltaTime;
        if (time >= 1f && capturePoint != null && !capturePoint.isCaptured && nextEnemy < enemies.Length)
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

    private void SpawnEnemy(AIEnemy tank)
    {
        int randomPoint = Mathf.RoundToInt(Random.Range(0, spawnPoints.Length));

        Vector3 spawnPositionRaw = spawnPoints[randomPoint].position;
        Vector3 spawnPosition = new Vector3(spawnPositionRaw.x, 50f, spawnPositionRaw.z);
        AIEnemy currentEnemy = Instantiate(enemies[nextEnemy], spawnPosition, Quaternion.identity);
        AI.Remove(tank);
        AI.Add(currentEnemy);

        nextEnemy++;
        timeUntilSpawn = spawnTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>() != null)
        {
            readyToAttack = true;
        }

        if (other.GetComponent<AIEnemy>() != null)
        {
            if (!AI.Contains(other.GetComponent<AIEnemy>()))
            {
                AI.Add(other.GetComponent<AIEnemy>());
            }
        }

        if (other.GetComponent<CapturePoint>() != null)
        {
            capturePoint = other.GetComponent<CapturePoint>();
        }
    }
}
