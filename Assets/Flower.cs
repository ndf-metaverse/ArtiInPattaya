using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flower : MonoBehaviour
{
    public Animator animator;
    public string witherTrigger = "Wither";
    public float lifeTime = 6f;
    public float witherTime = 0.5f;

    [Space]
    public Renderer mainRenderer;
    // public int materialIndex;
    public Color[] colors;
    // private Material material;

    private void Start()
    {
        StartCoroutine(Do_SelfDestruct());

        /*
        material = new Material(mainRenderer.materials[materialIndex]);
        mainRenderer.materials[materialIndex] = material;
        material.SetColor("_BaseColor", colors[Random.Range(0, colors.Length)]);
        */

        mainRenderer.material.SetColor("_BaseColor", colors[Random.Range(0, colors.Length)]);
    }

    private void OnDestroy()
    {
        if (mainRenderer.material != null)
        {
            Destroy(mainRenderer.material);
            mainRenderer.material = null;
        }
    }

    IEnumerator Do_SelfDestruct()
    {
        yield return new WaitForSeconds(lifeTime);
        animator.SetTrigger(witherTrigger);
        Destroy(gameObject, witherTime);
    }
}
