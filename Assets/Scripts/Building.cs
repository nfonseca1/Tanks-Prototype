using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    [SerializeField] Renderer[] roofs;
    [SerializeField] Material opaqueMaterial;
    [SerializeField] Material transparentMaterial;
    bool isInside = false;
    bool isOpaque = true;

    private void Update()
    {
        if (isInside)
        {
            if (isOpaque)
            {
                foreach (Renderer roof in roofs)
                {
                    roof.material = transparentMaterial;
                }
                isOpaque = false;
            }

            Color roofColor = roofs[0].material.color;

            float alpha = Mathf.Lerp(roofColor.a, .1f, 0.3f);
            Color newColor = new Color(roofColor.r, roofColor.g, roofColor.b, alpha);

            foreach (Renderer roof in roofs)
            {
                roof.material.color = newColor;
            }
        }
        else
        {
            Color roof1Color = roofs[0].material.color;

            float alpha = Mathf.Lerp(roof1Color.a, 1f, 0.3f);
            Color newColor = new Color(roof1Color.r, roof1Color.g, roof1Color.b, alpha);

            if (alpha > .99f && !isOpaque)
            {
                foreach (Renderer roof in roofs)
                {
                    roof.material = opaqueMaterial;
                }
                isOpaque = true;
            }
            else
            {
                foreach (Renderer roof in roofs)
                {
                    roof.material.color = newColor;
                }
            }
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
