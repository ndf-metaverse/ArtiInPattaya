using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TBEasyWebCam;
using System.Collections;

public class QRDecodeTest2 : MonoBehaviour
{
    public QRCodeDecodeController e_qrController;

    public Text UiText;

    public GameObject resetBtn;

    public GameObject scanLineObj;

    public Image torchImage;

    public string textScan;
    private bool canScan = true;

    public bool isOpenBrowserIfUrl;
    public Spawn spawn;
    public CameraManager cameraManager;
    public static QRDecodeTest2 instance;
    public float number;

    public void TestNumber()
    {
        number += Time.deltaTime;
        Debug.Log(number);
    }
    private void Start()
    {
        instance = this;

    }

    private void Update()
    {
    }

    public void qrScanFinished(string dataText)
    {
        if (!canScan) return;
        TestNumber();
        canScan = false;
        Debug.Log(dataText);

        if (isOpenBrowserIfUrl && Utility.CheckIsUrlFormat(dataText))
        {
            if (!dataText.Contains("http://") && !dataText.Contains("https://"))
            {
                dataText = "http://" + dataText;
            }
            Application.OpenURL(dataText);
        }
        textScan = dataText;
        UiText.text = dataText;
        if (resetBtn != null) resetBtn.SetActive(true);
        if (scanLineObj != null) scanLineObj.SetActive(false);
        StartCoroutine(cooldownscan());
    }
    public IEnumerator cooldownscan()
    {
        if(e_qrController.time > 0.02f)
        {
            Spawn.instance.SpawnObject(true);
            e_qrController.time = 0;
        }

        yield return new WaitForSeconds(2);
        // cameraManager.StartAVProAfterQRScan();

        ScanCooldown();

    }
    public void ScanCooldown()
    {
        // cameraManager.StartQRScan();

        Debug.Log("Reset");
        canScan = true;
        Reset();
    }
    public void Reset()
    {
        if (this.e_qrController != null)
        {
            this.e_qrController.Reset();
        }

        if (this.UiText != null)
        {
            this.UiText.text = string.Empty;
        }
        if (this.resetBtn != null)
        {
            this.resetBtn.SetActive(false);
        }
        if (this.scanLineObj != null)
        {
            this.scanLineObj.SetActive(true);
        }
    }

    public void Play()
    {
        Reset();
        if (this.e_qrController != null)
        {
            this.e_qrController.StartWork();
        }
    }

    public void Stop()
    {
        if (this.e_qrController != null)
        {
            this.e_qrController.StopWork();
        }

        if (this.resetBtn != null)
        {
            this.resetBtn.SetActive(false);
        }
        if (this.scanLineObj != null)
        {
            this.scanLineObj.SetActive(false);
        }
    }

    public void GotoNextScene(string scenename)
    {
        if (this.e_qrController != null)
        {
            this.e_qrController.StopWork();
        }
        //Application.LoadLevel(scenename);
        SceneManager.LoadScene(scenename);
    }


}
