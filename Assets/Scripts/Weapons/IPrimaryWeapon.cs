using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPrimaryWeapon : IWeapon
{
    bool RotateTurret(float input);

    bool RotateTurretTowardsTarget(Vector3 position);

    bool RotateBarrel(float input);

    bool RotateBarrelToAngle(float angle);

    Vector3 GetSecondarySocketPoint();
}
