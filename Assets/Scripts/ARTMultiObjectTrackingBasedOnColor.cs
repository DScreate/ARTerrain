using UnityEngine;
using System.Collections.Generic;

#if UNITY_5_3 || UNITY_5_3_OR_NEWER
using UnityEngine.SceneManagement;
#endif
using OpenCVForUnity;
using ARTScripts;

/// <summary>
/// Multi Object Tracking Based on Color Example
/// Referring to https://www.youtube.com/watch?v=hQ-bpfdWQh8.
/// Reffering to OpenCV for unity
/// </summary>
[RequireComponent(typeof(ARTWebcamTextureToMatHelper))]
public class ARTMultiObjectTrackingBasedOnColor : MonoBehaviour
{
    /// <summary>
    /// The texture.
    /// </summary>
    Texture2D _texture;

    /// <summary>
    /// max number of objects to be detected in frame
    /// </summary>
    const int MAX_NUM_OBJECTS = 50;

    /// <summary>
    /// minimum and maximum object area. used to reduce noise. if the area is smaller then smaller objects/contours are displayed. as the area increases so does the size of the objects/contours that are displayed.
    /// </summary>
    const int MIN_OBJECT_AREA = 20 * 20;

    /// <summary>
    /// The rgb mat.
    /// </summary>
    Mat _rgbMat;

    /// <summary>
    /// The threshold mat.
    /// </summary>
    Mat _thresholdMat;

    /// <summary>
    /// The hsv mat.
    /// </summary>
    Mat _hsvMat;

    /// <summary>
    /// Used for converting detected colors to a grayscale that can be used by Terrain Generator.
    /// </summary>
    Mat _drawMat;

    ARTColorObject _blue = new ARTColorObject("blue");
    ARTColorObject _yellow = new ARTColorObject("yellow");
    ARTColorObject _red = new ARTColorObject("red");
    ARTColorObject _green = new ARTColorObject("green");

    //REMOVE
    protected Color32[] _colors;

    /// <summary>
    /// The webcam texture to mat helper.
    /// </summary>
    ARTWebcamTextureToMatHelper webCamTextureToMatHelper;

    /// <summary>
    /// Gets the 2dTexture used for terrain generation
    /// </summary>
    public Texture GetTexture()
    {        
        return _texture;
    }

    // Use this for initialization
    void Start()
    {
        webCamTextureToMatHelper = gameObject.GetComponent<ARTWebcamTextureToMatHelper>();
        webCamTextureToMatHelper.Initialize();
    }

    /// <summary>
    /// Raises the webcam texture to mat helper initialized event.
    /// </summary>
    public void OnWebCamTextureToMatHelperInitialized()
    {
        Debug.Log("OnWebCamTextureToMatHelperInitialized");

        Mat webCamTextureMat = webCamTextureToMatHelper.GetMat();

        _texture = new Texture2D(webCamTextureMat.cols(), webCamTextureMat.rows(), TextureFormat.RGBA32, false);

        //REMOVE
        gameObject.GetComponent<Renderer>().material.mainTexture = _texture;
        gameObject.transform.localScale = new Vector3(webCamTextureMat.cols(), webCamTextureMat.rows(), 1);      
        
        float width = webCamTextureMat.width();
        float height = webCamTextureMat.height();

        float widthScale = (float)Screen.width / width;
        float heightScale = (float)Screen.height / height;
        if (widthScale < heightScale)
        {
            Camera.main.orthographicSize = (width * (float)Screen.height / (float)Screen.width) / 2;
        }
        else
        {
            Camera.main.orthographicSize = height / 2;
        }
        //END REMOVE

        Debug.Log("Screen.width " + Screen.width + " Screen.height " + Screen.height + " Screen.orientation " + Screen.orientation);

        _rgbMat = new Mat(webCamTextureMat.rows(), webCamTextureMat.cols(), CvType.CV_8UC3);
        _drawMat = new Mat(webCamTextureMat.rows(), webCamTextureMat.cols(), CvType.CV_8UC3);
        _colors = new Color32[webCamTextureMat.rows() * webCamTextureMat.cols()];
        _thresholdMat = new Mat();
        _hsvMat = new Mat();        
    }

