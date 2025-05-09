using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TBEasyWebCam;
using System.Collections;

public class QRDecodeTest : MonoBehaviour
{
	public QRCodeDecodeController e_qrController;

	public Text UiText;

	public GameObject resetBtn;

	public GameObject scanLineObj;
    
	public Image torchImage;

    private float scanCooldown = 2f;
    private bool canScan = true;
    
    public bool isOpenBrowserIfUrl;
	public Spawn spawn;
	public CameraManager cameraManager;
    private void Start()
	{
	}

	private void Update()
	{
	}

	public void qrScanFinished(string dataText)
	{
        if (!canScan) return;

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

        UiText.text = dataText;
        if (resetBtn != null) resetBtn.SetActive(true);
        if (scanLineObj != null) scanLineObj.SetActive(false);
		StartCoroutine(cooldownscan());
	}
	public IEnumerator cooldownscan()
	{
		yield return new WaitForSeconds(2);
        cameraManager.StartAVProAfterQRScan();

    }
    public void ScanCooldown()
    {
		cameraManager.StartQRScan();

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
		Reset ();
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
