using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenCVForUnity;
using System;

namespace ComputerVision
{
    public class ObjectTrackingBasedOnColor
    {

        /// <summary>
        /// Grayscale Mat that is converted to a Texture2D, which Unity can use.
        /// </summary>
        Texture2D _grayscaleTexture;

        /// <summary>
        /// max number of objects to be detected in frame
        /// </summary>
        const int MAX_NUM_OBJECTS = 50;

        /// <summary>
        /// minimum and maximum object area. used to reduce noise. if the area is smaller then smaller objects/contours are displayed. as the area increases so does the size of the objects/contours that are displayed.
        /// </summary>
        const int MIN_OBJECT_AREA = 20 * 20;

        /// <summary>
        /// Mat that stores the WebCamTexture
        /// </summary>
        Mat _webCamMat;

        /// <summary>
        /// The threshold mat.
        /// </summary>
        Mat _threshold;

        /// <summary>
        /// The hsv mat.
        /// </summary>
        Mat _hsv;

        /// <summary>
        /// Used to set the grayscale Mat in UpdateGrayScaleTexture()
        /// </summary>
        Mat _grayscale;

        /// <summary>
        /// Webcam stream
        /// </summary>
        WebCamTexture _webCamStream;

        ARTColor _blue = new ARTColor("blue");
        ARTColor _yellow = new ARTColor("yellow");
        ARTColor _red = new ARTColor("red");
        ARTColor _green = new ARTColor("green");

        /// <summary>
        /// Used as input for some methods in order to save those methods from having to create their own Colors32 array
        /// </summary>
        Color32[] _colorsUsedToSaveMemory;

        public ObjectTrackingBasedOnColor(WebCamTexture webCamStream)
        {
            if (webCamStream == null)
                throw new ArgumentNullException("WebCamTexture cannot be null");

            _webCamStream = webCamStream;            

            _webCamMat = new Mat(webCamStream.height, webCamStream.width, CvType.CV_8UC3);

            _colorsUsedToSaveMemory = new Color32[webCamStream.width * webCamStream.height];

            Utils.webCamTextureToMat(webCamStream, _webCamMat, _colorsUsedToSaveMemory);            

            _grayscale = new Mat(webCamStream.height, webCamStream.width, CvType.CV_8UC3, new Scalar(0,0,0));

            _threshold = new Mat();
            _hsv = new Mat();

            _grayscaleTexture = new Texture2D(webCamStream.width, webCamStream.height, TextureFormat.RGB24, false);
        }
        //This will be used for returning a specific texture that only detects colors depending type passed in
        public Texture2D GetTerrainTexture(String type)
        {
            //TODO: create methods that will update textures for each type
            if (type == "Sand")
                return null;

            else if (type == "Grass")
                return null;

            else if (type == "Mountain")
                return null;

            else if (type == "Sand")
                return null;

            else
            {
                Debug.Log("Incorrect type.");
                return new Texture2D(_webCamMat.cols(), _webCamMat.rows());
            }
        }
        #region Update grayscale texture
        /// <summary>
        /// Returns Texture2D that contains a grayscale of _webCamStream based off colors we are detecting and displaying.
        /// </summary>
        public Texture2D UpdateGrayScale()
        {
            Utils.webCamTextureToMat(_webCamStream, _webCamMat, _colorsUsedToSaveMemory);

            Imgproc.cvtColor(_webCamMat, _hsv, Imgproc.COLOR_RGB2HSV);

            var tempGrayscale = new Mat();
            _grayscale.copyTo(tempGrayscale);

            //first find blue contours
            Core.inRange(_hsv, _blue.getHSVmin(), _blue.getHSVmax(), _threshold);
            morphOps(_threshold);
            trackFilteredObject(_blue, _threshold, tempGrayscale);

            //then yellows
            Core.inRange(_hsv, _yellow.getHSVmin(), _yellow.getHSVmax(), _threshold);
            morphOps(_threshold);
            trackFilteredObject(_yellow, _threshold, tempGrayscale);

            //then reds
            Core.inRange(_hsv, _red.getHSVmin(), _red.getHSVmax(), _threshold);
            morphOps(_threshold);
            trackFilteredObject(_red, _threshold, tempGrayscale);

            //then greens
            Core.inRange(_hsv, _green.getHSVmin(), _green.getHSVmax(), _threshold);
            morphOps(_threshold);
            trackFilteredObject(_green, _threshold, tempGrayscale);

            //TODO: Change mat so that we are only capturing a tempGrayscale
            Utils.matToTexture2D(tempGrayscale, _grayscaleTexture, _colorsUsedToSaveMemory);

            return _grayscaleTexture;
        }