    /// <summary>
    /// Raises the webcam texture to mat helper disposed event.
    /// </summary>
    public void OnWebCamTextureToMatHelperDisposed()
    {
        Debug.Log("OnWebCamTextureToMatHelperDisposed");

        if (_rgbMat != null)
            _rgbMat.Dispose();
        if (_thresholdMat != null)
            _thresholdMat.Dispose();
        if (_hsvMat != null)
            _hsvMat.Dispose();
        if (_drawMat != null)
            _drawMat.Dispose();
    }

    /// <summary>
    /// Raises the webcam texture to mat helper error occurred event.
    /// </summary>
    /// <param name="errorCode">Error code.</param>
    public void OnWebCamTextureToMatHelperErrorOccurred(ARTWebcamTextureToMatHelper.ErrorCode errorCode)
    {
        Debug.Log("OnWebCamTextureToMatHelperErrorOccurred " + errorCode);
    }

    // Update is called once per frame
    void Update()
    {
        if (webCamTextureToMatHelper.IsPlaying() && webCamTextureToMatHelper.DidUpdateThisFrame())
        {
            Mat rgbaMat = webCamTextureToMatHelper.GetMat();
            Mat drawMat = new Mat();
            _drawMat.copyTo(drawMat);

            Imgproc.cvtColor(rgbaMat, _rgbMat, Imgproc.COLOR_RGBA2RGB);          
            
            Imgproc.cvtColor(_rgbMat, _hsvMat, Imgproc.COLOR_RGB2HSV);

            //first find blue objects
            Core.inRange(_hsvMat, _blue.getHSVmin(), _blue.getHSVmax(), _thresholdMat);
            morphOps(_thresholdMat);
            trackFilteredObject(_blue, _thresholdMat, drawMat);
            
            //then yellows
            Core.inRange(_hsvMat, _yellow.getHSVmin(), _yellow.getHSVmax(), _thresholdMat);
            morphOps(_thresholdMat);
            trackFilteredObject(_yellow, _thresholdMat, drawMat);

            //then reds
            Core.inRange(_hsvMat, _red.getHSVmin(), _red.getHSVmax(), _thresholdMat);
            morphOps(_thresholdMat);
            trackFilteredObject(_red, _thresholdMat, drawMat);

            //then greens
            Core.inRange(_hsvMat, _green.getHSVmin(), _green.getHSVmax(), _thresholdMat);
            morphOps(_thresholdMat);
            trackFilteredObject(_green, _thresholdMat, drawMat);

            //Imgproc.putText(_rgbMat, "W:" + _rgbMat.width() + " H:" + _rgbMat.height() + " SO:" + Screen.orientation, new Point(5, _rgbMat.rows() - 10), Core.FONT_HERSHEY_SIMPLEX, 1.0, new Scalar(255, 255, 255, 255), 2, Imgproc.LINE_AA, false);

            //TODO: Change mat so that we are only capturing a grayscale
            Utils.matToTexture2D(drawMat, _texture, _colors);
        }        
    }       
    /// <summary>
    /// Raises the destroy event.
    /// </summary>
    void OnDestroy()
    {
        webCamTextureToMatHelper.Dispose();
    }
    //REMOVE
    /// <summary>
    /// Raises the back button click event.
    /// </summary>
    public void OnBackButtonClick()
    {
#if UNITY_5_3 || UNITY_5_3_OR_NEWER
        SceneManager.LoadScene("OpenCVForUnityExample");
#else
            Application.LoadLevel ("OpenCVForUnityExample");
#endif
    }

    /// <summary>
    /// Raises the play button click event.
    /// </summary>
    public void OnPlayButtonClick()
    {
        webCamTextureToMatHelper.Play();
    }

    /// <summary>
    /// Raises the pause button click event.
    /// </summary>
    public void OnPauseButtonClick()
    {
        webCamTextureToMatHelper.Pause();
    }

    /// <summary>
    /// Raises the stop button click event.
    /// </summary>
    public void OnStopButtonClick()
    {
        webCamTextureToMatHelper.Stop();
    }

    /// <summary>
    /// Raises the change camera button click event.
    /// </summary>
    public void OnChangeCameraButtonClick()
    {
        webCamTextureToMatHelper.Initialize(null, webCamTextureToMatHelper.requestedWidth, webCamTextureToMatHelper.requestedHeight);
    }
    //END REMOVE

