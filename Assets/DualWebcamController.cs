using UnityEngine;
using UnityEngine.UI;

public class DualWebcamController : MonoBehaviour
{
    public RawImage camDisplay1;
    public RawImage camDisplay2;

    public WebCamTexture camTexture1;
    public WebCamTexture camTexture2;

    void Start()
    {
        WebCamDevice[] devices = WebCamTexture.devices;

        if (devices.Length < 2)
        {
            Debug.LogError("ต้องมีกล้องอย่างน้อย 2 ตัวถึงจะใช้งานได้");
            return;
        }

        try
        {
            camTexture1 = new WebCamTexture(devices[0].name);
            camDisplay1.texture = camTexture1;
            camDisplay1.material.mainTexture = camTexture1;
            camTexture1.Play();
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error starting camera 1: " + ex.Message);
        }

        try
        {
            camTexture2 = new WebCamTexture(devices[1].name);
            camDisplay2.texture = camTexture2;
            camDisplay2.material.mainTexture = camTexture2;
            camTexture2.Play();
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error starting camera 2: " + ex.Message);
        }
    }

    void OnDestroy()
    {
        if (camTexture1 != null && camTexture1.isPlaying) camTexture1.Stop();
        if (camTexture2 != null && camTexture2.isPlaying) camTexture2.Stop();
    }
}
