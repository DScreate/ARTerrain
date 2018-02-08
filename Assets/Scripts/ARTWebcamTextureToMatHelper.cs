using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using OpenCVForUnity;
using System;

/*
 * Reference WebCamTextureToMatHelper.cs from OpenCVForUnity Package Ahmad Adnan Kaifi
 */


public class ARTWebcamTextureToMatHelper : MonoBehaviour {

    /// <summary>
    /// Set the name of the device to use.
    /// </summary>
    [TooltipAttribute("Set the name of the device to use.")]
    public string requestedDeviceName = null;

    /// <summary>
    /// Set the width of WebCamTexture.
    /// </summary>
    [TooltipAttribute("Set the width of WebCamTexture.")]
    public int requestedWidth = 640;

    /// <summary>
    /// Set the height of WebCamTexture.
    /// </summary>
    [TooltipAttribute("Set the height of WebCamTexture.")]
    public int requestedHeight = 480;

    /// <summary>
    /// Set FPS of WebCamTexture.
    /// </summary>
    [TooltipAttribute("Set FPS of WebCamTexture.")]
    public int requestedFPS = 30;

    /// <summary>
    /// UnityEvent that is triggered when this instance is initialized.
    /// </summary>
    public UnityEvent onInitialized;

    /// <summary>
    /// UnityEvent that is triggered when this instance is disposed.
    /// </summary>
    public UnityEvent onDisposed;

    /// <summary>
    /// UnityEvent that is triggered when this instance is error Occurred.
    /// </summary>
    public ErrorUnityEvent onErrorOccurred;

    /// <summary>
    /// The webcam texture.
    /// </summary>
    protected WebCamTexture webCamTexture;

    /// <summary>
    /// The webcam device.
    /// </summary>
    protected WebCamDevice webCamDevice;

    /// <summary>
    /// The rgba mat.
    /// </summary>
    protected Mat rgbaMat;
    
    /*
    /// <summary>
    /// The rotated rgba mat
    /// </summary>
    protected Mat rotatedRgbaMat;
    */

    /// <summary>
    /// The buffer colors.
    /// </summary>
    protected Color32[] colors;

    /// <summary>
    /// Indicates whether this instance is waiting for initialization to complete.
    /// </summary>
    protected bool isInitWaiting = false;

    /// <summary>
    /// Indicates whether this instance has been initialized.
    /// </summary>
    protected bool hasInitDone = false;

    [System.Serializable]
    public enum ErrorCode : int
    {
        CAMERA_DEVICE_NOT_EXIST = 0,
        TIMEOUT = 1,
    }

    [System.Serializable]
    public class ErrorUnityEvent : UnityEngine.Events.UnityEvent<ErrorCode>
    {

    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (hasInitDone)
        {
            StartCoroutine(_Initialize());
        }
    }

    /// <summary>
    /// Raises the destroy event.
    /// </summary>
    protected virtual void OnDestroy()
    {
        Dispose();
    }

    /// <summary>
    /// Initializes this instance.
    /// </summary>
    public virtual void Initialize()
    {
        if (isInitWaiting)
            return;

        if (onInitialized == null)
            onInitialized = new UnityEvent();
        if (onDisposed == null)
            onDisposed = new UnityEvent();

        StartCoroutine(_Initialize());
    }

    /// <summary>
    /// Initializes this instance.
    /// </summary>
    /// <param name="deviceName">Device name.</param>
    /// <param name="requestedWidth">Requested width.</param>
    /// <param name="requestedHeight">Requested height.</param>
    /// <param name="requestedIsFrontFacing">If set to <c>true</c> requested to using the front camera.</param>
    /// <param name="requestedFPS">Requested FPS.</param>
    public virtual void Initialize(string deviceName, int requestedWidth, int requestedHeight, int requestedFPS = 30)
    {
        if (isInitWaiting)
            return;

        this.requestedDeviceName = deviceName;
        this.requestedWidth = requestedWidth;
        this.requestedHeight = requestedHeight;
        this.requestedFPS = requestedFPS;
        if (onInitialized == null)
            onInitialized = new UnityEvent();
        if (onDisposed == null)
            onDisposed = new UnityEvent();

        StartCoroutine(_Initialize());
    }

