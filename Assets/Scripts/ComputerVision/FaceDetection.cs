using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenCVForUnity;

public class FaceDetection : MonoBehaviour {

    WebcamTextureController webcamController;

    /// <summary>
    /// The gray mat.
    /// </summary>
    Mat grayscale;

    /// <summary>
    /// The texture.
    /// </summary>
    public Texture2D testTexture;

    /// <summary>
    /// The cascade.
    /// </summary>
    CascadeClassifier cascade;

    /// <summary>
    /// The faces.
    /// </summary>
    MatOfRect faces;

    public OpenCVForUnity.Rect[] faceLocations;

    public OpenCVForUnity.Rect[] FaceLocations
    {
        get
        {
            if (faceLocations == null)
            {
                Debug.Log("No faces tracked");
                return new OpenCVForUnity.Rect[0];
            }

            else
                return faceLocations;
        }
    }

    // Use this for initialization
    void Start () {
        InitializeCascade();

        InitializeWebcamController();

        int width = webcamController.WebcamTex.width;
        int height = webcamController.WebcamTex.height;

        testTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);

        grayscale = new Mat(height, width, CvType.CV_8UC1);

        faces = new MatOfRect();
    }

    private void InitializeCascade()
    {
        cascade = new CascadeClassifier();
        cascade.load(Utils.getFilePath("lbpcascade_frontalface.xml"));

        if (cascade.empty())
            Debug.LogError("cascade file was not found in “Assets/StreamingAssets/” folder. ");
    }

    private void InitializeWebcamController()
    {
        webcamController = FindObjectOfType<WebcamTextureController>();

        webcamController.Initialize();        
    }

    // Update is called once per frame
    void Update()
    {
        if (webcamController.DidUpdateThisFrame())
        {
            Mat rgbaMat = webcamController.WebcamMat;

            //Photo.fastNlMeansDenoising(rgbaMat, rgbaMat);
            Imgproc.cvtColor(rgbaMat, grayscale, Imgproc.COLOR_RGBA2GRAY);
            Imgproc.equalizeHist(grayscale, grayscale);

            if (cascade != null)
                cascade.detectMultiScale(grayscale, faces, 1.1, 2, 2, // TODO: objdetect.CV_HAAR_SCALE_IMAGE
                    new Size(grayscale.cols() * 0.2, grayscale.rows() * 0.2), new Size());

            faceLocations = faces.toArray();

            Utils.matToTexture2D(grayscale, testTexture, webcamController.Colors);
        }
    }

    /*
    private Rectangle[] CreateTheFaceRects()
    {
        faceLocations = faces.toArray();

        Rectangle[] _faceLocations = new Rectangle[rects.Length];

        for (int i = 0; i < rects.Length; i++)
        {
            int width = rects[i].width;
            int height = rects[i].height;

            Point topLeft = new Point(rects[i].x, rects[i].y);
            Point bottomRight = new Point(rects[i].x + width, rects[i].y + height);

            _faceLocations[i] = new Rectangle(topLeft, bottomRight, width, height);

            //Imgproc.rectangle(rgbMat, new Point(rects[i].x, rects[i].y), new Point(rects[i].x + rects[i].width, rects[i].y + rects[i].height), new Scalar(255, 0, 0, 255), 2);
        }

        return _faceLocations;
    }
    */
    
    /*
    //I think we can just use OpenCV rects
    public struct Rectangle
    {
        private Point topLeft;
        private Point bottomRight;

        public Point TopLeft
        {
            get { return topLeft; }
            private set { topLeft = value; }
        }

        public Point BottomRight
        {
            get { return bottomRight; }
            private set { bottomRight = value; }
        }

        private int width;
        private int height;

        public int Width
        {
            get { return width; }
            private set { width = value; }
        }

        public int Height
        {
            get { return height; }
            private set { height = value; }
        }

        public Rectangle(Point topLeft, Point bottomRight, int width, int height)
        {
            this.topLeft = topLeft;
            this.bottomRight = bottomRight;
            this.width = width;
            this.height = height;
        }
    }
    */
}