    /// <summary>
    /// Draws the object.
    /// </summary>
    /// <param name="theColorObjects">The color objects.</param>
    /// <param name="frame">Frame.</param>
    /// <param name="temp">Temp.</param>
    /// <param name="contours">Contours.</param>
    /// <param name="hierarchy">Hierarchy.</param>
    private void drawObject(List<ARTColorObject> theColorObjects, Mat drawMat, Mat temp, List<MatOfPoint> contours, Mat hierarchy)
    {
        for (int i = 0; i < theColorObjects.Count; i++)
        { 
            Imgproc.drawContours(drawMat, contours, i, theColorObjects[i].getColor(), -1, 8, hierarchy, int.MaxValue, new Point());
        }
    }


    /// <summary>
    /// Morphs the ops.
    /// </summary>
    /// <param name="thresh">Thresh.</param>
    private void morphOps(Mat thresh)
    {
        //create structuring element that will be used to "dilate" and "erode" image.
        //the element chosen here is a 3px by 3px rectangle
        Mat erodeElement = Imgproc.getStructuringElement(Imgproc.MORPH_RECT, new Size(3, 3));
        //dilate with larger element so make sure object is nicely visible
        Mat dilateElement = Imgproc.getStructuringElement(Imgproc.MORPH_RECT, new Size(8, 8));

        //When we don't erode, it's easier to detect colors but there's more noise/less stable.
        //When we decrease size of the erodeElement, it's easier to detect colors but there's more noise.
        Imgproc.erode(thresh, thresh, erodeElement);
        Imgproc.erode(thresh, thresh, erodeElement);

        //When we don't dilate, it becomes harder to detect colors.
        //When we increase size of dilateElement, it becomes easier to detect colors. However, the edges of objects becomes blockier and less defined.
        Imgproc.dilate(thresh, thresh, dilateElement);
        Imgproc.dilate(thresh, thresh, dilateElement);
    }

    /// <summary>
    /// Tracks the filtered object.
    /// </summary>
    /// <param name="theColorObject">The color object.</param>
    /// <param name="threshold">Threshold.</param>
    /// <param name="HSV">HS.</param>
    /// <param name="drawMat">The mat that we draw onto.</param>
    private void trackFilteredObject(ARTColorObject theColorObject, Mat threshold, Mat drawMat)
    {
        List<ARTColorObject> colorObjects = new List<ARTColorObject>();
        Mat temp = new Mat();
        threshold.copyTo(temp);
        //these two vectors needed for output of findContours
        List<MatOfPoint> contours = new List<MatOfPoint>();
        Mat hierarchy = new Mat();
        //find contours of filtered image using openCV findContours function
        //from OpenCV docs:
        //contours: detected contours stored as a vector of points
        //hierarchy: output vector, containing information about the image topology. has as many elements
        //as the number of contours.
        Imgproc.findContours(temp, contours, hierarchy, Imgproc.RETR_CCOMP, Imgproc.CHAIN_APPROX_SIMPLE);

        //use moments method to find our filtered object
        bool colorObjectFound = false;
        if (hierarchy.rows() > 0)
        {
            int numObjects = hierarchy.rows();

            //if number of objects/contours greater than MAX_NUM_OBJECTS we have a noisy filter
            if (numObjects < MAX_NUM_OBJECTS)
            {
                for (int index = 0; index >= 0; index = (int)hierarchy.get(0, index)[0])
                {

                    Moments moment = Imgproc.moments(contours[index]);
                    //gets the area of the current contour. contours are curves joining all the continuous points, having the same color or intensity. 
                    double area = moment.get_m00();

                    //if the area is less than 20 px by 20px then it is probably just noise
                    //if the area is the same as the 3/2 of the image size, probably just a bad filter
                    //we only want the object with the largest area so we safe a reference area each
                    //iteration and compare it to the area in the next iteration.
                    if (area > MIN_OBJECT_AREA)
                    {
                        ARTColorObject colorObject = new ARTColorObject();

                        colorObject.setXPos((int)(moment.get_m10() / area));
                        colorObject.setYPos((int)(moment.get_m01() / area));
                        colorObject.setType(theColorObject.getType());
                        colorObject.setColor(theColorObject.getColor());

                        colorObjects.Add(colorObject);

                        colorObjectFound = true;
                    }
                    else
                    {
                        colorObjectFound = false;
                    }
                }
                //let user know you found an object
                if (colorObjectFound == true)
                {
                    //draw object location on screen
                    drawObject(colorObjects, drawMat, temp, contours, hierarchy);
                }

            }
            else
            {
                Debug.Log("Too much noise on drawMat.");               
            }
        }
    }
}