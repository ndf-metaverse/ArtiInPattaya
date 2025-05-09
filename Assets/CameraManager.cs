using UnityEngine;
using System.Collections;
using RenderHeads.Media.AVProLiveCamera;
using UnityEngine.Playables;
using UnityEngine.UI;

public class CameraManager : MonoBehaviour
{
    public DeviceCameraController easyWebcamController;
    public AVProLiveCamera avProLiveCamera;
    public RawImage previewImage;

    public int deviceIndex = 0;
    public int modeIndex = 0;
    public float frameRate = 30f;

    public GameObject previreImage;
    public QRDecodeTest qrDecodeTest;

    public AVProLiveCameraDevice avproDevice;
    public void StartQRScan()
    {
        StopAVPro();
        //previreImage.SetActive(false);
        easyWebcamController.StartWork();

        // ✅ แสดง WebCamTexture บน RawImage
        if (previewImage != null)
        {
            previewImage.texture = easyWebcamController.dWebCam.preview;
        }
    }

    public void StartAVProAfterQRScan()
    {
        StartCoroutine(SwitchToAVProViaComponent());
    }

    private IEnumerator SwitchToAVProViaComponent()
    {
        previreImage.SetActive(true);

        easyWebcamController.StopWork();
        yield return new WaitForSeconds(0.5f);

        avProLiveCamera._deviceSelection = AVProLiveCamera.SelectDeviceBy.Index;
        avProLiveCamera._desiredDeviceIndex = deviceIndex;

        avProLiveCamera._modeSelection = AVProLiveCamera.SelectModeBy.Index;
        avProLiveCamera._desiredModeIndex = modeIndex;
        avProLiveCamera._desiredFrameRate = frameRate;

        avProLiveCamera.Begin();

        yield return new WaitForSeconds(4);  // รอกล้องเริ่มทำงาน

        if (previewImage != null && avProLiveCamera.Device != null)
        {
            previewImage.texture = avProLiveCamera.Device.OutputTexture;
            Spawn.instance.SpawnObject();

            qrDecodeTest.ScanCooldown();

        }
    }

    public void StopAVPro()
    {
        if (avProLiveCamera.Device != null && avProLiveCamera.Device.IsRunning)
        {
            avProLiveCamera.Device.Close();
        }
    }
}
