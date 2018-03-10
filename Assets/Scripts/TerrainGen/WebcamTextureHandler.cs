using System;
using UnityEngine;

public class WebcamTextureHandler : MonoBehaviour {

    public string requestedDeviceName;

    public int webcamRequestedWidth;
    public int webcamRequestedHeight;

    public WebCamTexture webcamtex;

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
                    break;
                }
            }
        }

        if (webcamtex == null)
            webcamtex = new WebCamTexture(webcamRequestedWidth, webcamRequestedHeight);        

        webcamRequestedWidth = webcamtex.requestedWidth;
        webcamRequestedHeight = webcamtex.requestedHeight;

        webcamtex.Play();

        Debug.Log("Webcam request width: " + webcamRequestedWidth + ". Webcam requested height: " + webcamRequestedHeight);
    }
}
