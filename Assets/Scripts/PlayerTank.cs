using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTank : Tank
{
    [SerializeField] ParticleSystem particleSystem1;
    [SerializeField] ParticleSystem particleSystem2;
    ParticleSystem.EmissionModule frontEmission;
    ParticleSystem.EmissionModule backEmission;
    Trajectory trajectory;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        trajectory = turret.GetComponent<Trajectory>();
        frontEmission = particleSystem1.emission;
        backEmission = particleSystem2.emission;
    }

    public new void Move(float input)
    {
        base.Move(input);

        if (input > 0)
        {
            frontEmission.enabled = true;
            backEmission.enabled = false;
        }
        else if (input < 0)
        {
            frontEmission.enabled = false;
            backEmission.enabled = true;
        }
        else
        {
            frontEmission.enabled = false;
            backEmission.enabled = false;
        }
    }

    public void ElevateBarrel()
    {
        barrelWheel.Rotate(new Vector3(elevateSpeed * Time.deltaTime, 0, 0), Space.Self);
        if (barrelWheel.localEulerAngles.x >= maxBarrelHeight)
        {
            barrelWheel.localEulerAngles = new Vector3(maxBarrelHeight, defaultBarrelRot.y, defaultBarrelRot.z);
        }
        //trajectory.Calculate(barrelWheel.localEulerAngles.x, launchVelocity, emitter);
    }
}