    /// <summary>
    /// Initializes this instance by coroutine.
    /// </summary>
    protected virtual IEnumerator _Initialize()
    {
        if (hasInitDone)
            _Dispose();

        isInitWaiting = true;

        if (!String.IsNullOrEmpty(requestedDeviceName))
        {
            webCamTexture = new WebCamTexture(requestedDeviceName, requestedWidth, requestedHeight, requestedFPS);
        }
        else
        {
            // Checks how many and which cameras are available on the device
            for (int cameraIndex = 0; cameraIndex < WebCamTexture.devices.Length; cameraIndex++)
            {
                if (WebCamTexture.devices[cameraIndex].isFrontFacing == requestedIsFrontFacing)
                {

                    webCamDevice = WebCamTexture.devices[cameraIndex];
                    webCamTexture = new WebCamTexture(webCamDevice.name, requestedWidth, requestedHeight, requestedFPS);

                    break;
                }
            }
        }

        if (webCamTexture == null)
        {
            if (WebCamTexture.devices.Length > 0)
            {
                webCamDevice = WebCamTexture.devices[0];
                webCamTexture = new WebCamTexture(webCamDevice.name, requestedWidth, requestedHeight, requestedFPS);
            }
            else
            {
                isInitWaiting = false;

                if (onErrorOccurred != null)
                    onErrorOccurred.Invoke(ErrorCode.CAMERA_DEVICE_NOT_EXIST);

                yield break;
            }
        }

        // Starts the camera
        webCamTexture.Play();

        int initFrameCount = 0;
        bool isTimeout = false;

        while (true)
        {
            if (initFrameCount > timeoutFrameCount)
            {
                isTimeout = true;
                break;
            }
            // If you want to use webcamTexture.width and webcamTexture.height on iOS, you have to wait until webcamTexture.didUpdateThisFrame == 1, otherwise these two values will be equal to 16. (http://forum.unity3d.com/threads/webcamtexture-and-error-0x0502.123922/)
#if UNITY_IOS && !UNITY_EDITOR && (UNITY_4_6_3 || UNITY_4_6_4 || UNITY_5_0_0 || UNITY_5_0_1)
                else if (webCamTexture.width > 16 && webCamTexture.height > 16) {
#else
            else if (webCamTexture.didUpdateThisFrame)
            {
#if UNITY_IOS && !UNITY_EDITOR && UNITY_5_2
                    while (webCamTexture.width <= 16) {
                        if (initFrameCount > timeoutFrameCount) {
                            isTimeout = true;
                            break;
                        }else {
                            initFrameCount++;
                        }
                        webCamTexture.GetPixels32 ();
                        yield return new WaitForEndOfFrame ();
                    }
                    if (isTimeout) break;
#endif
#endif

                Debug.Log("name " + webCamTexture.name + " width " + webCamTexture.width + " height " + webCamTexture.height + " fps " + webCamTexture.requestedFPS);
                Debug.Log("videoRotationAngle " + webCamTexture.videoRotationAngle + " videoVerticallyMirrored " + webCamTexture.videoVerticallyMirrored + " isFrongFacing " + webCamDevice.isFrontFacing);

                if (colors == null || colors.Length != webCamTexture.width * webCamTexture.height)
                    colors = new Color32[webCamTexture.width * webCamTexture.height];

                rgbaMat = new Mat(webCamTexture.height, webCamTexture.width, CvType.CV_8UC4);
                screenOrientation = Screen.orientation;

#if !UNITY_EDITOR && !(UNITY_STANDALONE || UNITY_WEBGL)
                    if (screenOrientation == ScreenOrientation.Portrait || screenOrientation == ScreenOrientation.PortraitUpsideDown) {
                        rotatedRgbaMat = new Mat (webCamTexture.width, webCamTexture.height, CvType.CV_8UC4);
                    }
#endif

                if (requestedRotate90Degree)
                {
                    if (rotatedRgbaMat == null)
                        rotatedRgbaMat = new Mat(webCamTexture.width, webCamTexture.height, CvType.CV_8UC4);
                }

                isInitWaiting = false;
                hasInitDone = true;

                if (onInitialized != null)
                    onInitialized.Invoke();

                break;
            }
            else
            {
                initFrameCount++;
                yield return 0;
            }
        }

        if (isTimeout)
        {
            webCamTexture.Stop();
            webCamTexture = null;
            isInitWaiting = false;

            if (onErrorOccurred != null)
                onErrorOccurred.Invoke(ErrorCode.TIMEOUT);
        }
    }

    // Use this for initialization
    void Start () {
		
	}
	
}
