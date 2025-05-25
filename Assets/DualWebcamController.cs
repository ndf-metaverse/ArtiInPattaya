using System.Collections;
using System.Collections.Generic;
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
    public string textScan2;

    public static DualWebcamController instance;
    public bool firstScan;

    private HashSet<string> activeQRCodes = new HashSet<string>();
    private Dictionary<string, float> qrLastSeenTime = new Dictionary<string, float>();
    public float qrDisappearThreshold = 3f;

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

        cam1 = new WebCamTexture(devices[0].name);
        cam2 = new WebCamTexture(devices[1].name);

        cam1.Play();
        cam2.Play();

        if (previewCam1 != null) previewCam1.texture = cam1;
        if (previewCam2 != null) previewCam2.texture = cam2;

        if (cam1MaterialApply != null)
            cam1MaterialApply.WebcamCameraController = null; 
        if (cam2MaterialApply != null)
            cam2MaterialApply.WebcamCameraController = null;

        if (cam1MaterialApply != null && cam1MaterialApply.Material != null)
            cam1MaterialApply.Material.mainTexture = cam1;
        if (cam2MaterialApply != null && cam2MaterialApply.Material != null)
            cam2MaterialApply.Material.mainTexture = cam2;

        StartCoroutine(ScanLoop(cam1, "Camera1"));
        StartCoroutine(ScanLoop(cam2, "Camera2"));

        StartCoroutine(CheckQRDisappearLoop());

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
                        string qrText = result.Text;
                        qrLastSeenTime[qrText] = Time.time;

                        if (!activeQRCodes.Contains(qrText))
                        {
                            // ยังไม่ถูก mark ว่ากำลังถูกเห็น → spawn ได้
                            activeQRCodes.Add(qrText);

                            if (camName == "Camera1")
                            {
                                textScan = qrText;
                                spawnSystem.SpawnObject(true, 1, 0);
                            }
                            else if (camName == "Camera2")
                            {
                                textScan2 = qrText;
                                spawnSystem.SpawnObject(true, 2, 0);
                            }

                            StartCoroutine(ResetScanFlag(camName, 0.5f));
                        }
                    }

                }
                catch
                {
                    // ignore error
                }
            }
            yield return new WaitForSeconds(2);
        }
    }
    private IEnumerator CheckQRDisappearLoop()
    {
        while (true)
        {
            List<string> toRemove = new List<string>();

            foreach (var qr in activeQRCodes)
            {
                if (qrLastSeenTime.ContainsKey(qr) && Time.time - qrLastSeenTime[qr] > qrDisappearThreshold)
                {
                    toRemove.Add(qr);
                }
            }

            foreach (var qr in toRemove)
            {
                activeQRCodes.Remove(qr);
                qrLastSeenTime.Remove(qr);
                Debug.Log($"QR '{qr}' Holding");
            }

            yield return new WaitForSeconds(1f);
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
