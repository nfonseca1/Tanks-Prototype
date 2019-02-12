using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    PlayerTank playerTank;

    float lerpX = 0;
    float lerpY = 0;
    float lerpLimit = 0.25f;

    // Start is called before the first frame update
    void Start()
    {
        playerTank = GetComponent<PlayerTank>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckInput();
        playerTank.Move(lerpY / lerpLimit);
        playerTank.Rotate(lerpX / lerpLimit);
    }

    private void CheckInput()
    {
        float axisX = Input.GetAxis("Horizontal");
        float axisY = Input.GetAxis("Vertical");
        ManageInput(axisX, ref lerpX);
        ManageInput(axisY, ref lerpY);
    }

    private void ManageInput(float input, ref float lerp)
    {
        if (input > 0)
        {
            lerp += Time.deltaTime;
            if(lerp > lerpLimit) { lerp = lerpLimit; }
        }
        else if (input < 0)
        {
            lerp -= Time.deltaTime;
            if (lerp < -lerpLimit) { lerp = -lerpLimit; }
        }
        else
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
}
