using UnityEngine;
using UnityEngine.UI;
using ZXing;

public class DualWebcamController : MonoBehaviour
{
    public WebCameraMaterialApply cam1MaterialApply;
    public WebCameraMaterialApply cam2MaterialApply;

    public RawImage previewCam1;
    public RawImage previewCam2;

    private WebCamTexture cam1;
    private WebCamTexture cam2;
    private IBarcodeReader barcodeReader = new BarcodeReader();

    private Spawn spawnSystem;

    private bool canScanCam1 = true;
    private bool canScanCam2 = true;

    public string textScan;

    public static DualWebcamController instance;

    void Start()
    {
        instance = this;

        spawnSystem = FindObjectOfType<Spawn>();
        if (spawnSystem == null)
        {
            Debug.LogError("ไม่พบ Spawn System ใน Scene!");
        }

        var devices = WebCamTexture.devices;
        if (devices.Length < 2)
        {
            Debug.LogError("ต้องมีกล้อง 2 ตัวขึ้นไป");
            return;
        }

        // สร้าง WebCamTexture และเริ่มเล่น
        cam1 = new WebCamTexture(devices[0].name);
        cam2 = new WebCamTexture(devices[1].name);

        cam1.Play();
        cam2.Play();

        // เซ็ต RawImage preview
        if (previewCam1 != null) previewCam1.texture = cam1;
        if (previewCam2 != null) previewCam2.texture = cam2;

        // เซ็ต MaterialApply สำหรับกล้องแต่ละตัว ให้ใช้กล้องที่เปิด
        if (cam1MaterialApply != null)
            cam1MaterialApply.WebcamCameraController = null; // ถ้าไม่ได้ใช้ DeviceCameraController
                                                             // หรือกำหนด reference กล้องที่เหมาะสมถ้ามี (ตามระบบคุณ)
        if (cam2MaterialApply != null)
            cam2MaterialApply.WebcamCameraController = null;

        // ในที่นี้เราเซ็ต Material texture ตรงๆ
        if (cam1MaterialApply != null && cam1MaterialApply.Material != null)
            cam1MaterialApply.Material.mainTexture = cam1;
        if (cam2MaterialApply != null && cam2MaterialApply.Material != null)
            cam2MaterialApply.Material.mainTexture = cam2;

        StartCoroutine(ScanLoop(cam1, "Camera1"));
        StartCoroutine(ScanLoop(cam2, "Camera2"));
    }

    System.Collections.IEnumerator ScanLoop(WebCamTexture cam, string camName)
    {
        while (true)
        {
            if (cam.isPlaying && cam.width > 100)
            {
                try
                {
                    var data = cam.GetPixels32();
                    var result = barcodeReader.Decode(data, cam.width, cam.height);
                    if (result != null)
                    {
                        Debug.Log($"[{camName}] QR Code: {result.Text}");
                        textScan = result.Text;

                        if (spawnSystem != null)
                        {
                            if ((camName == "Camera1" && canScanCam1) || (camName == "Camera2" && canScanCam2))
                            {
                                if (camName == "Camera1")
                                {

                                    spawnSystem.SpawnObject(true,1);
                                }
                                else if (camName == "Camera2")
                                {

                                    spawnSystem.SpawnObject(true, 2);
                                }
                                StartCoroutine(ResetScanFlag(camName, 10f));
                            }
                        }
                    }
                }
                catch
                {
                    // ignore error
                }
            }
            yield return new WaitForSeconds(8);
        }
    }

    System.Collections.IEnumerator ResetScanFlag(string camName, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (camName == "Camera1") canScanCam1 = true;
        else canScanCam2 = true;
    }

    void OnDestroy()
    {
        if (cam1 != null) cam1.Stop();
        if (cam2 != null) cam2.Stop();
    }
}
