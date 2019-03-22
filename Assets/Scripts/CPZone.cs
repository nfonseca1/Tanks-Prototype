using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPZone : MonoBehaviour
{
    public bool readyToAttack = false;
    [SerializeField] AIEnemy[] enemiesToSpawn;
    [SerializeField] Transform[] spawnPoints;
    [SerializeField] Transform[] turretSpawnPoints;

    CapturePoint capturePoint;
    Dictionary<AIEnemy, string> currentAI = new Dictionary<AIEnemy, string>();
    float time = 0f;
    float timeUntilSpawn = 0f;

    const float spawnTime = 5f;

    

    // Update is called once per frame
    void Update()
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
        foreach (var tank in currentAI)
        {
            if (tank.Key == null)
            {
                SpawnEnemy(tank);
                break;
            }
        }
    }

    private void SpawnEnemy(KeyValuePair<AIEnemy, string> tank)
    {
        string enemyType = tank.Value;
        Vector3 spawnPosition;

        if (enemyType == "AITurret" || enemyType == "AIGunTurret" || enemyType == "RocketTurret")
        {
            int randomPoint = Mathf.RoundToInt(Random.Range(0, turretSpawnPoints.Length));
            Vector3 spawnPositionRaw = turretSpawnPoints[randomPoint].position;
            spawnPosition = new Vector3(spawnPositionRaw.x, 50f, spawnPositionRaw.z);
        }
        else
        {
            int randomPoint = Mathf.RoundToInt(Random.Range(0, spawnPoints.Length));
            Vector3 spawnPositionRaw = spawnPoints[randomPoint].position;
            spawnPosition = new Vector3(spawnPositionRaw.x, 50f, spawnPositionRaw.z);
        }
        
        AIEnemy currentEnemy = null;
        for (var i = 0; i < enemiesToSpawn.Length; i++)
        {
            if (enemiesToSpawn[i].tag == enemyType)
            {
                currentEnemy = Instantiate(enemiesToSpawn[i], spawnPosition, Quaternion.identity);
                break;
            }
        }
        currentAI.Remove(tank.Key);
        if (currentEnemy != null)
        {
            currentAI.Add(currentEnemy, currentEnemy.tag);
        }

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
            if (!currentAI.ContainsKey(other.GetComponent<AIEnemy>()))
            {
                AIEnemy newAI = other.GetComponent<AIEnemy>();
                currentAI.Add(newAI, newAI.tag);
            }
        }

        if (other.GetComponent<CapturePoint>() != null)
        {
            capturePoint = other.GetComponent<CapturePoint>();
        }
    }
}
