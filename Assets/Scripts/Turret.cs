using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    [SerializeField] Transform barrelWheel;
    [SerializeField] Transform leftBarrel;
    [SerializeField] Transform rightBarrel;

    PlayerTank[] players;
    bool playersExist = false;
    Transform closestPlayer;
    Vector3 hitPoint;


    void Start()
    {
        StartCoroutine(GetClosestPlayerWithDelay());
    }

    // Update is called once per frame
    void Update()
    {
        if (playersExist)
        {
            CalculateAimTarget();
            Rotate();
            Aim();
        }
    }

    private bool CalculateAimTarget()
    {
        hitPoint = closestPlayer.position;

        return true;
    }

    void GetPlayers()
    {
        players = FindObjectsOfType<PlayerTank>();
        if (players.Length == 0)
        {
            playersExist = false;
        }
        else
        {
            playersExist = true;
        }
    }

    IEnumerator GetClosestPlayerWithDelay()
    {
        while (true)
        {
            GetPlayers();
            if (players[0] == null)
            {
                playersExist = false;
            }
            else
            {
                playersExist = true;
                GetClosestPlayerNow();
            }
            yield return new WaitForSeconds(5f);
        }
    }

    private void GetClosestPlayerNow()
    {
        closestPlayer = players[0].transform;
        float closestDistance = (players[0].transform.position - transform.position).magnitude;

        for (var i = 0; i < players.Length; i++)
        {
            if (i == 0) { continue; }

            float distanceToCheck = (players[i].transform.position - transform.position).magnitude;
            if (distanceToCheck < closestDistance)
            {
                closestPlayer = players[i].transform;
                closestDistance = distanceToCheck;
            }
        }
    }

    void Rotate()
    {
        transform.LookAt(closestPlayer);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
    }

    void Aim()
    {
        barrelWheel.LookAt(closestPlayer);
        barrelWheel.localEulerAngles = new Vector3(barrelWheel.localEulerAngles.x, 0, 0);
    }
}
