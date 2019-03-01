﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapturePoint : MonoBehaviour
{
    [SerializeField] Transform flag;
    float points = 0f;
    bool capturing = false;
    bool isCaptured = false;

    const float pointsForSuccess = 12f;
    const float minHeight = 6.0f;
    const float maxHeight = 25.6f;


    private void Start()
    {
        flag.position = new Vector3(flag.position.x, minHeight, flag.position.z);
    }

    private void Update()
    {
        if (isCaptured)
        {
            flag.position = new Vector3(flag.position.x, maxHeight, flag.position.z);
        }
        else
        {
            ManagePoints();
        }
    }

    private void ManagePoints()
    {
        if (capturing)
        {
            points = points + 1 * Time.deltaTime;
            if (points >= pointsForSuccess)
            {
                points = pointsForSuccess;
                isCaptured = true;
            }
        }
        else
        {
            points = points - 1 * Time.deltaTime;
            if (points <= 0)
            {
                points = 0;
            }
        }

        float flagHeight = Mathf.Lerp(minHeight, maxHeight, points / pointsForSuccess);
        flag.position = new Vector3(flag.position.x, flagHeight, flag.position.z);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<PlayerController>() != null)
        {
            capturing = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerController>() != null)
        {
            capturing = false;
        }
    }
}