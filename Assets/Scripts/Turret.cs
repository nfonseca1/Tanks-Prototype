using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    [SerializeField] Transform barrelWheel;
    [SerializeField] Transform leftBarrel;
    [SerializeField] Transform rightBarrel;
    [SerializeField] Transform leftEmitter;
    [SerializeField] Transform rightEmitter;
    [SerializeField] Bullet bullet;

    PlayerTank[] players;
    bool playersExist = false;
    Transform closestPlayer;
    Vector3 hitPoint;
    float fireRate = 0.2f;
    float timeUntilFire = 0f;
    bool leftBarrelIsNext = true;
    float launchVelocity = 20f;
    Vector3 barrelPosition;


    void Start()
    {
        StartCoroutine(GetClosestPlayerWithDelay());
    }

    // Update is called once per frame
    void Update()
    {
        GetPlayers();
        if (playersExist)
        {
            CalculateAimTarget();
            Rotate();
            Aim();
            ManageFireInput();
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

    private void ManageFireInput()
    {
        bool aimStatus = false;

        if (timeUntilFire <= 0)
        {
            if (leftBarrelIsNext)
            {
                Fire(leftBarrel, leftEmitter);
                leftBarrelIsNext = false;
            }
            else
            {
                Fire(rightBarrel, rightEmitter);
                leftBarrelIsNext = true;
            }
            timeUntilFire = fireRate;
        }
        else
        {
            timeUntilFire -= Time.deltaTime;

            UpdateBarrel();
        }

    }

    private void Fire(Transform barrel, Transform emitter)
    {
        Bullet currentBullet = Instantiate(bullet, emitter.position, emitter.rotation);
        currentBullet.ApplyForce(launchVelocity);
        Destroy(currentBullet.gameObject, 2.0f);
        
        barrel.localPosition = new Vector3(barrel.localPosition.x, barrel.localPosition.y, -0.1f);
    }

    private void UpdateBarrel()
    {
        Transform barrel;
        if (leftBarrelIsNext)
        {
            barrel = rightBarrel;
            barrelPosition = new Vector3(rightBarrel.localPosition.x, rightBarrel.localPosition.y, 0);
        }
        else
        {
            barrel = leftBarrel;
            barrelPosition = new Vector3(leftBarrel.localPosition.x, leftBarrel.localPosition.y, 0);
        }

        barrel.localPosition = Vector3.Lerp(barrel.localPosition, barrelPosition, 0.15f);
        
        if (barrel.localPosition.z >= 0.95f)
        {
            barrel.localPosition = barrelPosition;
        }
    }
}
