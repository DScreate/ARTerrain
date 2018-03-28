using System;
using UnityEngine;

public class WebcamTextureController : MonoBehaviour {

    public string requestedDeviceName;

    public int webcamRequestedWidth = 640;
    public int webcamRequestedHeight = 480;

    private WebCamTexture webcamtex;

    private int deviceIndex;

    public WebCamTexture Webcamtex
    {
        get { return webcamtex; }
    }

    public void Initialize()
    {
        WebCamDevice[] devices = WebCamTexture.devices;

        if (!String.IsNullOrEmpty(requestedDeviceName))
        {
            for (int i = 0; i < devices.Length; i++)
            {
                if (devices[i].name == requestedDeviceName)
                {
                    webcamtex = new WebCamTexture(requestedDeviceName, webcamRequestedWidth, webcamRequestedHeight);
                    deviceIndex = i;
                    break;
                }
            }
        }

        if (webcamtex == null)
        {
            webcamtex = new WebCamTexture(webcamRequestedWidth, webcamRequestedHeight);
            deviceIndex = 0;
        }

        //webcamRequestedWidth = webcamtex.requestedWidth;
        //webcamRequestedHeight = webcamtex.requestedHeight;

        webcamtex.Play();

        Debug.Log("Webcam request width: " + webcamtex.requestedWidth + ". Webcam requested height: " + webcamtex.requestedHeight);
    }

    //Will probably get weird behavior here if width and height are different from before

    public void ChangeWebcamTextureToNextAvailable()
    {
        string nextWebcamDeviceName = GetNextWebCamDevice().name;

        webcamtex.Stop();

        webcamtex = new WebCamTexture(nextWebcamDeviceName, webcamRequestedWidth, webcamRequestedHeight);

        webcamtex.Play();

        Debug.Log("Webcam request width: " + webcamtex.requestedWidth + ". Webcam requested height: " + webcamtex.requestedHeight + ". Webcam device name: " + nextWebcamDeviceName);
    }

    private WebCamDevice GetNextWebCamDevice()
    {
        WebCamDevice[] devices = WebCamTexture.devices;

        WebCamDevice nextDevice;

        int i = deviceIndex + 1;
 
        if (i >= devices.Length)
            i = 0;

        nextDevice = devices[i];
        deviceIndex = i;

        return nextDevice;
    }
}
