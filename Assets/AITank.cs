using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AITank : Tank
{
    Transform closestPlayer;
    Vector3 aimEuler;
    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    public void Rotate(float input)
    {
        Vector3 playerLookPoint = closestPlayer.position;
        Quaternion targetRotation = Quaternion.LookRotation(playerLookPoint - transform.position, transform.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, .2f);
        transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);
    }

    public void ElevateBarrel()
    {
        barrelWheel.localEulerAngles = Vector3.Lerp(barrelWheel.localEulerAngles, aimEuler, 0.2f);
    }
}
