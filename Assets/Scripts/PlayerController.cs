using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    PlayerTank playerTank;
    Vector3 hitPoint;
    
    float lerpX = 0;
    float lerpY = 0;
    float lerpLimitX = 0.1f;
    float lerpLimitY = 0.25f;
    float fireRate = 2f;
    float timeUntilFire = 0f;
    
    void Start()
    {
        playerTank = GetComponent<PlayerTank>();
    }
    
    void Update()
    {
        CheckInput();
        CalculateAimTarget();
        ManageFireInput();

        playerTank.Move(lerpY / lerpLimitY); // lerp / lerplimit dictates the percentage of movement
        playerTank.Rotate(lerpX / lerpLimitX);
        playerTank.Aim(hitPoint);
    }

    private void CheckInput()
    {
        float axisX = Input.GetAxis("Horizontal");
        float axisY = Input.GetAxis("Vertical");

        ManageAxisInput(axisX, ref lerpX, lerpLimitX);
        ManageAxisInput(axisY, ref lerpY, lerpLimitY);


    }

    private void ManageAxisInput(float input, ref float lerp, float lerpLimit)
    {
        if (input > 0) // If we are pressing the axis input, add or subtract from the lerp
        {
            lerp += Time.deltaTime;
            if(lerp > lerpLimit) { lerp = lerpLimit; }
        }
        else if (input < 0)
        {
            lerp -= Time.deltaTime;
            if (lerp < -lerpLimit) { lerp = -lerpLimit; }
        }
        else // Otherwise add or subtract the lerp back to zero
        {
            if(lerp < 0)
            {
                lerp += Time.deltaTime;
                if (lerp >= 0) { lerp = 0; }
            }
            else if (lerp > 0)
            {
                lerp -= Time.deltaTime;
                if (lerp <= 0) { lerp = 0; }
            }
        }
    }

    private void ManageFireInput()
    {
        if (timeUntilFire <= 0)
        {
            if (Input.GetMouseButton(0))
            {
                playerTank.ElevateBarrel();
            }
            else if (Input.GetMouseButtonUp(0))
            {
                playerTank.Fire();
                timeUntilFire = fireRate;
            }
        }
        else
        {
            timeUntilFire -= Time.deltaTime;

            playerTank.UpdateBarrel();
        }
        
    }

    private void CalculateAimTarget()
    {
        Camera mainCamera = Camera.main;
        Vector3 mousePosition = Input.mousePosition;

        // Get world position of mouse
        Vector3 aimPoint = mainCamera.ScreenToWorldPoint(new Vector3(
            mousePosition.x,
            mousePosition.y,
            5f
            ));

        // Cast a ray into the world from the camera into the direction of the mouse's world position
        Vector3 aimDirection = aimPoint - mainCamera.gameObject.transform.position;
        Ray aimRay = new Ray(aimPoint, aimDirection);
        RaycastHit hitInfo;
        if (Physics.Raycast(aimRay, out hitInfo))
        {
            hitPoint = hitInfo.point;
        }
    }
}
