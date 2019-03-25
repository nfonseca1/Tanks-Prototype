using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : MonoBehaviour
{
    [SerializeField] MineExplosion explosion;
    [SerializeField] float explosionForce = 10f;
    [SerializeField] float explosionRadius = 7f;
    MeshCollider meshCollider;
    Material inside;
    float flashTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        meshCollider = GetComponent<MeshCollider>();

        Material[] materials = gameObject.GetComponent<MeshRenderer>().materials;
        foreach (var mat in materials)
        {
            if (mat.name == "Inside (Instance)")
            {
                inside = mat;
            }
        }

        StartCoroutine(Flash());
    }

    private void Update()
    {
        if (flashTime > 0)
        {
            flashTime -= Time.deltaTime;
        }
        else
        {
            inside.SetColor("_EmissionColor", Color.black);
        }
    }

    IEnumerator Flash()
    {
        while (true)
        {
            flashTime = 0.1f;
            inside.SetColor("_EmissionColor", Color.red);

            yield return new WaitForSeconds(1f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>() != null)
        {
            flashTime = 1f;
            inside.SetColor("_EmissionColor", Color.red);

            StartCoroutine(BlowUp());
        }
    }

    IEnumerator BlowUp()
    {
        yield return new WaitForSeconds(0.2f);

        Vector3 explodePoint = new Vector3(transform.position.x, transform.position.y + 0.01f, transform.position.z);

        Destroy(meshCollider);
        MineExplosion thisExplosion = Instantiate(explosion, explodePoint, transform.rotation);
        Destroy(thisExplosion.gameObject, 5f);
        thisExplosion.Explode(explosionForce, explosionRadius);
        Destroy(gameObject);
    }
}
