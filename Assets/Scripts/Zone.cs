using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zone : MonoBehaviour
{
    public bool readyToAttack = false;
    [SerializeField] AIController enemy;

    List<AIController> AI = new List<AIController>();
    float time = 0f;
    int numberOfAI = 0;

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
            if (tank != null)
            {
                print("still here");
            }
            else
            {
                print("not here");
                Vector3 spawnPosition = new Vector3(transform.position.x, 50f, transform.position.z);
                AIController currentEnemy = Instantiate(enemy, spawnPosition, Quaternion.identity);
                AI.Remove(tank);
                AI.Add(currentEnemy);
            }
        }
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
                print(AI.ToArray().Length);
            }
        }
    }
}
