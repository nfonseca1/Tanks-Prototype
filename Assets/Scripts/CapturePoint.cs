﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapturePoint : MonoBehaviour
{
    public bool isCaptured = false;
    [SerializeField] Transform flag;
    [SerializeField] int gateSet = 1;
    PlayerController player;
    float points = 0f;
    bool capturing = false;
    bool triggeredSuccessText = false;

    const float pointsForSuccess = 15f;
    const float minHeight = 6.0f;
    const float maxHeight = 25.6f;
    
    LevelManager levelManager;


    private void Start()
    {
        levelManager = FindObjectOfType<LevelManager>();
        flag.position = new Vector3(flag.position.x, minHeight, flag.position.z);
    }

    private void Update()
    {
        if (isCaptured)
        {
            flag.position = new Vector3(flag.position.x, maxHeight, flag.position.z);
            if (!triggeredSuccessText)
            {
                player.DisplayZoneCaptureText();
                triggeredSuccessText = true;
            }
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
                levelManager.CaptureZone(gateSet);
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>() != null)
        {
            player = other.GetComponent<PlayerController>();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<PlayerController>() != null)
        {
            if (other.GetComponent<SpawnedPlayer>() == null)
            {
                capturing = true;
            }
            else
            {
                capturing = false;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerController>() != null)
        {
            capturing = false;
        }
    }

    public int GetGateSet()
    {
        return gateSet;
    }
}
