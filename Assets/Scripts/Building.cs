using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    [SerializeField] Renderer roof1;
    [SerializeField] Renderer roof2;
    bool isInside = false;

    private void Update()
    {
        if (isInside)
        {
            Color roof1Color = roof1.material.color;
            Color roof2Color = roof2.material.color;

            float alpha = Mathf.Lerp(roof1Color.a, .1f, 0.3f);
            Color newColor = new Color(roof1Color.r, roof1Color.g, roof1Color.b, alpha);

            roof1.material.color = newColor;
            roof2.material.color = newColor;
        }
        else
        {
            Color roof1Color = roof1.material.color;
            Color roof2Color = roof2.material.color;

            float alpha = Mathf.Lerp(roof1Color.a, 1f, 0.3f);
            Color newColor = new Color(roof1Color.r, roof1Color.g, roof1Color.b, alpha);

            roof1.material.color = newColor;
            roof2.material.color = newColor;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<PlayerController>() != null)
        {
            isInside = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerController>() != null)
        {
            isInside = false;
        }
    }
}
