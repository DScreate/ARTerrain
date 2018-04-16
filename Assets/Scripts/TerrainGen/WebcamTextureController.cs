using System;
using UnityEngine;
using OpenCVForUnity;

public class WebcamTextureController : MonoBehaviour {

    public string requestedDeviceName;

    public int webcamRequestedWidth = 640;
    public int webcamRequestedHeight = 480;

    private WebCamTexture webcamTexture;

    private int deviceIndex;

    private bool initialized = false;

    //destination mat for changing webcam texture to opencv mat
    private Mat webcamMat;

    //used to save memory
    private Color32[] colors;

    public WebCamTexture WebcamTex
    {
        get { return webcamTexture; }
    }

    public Mat WebcamMat
    {
        get {
            if(webcamTexture.didUpdateThisFrame)
                Utils.webCamTextureToMat(webcamTexture, webcamMat, colors);

            return webcamMat;
        }
    }

    public Color32[] Colors
    {
        get { return colors; }
    }

    public void Initialize()
    {
        if (!initialized)
        {
            InitializeWebcamTexture();            

            //webcamRequestedWidth = webcamtex.requestedWidth;
            //webcamRequestedHeight = webcamtex.requestedHeight;

            webcamTexture.Play();

            webcamMat = new Mat(webcamTexture.height, webcamTexture.width, CvType.CV_8UC4);

            initialized = true;

            Debug.Log("Webcam request width: " + webcamTexture.requestedWidth + ". Webcam requested height: " + webcamTexture.requestedHeight);
        }
    }

    private void InitializeWebcamTexture()
    {        
        if (!String.IsNullOrEmpty(requestedDeviceName))
            InitializeWebcamTextureWithDeviceName();

        if (webcamTexture == null)
            InitializeWebcamTextureWithDefaultDevice();
    }

    private void InitializeWebcamTextureWithDeviceName()
    {
        WebCamDevice[] devices = WebCamTexture.devices;

        for (int i = 0; i < devices.Length; i++)
        {
            if (devices[i].name == requestedDeviceName)
            {
                webcamTexture = new WebCamTexture(requestedDeviceName, webcamRequestedWidth, webcamRequestedHeight);
                deviceIndex = i;
                break;
            }
        }
    }

    private void InitializeWebcamTextureWithDefaultDevice()
    {
        webcamTexture = new WebCamTexture(webcamRequestedWidth, webcamRequestedHeight);
        deviceIndex = 0;
    }

    //Will probably get weird behavior here if width and height are different from before
    public void ChangeWebcamTextureToNextAvailable()
    {
        string nextWebcamDeviceName = GetNextWebCamDevice().name;

        webcamTexture.Stop();

        webcamTexture = new WebCamTexture(nextWebcamDeviceName, webcamRequestedWidth, webcamRequestedHeight);

        webcamTexture.Play();

        Debug.Log("Webcam request width: " + webcamTexture.requestedWidth + ". Webcam requested height: " + webcamTexture.requestedHeight + ". Webcam device name: " + nextWebcamDeviceName);
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

    public bool DidUpdateThisFrame()
    {
        return (initialized) ? webcamTexture.didUpdateThisFrame : false;
    }
}
