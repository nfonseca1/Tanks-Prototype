using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EnemyController : MonoBehaviour
{
    [SerializeField] float speed = 25f;
    [SerializeField] float torque = 45f;
    [SerializeField] float launchVelocity = 25f;
    [SerializeField] float elevateSpeed = 75f;

    [SerializeField] float explosionForce = 200f;
    [SerializeField] float explosionLift = 1f;

    [SerializeField] Transform turret;
    [SerializeField] Transform barrelWheel;
    [SerializeField] Transform barrel;
    [SerializeField] Transform emitter;
    [SerializeField] Shell shell;
    [SerializeField] Transform explosionPoint;

    TankController[] players;
    Transform closestPlayer;
    float maxDistance = 50f;
    float minDistance = 25f;
    float neutralDistance = 30f;
    float maxShootDistance = 50f;
    bool isMoving = false;

    Rigidbody rigidbody;
    Vector3 hitPoint;
    Vector3 aimEuler;
    float acceleration = 0;
    bool barrelUpdated = true;
    float lerp = 0;
    float fireWaitTime = 0f;

    Vector3 barrelScale;
    const float fireRate = 3f;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = gameObject.GetComponent<Rigidbody>();

        barrelWheel.localEulerAngles = new Vector3(0, 180, 0);
        barrelScale = barrel.localScale;

        GetPlayers();
        StartCoroutine(GetClosestPlayer());
    }

    private void Update()
    {
        CalculateAimAngle();

        if ((hitPoint - transform.position).magnitude < maxShootDistance && !isMoving)
        {
            fireWaitTime += Time.deltaTime;
            ElevateBarrel();
            Fire();
        }
        if (!barrelUpdated)
        {
            UpdateBarrel();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if ((closestPlayer.position - transform.position).magnitude > maxDistance || isMoving)
        {
            Move();
        }
        bool aimResult = CalculateAimTarget();
        Aim(aimResult);
        
    }

    void GetPlayers()
    {
        players = FindObjectsOfType<TankController>();
    }

    private void Move()
    {
        float distance = (closestPlayer.position - transform.position).magnitude;
        if (distance > neutralDistance || lerp > 0)
        {
            isMoving = true;

            if(distance > neutralDistance)
            {
                lerp = Mathf.Lerp(lerp, 1, 0.1f);
            }
            else
            {
                lerp = Mathf.Lerp(lerp, 0, 0.1f);
                if(lerp < 0.05f)
                {
                    lerp = 0;
                }
            }

            Vector3 newPosition = transform.position + (transform.forward * speed * lerp * Time.deltaTime);
            Vector3 newPositionXZ = new Vector3(newPosition.x, transform.position.y, newPosition.z);
            rigidbody.MovePosition(newPositionXZ);

            Vector3 playerLookPoint = closestPlayer.position;
            Quaternion targetRotation = Quaternion.LookRotation(playerLookPoint - transform.position, transform.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, .2f);
            transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);
        }
        else
        {
            isMoving = false;
        }
    }

    IEnumerator GetClosestPlayer()
    {
        while (true)
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
            yield return new WaitForSeconds(5f);
        }
    }

    private bool CalculateAimTarget()
    {
        hitPoint = closestPlayer.position;

        return true;
    }

    private void CalculateAimAngle()
    {
        float aimDistance = (hitPoint - transform.position).magnitude;
        float aimAngle = 0.5f * (Mathf.Asin((Physics.gravity.y * aimDistance) / Mathf.Pow(launchVelocity, 2)) * Mathf.Rad2Deg);
        aimEuler = new Vector3(-aimAngle, 180, 0);
    }

    private void Aim(bool aimResult)
    {
        if (aimResult)
        {
            Vector3 turretLookPoint = new Vector3(hitPoint.x, turret.position.y, hitPoint.z);
            Quaternion targetRotation = Quaternion.LookRotation(turretLookPoint - turret.position, turret.up);
            turret.rotation = Quaternion.Lerp(turret.rotation, targetRotation, .3f);
            turret.localEulerAngles = new Vector3(0, turret.localEulerAngles.y, 0);
        }
    }

    private void ElevateBarrel()
    {
        print(barrelWheel.localEulerAngles + " - " + aimEuler);
        barrelWheel.localEulerAngles = Vector3.Lerp(barrelWheel.localEulerAngles, aimEuler, 0.2f);
    }

    private void Fire()
    {
        if(fireWaitTime >= fireRate)
        {
            fireWaitTime = 0;

            Shell currentShell = Instantiate(shell, emitter.position, emitter.rotation);
            currentShell.ApplyForce(launchVelocity);
            Destroy(currentShell, 10f);

            rigidbody.AddExplosionForce(explosionForce, explosionPoint.position, 100f, explosionLift);
            barrel.localScale = new Vector3(barrel.localScale.x, barrel.localScale.y, barrel.localScale.z * 0.7f);
            barrelUpdated = false;
        }
    }

    private void UpdateBarrel()
    {
        barrelUpdated = false;
        barrel.localScale = Vector3.Lerp(barrel.localScale, barrelScale, 0.2f);

        if (barrel.localScale.z >= 0.95f)
        {
            barrel.localScale = barrelScale;
            barrelUpdated = true;
        }
    }
}
