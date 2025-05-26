using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloneMaterialTexture : MonoBehaviour
{
    public Renderer[] mainRenderers;
    public Material originalMaterial;
    public Material SecondMaterial;
    public string textureName = "_BaseMap";
    public int camUse;

    private void Start()
    {
        if (mainRenderers == null || originalMaterial == null)
            return;

        // เตรียม texture ที่จะใช้ซ้ำ
        Texture texture = originalMaterial.GetTexture(textureName);
        if (camUse == 1)
        {
            texture = originalMaterial.GetTexture(textureName);
        }
        else if (camUse == 2)
        {
            texture = SecondMaterial.GetTexture(textureName);
        }

        Texture2D originalTex = null;

        if (texture is RenderTexture renderTex)
        {
            RenderTexture currentActiveRT = RenderTexture.active;
            RenderTexture.active = renderTex;

            originalTex = new Texture2D(renderTex.width, renderTex.height, TextureFormat.RGBA32, false);
            originalTex.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
            originalTex.Apply();

            RenderTexture.active = currentActiveRT;
        }
        else if (texture is WebCamTexture webCamTex)
        {
            originalTex = new Texture2D(webCamTex.width, webCamTex.height, TextureFormat.RGBA32, false);
            originalTex.SetPixels(webCamTex.GetPixels());
            originalTex.Apply();
        }
        else
        {
            originalTex = originalMaterial.mainTexture as Texture2D;
        }

        Texture2D clonedTex = null;
        if (originalTex != null)
        {
            clonedTex = new Texture2D(originalTex.width, originalTex.height, originalTex.format, originalTex.mipmapCount > 1);
            Graphics.CopyTexture(originalTex, clonedTex);
        }

        // Loop ทุก renderer แล้ว assign material ใหม่พร้อม texture และตั้ง EmissionMap ให้เหมือน BaseMap
        foreach (Renderer rend in mainRenderers)
        {
            if (rend == null) continue;

            Material[] mats = rend.materials;
            for (int i = 0; i < mats.Length; i++)
            {
                mats[i] = new Material(originalMaterial);
                if (clonedTex != null)
                {
                    mats[i].mainTexture = clonedTex;

                    // ตั้ง Emission Map ใช้ texture เดียวกับ Base Map
                    mats[i].SetTexture("_EmissionMap", clonedTex);

                    // เปิดใช้งาน emission keyword ใน shader
                    mats[i].EnableKeyword("_EMISSION");
                }
            }
            rend.materials = mats;
        }
    }

    private void OnDestroy()
    {
        if (mainRenderers == null) return;

        foreach (Renderer rend in mainRenderers)
        {
            if (rend == null) continue;
            foreach (var mat in rend.materials)
            {
                if (mat != null)
                    Destroy(mat);
            }
        }
    }
}
