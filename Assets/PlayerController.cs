using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    PlayerTank playerTank;

    float lerp = 0;
    float lerpLimit = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        playerTank = GetComponent<PlayerTank>();
    }

    // Update is called once per frame
    void Update()
    {
        ManageInput();
        playerTank.Move(lerp);
    }

    private void ManageInput()
    {
        float axisY = Input.GetAxis("Vertical");
        print(axisY);

        if (axisY > 0)
        {
            lerp += Time.deltaTime;
            if(lerp > lerpLimit) { lerp = lerpLimit; }
        }
        else if (axisY < 0)
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
