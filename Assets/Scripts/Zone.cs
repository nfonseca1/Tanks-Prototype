using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zone : MonoBehaviour
{
    public bool readyToAttack = false;
    [SerializeField] AIBasicTank enemy;
    [SerializeField] Transform[] spawnPoints;

    CapturePoint capturePoint;
    List<AIBasicTank> AI = new List<AIBasicTank>();
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

    private void SpawnEnemy(AIBasicTank tank)
    {
        int randomPoint = Mathf.RoundToInt(Random.Range(0, spawnPoints.Length));

        Vector3 spawnPositionRaw = spawnPoints[randomPoint].position;
        Vector3 spawnPosition = new Vector3(spawnPositionRaw.x, 50f, spawnPositionRaw.z);
        AIBasicTank currentEnemy = Instantiate(enemy, spawnPosition, Quaternion.identity);
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

        if (other.GetComponent<AIBasicTank>() != null)
        {
            if (!AI.Contains(other.GetComponent<AIBasicTank>()))
            {
                AI.Add(other.GetComponent<AIBasicTank>());
            }
        }

        if (other.GetComponent<CapturePoint>() != null)
        {
            capturePoint = other.GetComponent<CapturePoint>();
        }
    }
}
