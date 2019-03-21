using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePad : MonoBehaviour
{
    [SerializeField] bool isMainSwitch = false;
    [SerializeField] int pressurePadGroup = 1;
    LevelManager levelManager;
    Material button;
    Color offColor;
    bool activated = false;
    bool playerPressing = false;

    private void Start()
    {
        levelManager = FindObjectOfType<LevelManager>();
        Material[] materials = gameObject.GetComponent<MeshRenderer>().materials;
        foreach (var mat in materials)
        {
            if (mat.name == "Button (Instance)")
            {
                button = mat;
            }
        }

        if (isMainSwitch)
        {
            offColor = Color.red;
        }
        else
        {
            offColor = Color.gray;
        }
        button.SetColor("_EmissionColor", offColor);
    }

    public int GetGroup()
    {
        return pressurePadGroup;
    }

    public void SetActive(bool isActive)
    {
        activated = isActive;
        if (activated)
        {
            button.SetColor("_EmissionColor", Color.green);
        }
        else
        {
            button.SetColor("_EmissionColor", offColor);
        }
    }

    void ActivateButton()
    {
        if (!playerPressing)
        {
            playerPressing = true;

            if (isMainSwitch)
            {
                activated = !activated;
            }
            else if (activated)
            {
                activated = false;
            }
            levelManager.ToggleSwitch(pressurePadGroup, activated);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>())
        {
            ActivateButton();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerController>())
        {
            playerPressing = false;
        }
    }
}
