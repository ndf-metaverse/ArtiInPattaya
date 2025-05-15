/// <summary>
/// write by 52cwalk,if you have some question ,please contract lycwalk@gmail.com
/// </summary>

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

using ZXing;
using ZXing.Common;
using ZXing.QrCode;
using System.IO;
using UnityEngine.Events;

public class QRCodeDecodeControllerWithResultPoint : MonoBehaviour
{
    [Serializable]
    public class UnityEventString : UnityEvent<string> { };
   
    bool decoding = false;		
	bool tempDecodeing = false;
	string dataText = null;
    Texture2D cameraTexture;
    ResultPoint[] resultPoints;
    int orientation;
	Vector2 offset;

    public DeviceCameraController e_DeviceController = null;
    public bool AutoRotate;
    public bool TryInverted;

    private Color[] orginalc;   	//the colors of the camera data.
	private Color32[] targetColorARR;   	//the colors of the camera data.
	private byte[] targetbyte;		//the pixels of the camera image.
	private int W, H, WxH;			//width/height of the camera image			
	int framerate = 0; 		

	public float time = 0f;   //the time to scan the qrcode
#if UNITY_IOS
	int blockWidth = 450;
#elif UNITY_ANDROID
	int blockWidth = 350;
#else
    int blockWidth = 350;
	#endif
	bool isInit = false;
	BarcodeReader barReader;

    public UnityEventString onQRScanFinished;
    public UnityEvent<Texture2D, ResultPoint[], Vector2, int> onGetResultPointsFinished;

    void Start()
	{
		barReader = new BarcodeReader ();
        barReader.AutoRotate = AutoRotate;
        barReader.TryInverted = TryInverted;

        if (!e_DeviceController) {
			e_DeviceController = GameObject.FindObjectOfType<DeviceCameraController>();
			if(!e_DeviceController)
			{
				Debug.LogError("the Device Controller is not exsit,Please Drag DeviceCamera from project to Hierarchy");
			}
		}
	}
	
	void Update()
	{
		#if UNITY_EDITOR
		if (framerate++ % 15== 0) {
#elif UNITY_IOS || UNITY_ANDROID
		if (framerate++ % 15== 0) {
#else
        if (framerate++ % 20== 0) {
#endif
            if (e_DeviceController.isPlaying && !decoding)
			{
				W = e_DeviceController.dWebCam.Width();					// get the image width
				H = e_DeviceController.dWebCam.Height();			// get the image height 

				if (W < 100 || H < 100) {
					return;
				}

				if(!isInit && W>100 && H>100)
				{
                    blockWidth = Math.Min(W, H);
                    isInit = true;
				}

				if(targetColorARR == null)
				{
					targetColorARR= new Color32[blockWidth * blockWidth];
				}

                if (cameraTexture == null || cameraTexture.width != blockWidth || cameraTexture.height != blockWidth)
                {
                    cameraTexture = new Texture2D(W, H, TextureFormat.RGBA32, false, false);
                }

                int posx = ((W-blockWidth)>>1);//
				int posy = ((H-blockWidth)>>1);
				
				orginalc = e_DeviceController.dWebCam.GetPixels(posx,posy,blockWidth,blockWidth);// get the webcam image colors

                //convert the color(float) to color32 (byte)
				for(int i=0;i!= blockWidth;i++)
				{
					for(int j = 0;j!=blockWidth ;j++)
					{
						targetColorARR[i + j*blockWidth].r = (byte)( orginalc[i + j*blockWidth].r*255);
						targetColorARR[i + j*blockWidth].g = (byte)(orginalc[i + j*blockWidth].g*255);
						targetColorARR[i + j*blockWidth].b = (byte)(orginalc[i + j*blockWidth].b*255);
						targetColorARR[i + j*blockWidth].a = 255;
					}
				}

                // Save camera texture
                cameraTexture.SetPixels(e_DeviceController.dWebCam.GetPixels());
                cameraTexture.Apply();

                // Memory QR image offset
                offset.x = posx;
                offset.y = posy;
#if !UNITY_WEBGL
                // scan the qrcode 
                Loom.RunAsync(() =>
				              {
					try
					{
						Result data;
						data = barReader.Decode(targetColorARR,blockWidth,blockWidth);//start decode
						if (data != null) // if get the result success
						{
							decoding = true; 	// set the variable is true
							dataText = data.Text;   // use the variable to save the code result
							resultPoints = data.ResultPoints;
							orientation = (int)data.ResultMetadata[ResultMetadataType.ORIENTATION];
						}
					}
					catch (Exception e)
					{
						//	Debug.LogError("Decode Error: " + e.Data.ToString());
						decoding = false;
					}
				});
#else
                Result data;
                data = barReader.Decode(targetColorARR, blockWidth, blockWidth);//start decode
                if (data != null) // if get the result success
                {
                    decoding = true;    // set the variable is true
                    dataText = data.Text;   // use the variable to save the code result
					resultPoints = data.ResultPoints;
					orientation = (int)data.ResultMetadata[ResultMetadataType.ORIENTATION];
                }
#endif

            }

            if (decoding)
			{
				countTime();
                // if the status variable is change
                if (tempDecodeing != decoding)
				{
                    onQRScanFinished.Invoke(dataText);//triger the scan finished event;
                    onGetResultPointsFinished.Invoke(cameraTexture, resultPoints, offset, orientation);
                }
				tempDecodeing = decoding;
			}
		}
	}
 	public void countTime()
	{
		time += Time.deltaTime;
		
	}
	/// <summary>
	/// Reset this scan param
	/// </summary>
	public void Reset()
	{
		decoding = false;
		tempDecodeing = decoding;
	}
    
	/// <summary>
	/// Stops the work.
	/// </summary>
	public void StartWork()
	{
		if (e_DeviceController != null) {
			e_DeviceController.StartWork();
		}
		decoding = false;
		tempDecodeing = decoding;
	}
	
	/// <summary>
	/// Stops the work.
	/// </summary>
	public void StopWork()
	{
		decoding = true;
		tempDecodeing = decoding;
		if (e_DeviceController != null) {
			e_DeviceController.StopWork();
		}
	}
	
	/// <summary>
	/// Decodes the by static picture.
	/// </summary>
	/// <returns> return the decode result string </returns>
	/// <param name="tex">target texture.</param>
	public static string DecodeByStaticPic(Texture2D tex)
	{
		BarcodeReader codeReader = new BarcodeReader ();
		codeReader.AutoRotate = true;
		codeReader.TryInverted = true;
		
		Result data = codeReader.Decode (tex.GetPixels32 (), tex.width, tex.height);
		if (data != null) {
			return data.Text;
		} else {
			return "decode failed!";
		}
	}
	
}
