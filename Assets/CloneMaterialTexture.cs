using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloneMaterialTexture : MonoBehaviour
{
    public Renderer mainRenderer;
    public Material originalMaterial;
    public bool isRenderTexture;

    private void Start()
    {
        if (mainRenderer == null || originalMaterial == null)
            return;

        // Create new instance of material
        mainRenderer.material = new Material(originalMaterial);

        Texture2D originalTex = null;
        if (isRenderTexture)
        {
            RenderTexture renderTex = originalMaterial.mainTexture as RenderTexture;
            if (renderTex != null)
            {
                RenderTexture currentActiveRT = RenderTexture.active;
                RenderTexture.active = renderTex;

                // Create a new Texture2D with the same size
                originalTex = new Texture2D(renderTex.width, renderTex.height, TextureFormat.RGBA32, false);

                // Read the pixels from the active RenderTexture into the new Texture2D
                originalTex.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
                originalTex.Apply();

                // Restore the previously active RenderTexture
                RenderTexture.active = currentActiveRT;
            }
        }
        else
        {
            originalTex = originalMaterial.mainTexture as Texture2D;
        }

        if (originalTex != null)
        {
            Texture2D clonedTex = new Texture2D(originalTex.width, originalTex.height, originalTex.format, originalTex.mipmapCount > 1);
            Graphics.CopyTexture(originalTex, clonedTex);

            mainRenderer.material.mainTexture = clonedTex;
        }
    }

    private void OnDestroy()
    {
        // Clean up material from memory
        Destroy(mainRenderer.material);
    }
}
