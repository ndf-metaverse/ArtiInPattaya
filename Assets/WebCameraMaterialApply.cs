using System.Collections;
using System.Collections.Generic;
using RenderHeads.Media.AVProLiveCamera;
using UnityEngine;

public class WebCameraMaterialApply : MonoBehaviour
{
    [SerializeField] DeviceCameraController easyWebcamController;
    [SerializeField] Material _material = null;
    [SerializeField] string _texturePropertyName = "_MainTex";
    public bool useSquareTexture;

    private int _propTexture = -1;
    private Texture _lastTexture;

    public DeviceCameraController WebcamCameraController
    {
        get { return easyWebcamController; }
        set
        {
            if (easyWebcamController != value)
            {
                easyWebcamController = value;
                Update();
            }
        }
    }

    public Material Material
    {
        get { return _material; }
        set
        {
            if (_material != value)
            {
                ApplyMapping(null);
                _material = value;
                Update();
            }
        }
    }


    public string TexturePropertyName
    {
        get { return _texturePropertyName; }
        set
        {
            if (_texturePropertyName != value)
            {
                ApplyMapping(null);
                _texturePropertyName = value;
                _propTexture = Shader.PropertyToID(_texturePropertyName);
                Update();
            }
        }
    }

    void Awake()
    {
        _propTexture = Shader.PropertyToID(_texturePropertyName);
    }

    void Update()
    {
        if (easyWebcamController != null && easyWebcamController.isPlaying)
        {
            ApplyMapping(useSquareTexture ? CropToSquare(easyWebcamController.dWebCam.preview) : easyWebcamController.dWebCam.preview);
        }
        else
        {
            ApplyMapping(null);
        }
    }

   void ApplyMapping(Texture texture)
{
    if (_lastTexture != texture)
    {
        if (_material != null)
        {
            if (_propTexture != -1)
            {
                if (!_material.HasProperty(_propTexture))
                {
                    Debug.LogError(string.Format("[AVProLiveCamera] Material {0} doesn't have texture property {1}", _material.name, _texturePropertyName), this);
                }
                _material.SetTexture(_propTexture, texture);
                
                // ตั้ง emission map เป็น texture เดียวกับ base map
                if (_material.HasProperty("_EmissionMap"))
                {
                    _material.SetTexture("_EmissionMap", texture);
                }
                // ตั้งสี emission เป็นขาวเต็ม
                if (_material.HasProperty("_EmissionColor"))
                {
                    _material.SetColor("_EmissionColor", Color.white);
                }
                // เปิด emission keyword
                _material.EnableKeyword("_EMISSION");

                _lastTexture = texture;
            }
        }
    }
}


    void OnDisable()
    {
        ApplyMapping(null);
    }

    public Texture2D CropToSquare(Texture source)
    {
        int size = Mathf.Min(source.width, source.height);
        int startX = (source.width - size) / 2;
        int startY = (source.height - size) / 2;

        Color[] pixels;
        if (source is RenderTexture renderTex)
        {
            RenderTexture currentActiveRT = RenderTexture.active;
            RenderTexture.active = renderTex;

            // Create a new Texture2D with the same size
            Texture2D texture2D = new Texture2D(size, size, TextureFormat.RGBA32, false);

            // Read the pixels from the active RenderTexture into the new Texture2D
            texture2D.ReadPixels(new Rect(startX, startY, size, size), 0, 0);
            texture2D.Apply();

            // Restore the previously active RenderTexture
            RenderTexture.active = currentActiveRT;

            pixels = texture2D.GetPixels(startX, startY, size, size);
        }
        else if (source is WebCamTexture webCamTex)
        {
            pixels = webCamTex.GetPixels(startX, startY, size, size);
        }
        else
        {
            Texture2D texture2D = source as Texture2D;
            pixels = texture2D.GetPixels(startX, startY, size, size);
        }

        Texture2D cropped = new Texture2D(size, size);
        cropped.SetPixels(pixels);
        cropped.Apply();
        return cropped;
    }
}
