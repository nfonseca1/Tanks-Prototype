using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTank : Tank
{
    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        barrelScale = new Vector3(1, 1, 1);
    }

    public void ElevateBarrel()
    {
        barrelWheel.Rotate(new Vector3(elevateSpeed * Time.deltaTime, 0, 0), Space.Self);
        if (barrelWheel.localEulerAngles.x >= maxBarrelHeight)
        {
            barrelWheel.localEulerAngles = new Vector3(maxBarrelHeight, defaultBarrelRot.y, defaultBarrelRot.z);
        }
    }
}
