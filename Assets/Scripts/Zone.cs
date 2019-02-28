using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zone : MonoBehaviour
{
    public bool readyToAttack = false;
    [SerializeField] AIController enemy;

    List<AIController> AI = new List<AIController>();
    CapsuleCollider collider;
    float time = 0f;
    int numberOfAI = 0;

    private void Start()
    {
        collider = GetComponent<CapsuleCollider>();
    }

    private void Update()
    {
        time += Time.deltaTime;
        if (time >= 1f)
        {
            numberOfAI = AI.ToArray().Length;

            CheckForAI();
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
        float randomX = Random.Range(-1, 1);
        float randomY = Random.Range(-1, 1);
        float randomRadius = Random.Range(0, collider.radius);

        Vector3 pos = Vector3.Normalize(new Vector3(randomX, 0, randomY)) * randomRadius;

        Vector3 spawnPosition = transform.position + new Vector3(pos.x, 50f, pos.z);
        AIController currentEnemy = Instantiate(enemy, spawnPosition, Quaternion.identity);
        AI.Remove(tank);
        AI.Add(currentEnemy);
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
    }
}
