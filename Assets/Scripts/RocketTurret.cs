using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketTurret : MonoBehaviour
{
    [SerializeField] Transform explosion;
    [SerializeField] Transform barrelWheel;
    [SerializeField] Transform barrel;
    [SerializeField] Transform emitter;
    [SerializeField] Missile missile;

    PlayerTank[] players;
    bool playersExist = false;
    Transform closestPlayer;
    Vector3 hitPoint;
    float fireRate = 5f;
    float timeUntilFire = 0f;
    Vector3 barrelPosition;
    int shotsFired = 0;
    bool isTowerMounted = false;
    bool readyToBlowUp = false;
    bool engage = false;


    void Start()
    {
        StartCoroutine(GetClosestPlayerWithDelay());
        timeUntilFire = Random.Range(.1f, fireRate);
    }

    // Update is called once per frame
    void Update()
    {
        if (!readyToBlowUp && engage)
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

        if (barrelWheel.localEulerAngles.x >= 40)
        {
            barrelWheel.localEulerAngles = new Vector3(40, 0, 0);
        }
    }

    private void ManageFireInput()
    {
        if (timeUntilFire <= 0)
        {
            Fire(emitter);
            shotsFired++;
            timeUntilFire = fireRate;
        }
        else
        {
            timeUntilFire -= Time.deltaTime;
            UpdateBarrel();
        }
    }

    private void Fire(Transform emitter)
    {
        Missile currentMissile = Instantiate(missile, emitter.position, emitter.rotation);
        Destroy(currentMissile.gameObject, 10f);

        barrel.localPosition = new Vector3(barrel.localPosition.x, barrel.localPosition.y, -0.1f);
    }

    private void UpdateBarrel()
    {
        Transform barrelPos;
        barrelPos = barrel;
        barrelPosition = new Vector3(barrel.localPosition.x, barrel.localPosition.y, 0);

        barrel.localPosition = Vector3.Lerp(barrel.localPosition, barrelPosition, 0.15f);

        if (barrel.localPosition.z >= 0.95f)
        {
            barrel.localPosition = barrelPosition;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<Tower>() != null)
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
        if (collision.gameObject.GetComponent<Tower>() != null)
        {
            readyToBlowUp = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        Zone zone = other.GetComponent<Zone>();
        if (zone != null)
        {
            if (zone.readyToAttack == true)
            {
                engage = true;
            }
        }
    }
}
