using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : AIEnemy
{
    [SerializeField] Transform explosion;
    [SerializeField] Transform barrelWheel;
    [SerializeField] Transform leftBarrel;
    [SerializeField] Transform rightBarrel;
    [SerializeField] Transform leftEmitter;
    [SerializeField] Transform rightEmitter;
    [SerializeField] Bullet bullet;

    [SerializeField] float fireRate = 0.2f;
    [SerializeField] float fireRateTotal = 2.5f;
    [SerializeField] float launchVelocity = 20f;
    [SerializeField] int maxShots = 8;
    [SerializeField] float bulletLifetime = 2f;
    [SerializeField] float maxAimAngle = 40f;
    PlayerTank[] players;
    Animation leftBarrelAnim;
    Animation rightBarrelAnim;
    bool playersExist = false;
    Transform closestPlayer;
    Vector3 hitPoint;
    float timeUntilFire = 0f;
    float timeUntilTotalFire = 0f;
    bool leftBarrelIsNext = true;
    Vector3 barrelPosition;
    int shotsFired = 0;
    bool isTowerMounted = false;
    bool readyToBlowUp = false;
    bool engage = false;


    void Start()
    {
        StartCoroutine(GetClosestPlayerWithDelay());
        timeUntilTotalFire = Random.Range(0, fireRateTotal);

        leftBarrelAnim = leftBarrel.GetComponent<Animation>();
        rightBarrelAnim = rightBarrel.GetComponent<Animation>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!readyToBlowUp && engage && grounded)
        {
            GetPlayers();
            if (playersExist && closestPlayer)
            {
                CalculateAimTarget();
                Rotate();
                Aim();
                ManageFireInput();
            }
        }
    }

    private bool CalculateAimTarget()
    {
        if (closestPlayer != null)
        {
            hitPoint = closestPlayer.position;
        }
        else
        {
            GetClosestPlayerNow();
        }

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
            if (players.Length == 0)
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
        float closestDistance;
        if (players[0].GetComponent<PlayerController>().grounded)
        {
            closestPlayer = players[0].transform;
            closestDistance = (players[0].transform.position - transform.position).magnitude;
        }
        else
        {
            closestPlayer = null;
            closestDistance = 1000000f;
        }

        for (var i = 0; i < players.Length; i++)
        {
            if (i == 0) { continue; }

            if (players[i].GetComponent<PlayerController>().grounded)
            {
                float distanceToCheck = (players[i].transform.position - transform.position).magnitude;
                if (distanceToCheck < closestDistance)
                {
                    closestPlayer = players[i].transform;
                    closestDistance = distanceToCheck;
                }
            }
            else
            {
                continue;
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

        if (barrelWheel.localEulerAngles.x >= maxAimAngle)
        {
            barrelWheel.localEulerAngles = new Vector3(maxAimAngle, 0, 0);
        }
    }

    private void ManageFireInput()
    {
        if (shotsFired >= maxShots)
        {
            timeUntilTotalFire = fireRateTotal;
            shotsFired = 0;
        }

        if (timeUntilTotalFire <= 0)
        {
            if (timeUntilFire <= 0)
            {
                if (leftBarrelIsNext)
                {
                    Fire(leftBarrel, leftEmitter);
                    leftBarrelIsNext = false;
                    shotsFired++;
                }
                else
                {
                    Fire(rightBarrel, rightEmitter);
                    leftBarrelIsNext = true;
                    shotsFired++;
                }
                timeUntilFire = fireRate;
            }
            else
            {
                timeUntilFire -= Time.deltaTime;
            }
        }
        else
        {
            timeUntilTotalFire -= Time.deltaTime;
        }
    }

    private void Fire(Transform barrel, Transform emitter)
    {
        Bullet currentBullet = Instantiate(bullet, emitter.position, emitter.rotation);
        currentBullet.ApplyForce(launchVelocity);
        Destroy(currentBullet.gameObject, bulletLifetime);
        
        if (leftBarrelIsNext)
        {
            leftBarrelAnim.Play();
        }
        else
        {
            rightBarrelAnim.Play();
        }
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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Tower")
        {
            isTowerMounted = true;
        }
        else if (readyToBlowUp && collision.gameObject.layer == 11)
        {
            Transform currentExplosion = Instantiate(explosion, transform.position, Quaternion.identity);
            Destroy(currentExplosion.gameObject, 3f);
            Destroy(gameObject);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Tower")
        {
            readyToBlowUp = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!engage)
        {
            CPZone zone = other.GetComponent<CPZone>();
            SequentialZone sZone = other.GetComponent<SequentialZone>();
            if (zone != null && zone.readyToAttack == true)
            {
                engage = true;
            }
            else if (sZone != null && sZone.readyToAttack == true)
            {
                engage = true;
            }
        }
    }
}
