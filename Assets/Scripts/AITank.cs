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

    public void ElevateBarrel()
    {
        barrelWheel.localEulerAngles = Vector3.Lerp(barrelWheel.localEulerAngles, aimEuler, 0.2f);
    }
}