        /// <summary>
        /// Returns Texture2D that contains a grayscale of src based off colors we are detecting and displaying.
        /// </summary>
        /// <param name="src"></param>
        public Texture2D UpdateGrayScale(WebCamTexture src)
        {
            if (_webCamMat.cols() != src.width || _webCamMat.rows() != src.height)
                _webCamMat = new Mat(src.height, src.width, CvType.CV_8UC3);

            if (_colorsUsedToSaveMemory.Length != src.width * src.height)
                _colorsUsedToSaveMemory = new Color32[src.width * src.height];

            Utils.webCamTextureToMat(src, _webCamMat, _colorsUsedToSaveMemory);

            Imgproc.cvtColor(_webCamMat, _hsv, Imgproc.COLOR_RGB2HSV);

            if (_grayscale.cols() != src.width || _grayscale.rows() != src.height)
                _grayscale = new Mat(src.height, src.width, CvType.CV_8UC3);

            var tempGrayscale = new Mat();
            _grayscale.copyTo(tempGrayscale);

            //first find blue contours
            Core.inRange(_hsv, _blue.getHSVmin(), _blue.getHSVmax(), _threshold);
            morphOps(_threshold);
            trackFilteredObject(_blue, _threshold, tempGrayscale);

            //then yellows
            Core.inRange(_hsv, _yellow.getHSVmin(), _yellow.getHSVmax(), _threshold);
            morphOps(_threshold);
            trackFilteredObject(_yellow, _threshold, tempGrayscale);

            //then reds
            Core.inRange(_hsv, _red.getHSVmin(), _red.getHSVmax(), _threshold);
            morphOps(_threshold);
            trackFilteredObject(_red, _threshold, tempGrayscale);

            //then greens
            Core.inRange(_hsv, _green.getHSVmin(), _green.getHSVmax(), _threshold);
            morphOps(_threshold);
            trackFilteredObject(_green, _threshold, tempGrayscale);

            //TODO: Change mat so that we are only capturing a tempGrayscale
            Utils.matToTexture2D(tempGrayscale, _grayscaleTexture, _colorsUsedToSaveMemory);

            return _grayscaleTexture;
        }

        /// <summary>
        /// Modifies dst so that it contains a grayscale of src based off colors we are detecting and displaying.
        /// </summary>
        public void UpdateGrayScale(WebCamTexture src, Texture2D dst)
        {
            if (_webCamMat.cols() != src.width || _webCamMat.rows() != src.height)
                _webCamMat = new Mat(src.height, src.width, CvType.CV_8UC3);

            if (_colorsUsedToSaveMemory.Length != src.width * src.height)
                _colorsUsedToSaveMemory = new Color32[src.width * src.height];

            Utils.webCamTextureToMat(src, _webCamMat, _colorsUsedToSaveMemory);

            Imgproc.cvtColor(_webCamMat, _hsv, Imgproc.COLOR_RGB2HSV);

            if (_grayscale.cols() != src.width || _grayscale.rows() != src.height)
                _grayscale = new Mat(src.height, src.width, CvType.CV_8UC3);

            var tempGrayscale = new Mat();
            _grayscale.copyTo(tempGrayscale);

            //first find blue contours
            Core.inRange(_hsv, _blue.getHSVmin(), _blue.getHSVmax(), _threshold);
            morphOps(_threshold);
            trackFilteredObject(_blue, _threshold, tempGrayscale);

            //then yellows
            Core.inRange(_hsv, _yellow.getHSVmin(), _yellow.getHSVmax(), _threshold);
            morphOps(_threshold);
            trackFilteredObject(_yellow, _threshold, tempGrayscale);

            //then reds
            Core.inRange(_hsv, _red.getHSVmin(), _red.getHSVmax(), _threshold);
            morphOps(_threshold);
            trackFilteredObject(_red, _threshold, tempGrayscale);

            //then greens
            Core.inRange(_hsv, _green.getHSVmin(), _green.getHSVmax(), _threshold);
            morphOps(_threshold);
            trackFilteredObject(_green, _threshold, tempGrayscale);

            //TODO: Change mat so that we are only capturing a tempGrayscale
            Utils.matToTexture2D(tempGrayscale, dst, _colorsUsedToSaveMemory);
        }
        #endregion

        /// <summary>
        /// Draws the object.
        /// </summary>
        /// <param name="theColorObjects">The color objects.</param>
        /// <param name="frame">Frame.</param>
        /// <param name="temp">Temp.</param>
        /// <param name="contours">Contours.</param>
        /// <param name="hierarchy">Hierarchy.</param>
        private void fillContours(List<ARTColor> theColorObjects, Mat grayscale, List<MatOfPoint> contours, Mat hierarchy)
        {
            for (int i = 0; i < theColorObjects.Count; i++)
            {
                Imgproc.drawContours(grayscale, contours, i, theColorObjects[i].getColor(), -1, 8, hierarchy, int.MaxValue, new Point());
            }
        }


        /// <summary>
        /// Morph operations.
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
        /// <param name="grayscale">The mat that we draw onto.</param>
        private void trackFilteredObject(ARTColor theColorObject, Mat threshold, Mat grayscale)
        {
            List<ARTColor> colorObjects = new List<ARTColor>();
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
                            ARTColor colorObject = new ARTColor();

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
                        fillContours(colorObjects, grayscale, contours, hierarchy);
                    }

                }
                else
                {
                    Debug.Log("Too much noise on grayscale.");
                }
            }
        }
    }
}
