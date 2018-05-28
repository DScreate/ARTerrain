using UnityEngine;
using OpenCVForUnity;
/// <summary>
/// This class is used to detect and outline faces in the webcam image. 
/// It also equalizes its histograms and (optionally) denoises the image.
/// </summary>
/// <remarks>
/// This is the only part of the project that uses OpenCVForUnity.
/// Code is modified from OpenCVForUnity example class FaceDetectionWebCamTextureExample.cs.
/// </remarks>
public class FaceDetection : MonoBehaviour
{
    WebcamTextureController webcamController;
 
    //Class variable to reduce garbage collection.
    Mat grayscale;

    private Texture2D faceTexture;

    public Texture2D FaceTexture
    {
        get
        {
            if (faceTexture == null)
            {
                Debug.Log("faceTexture null");
                return new Texture2D(1, 1);
            }
            return faceTexture;
        }
    }

    CascadeClassifier cascade;

    MatOfRect faces;

    private OpenCVForUnity.Rect[] faceLocations;

    [Tooltip("Should WebcamTexture be equalized?")]
    public bool EqualizeTexture;
    [Tooltip("Should WebcamTexture be denoised? Greatly reduces fps.")]
    public bool DenoiseTexture;

    /// <summary>
    /// Initializes the class variables.
    /// </summary>
    void Start()
    {
        InitializeCascade();

        InitializeWebcamController();

        int width = webcamController.WebcamTex.width;
        int height = webcamController.WebcamTex.height;

        faceTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);

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
        webcamController = gameObject.GetComponent(typeof(WebcamTextureController)) as WebcamTextureController;

        webcamController.Initialize();
    }
    /// <summary>
    /// Detects and outlines faces in the WebcamTexture and stored as a Mat.
    /// Optionally equalizes and denoises WebcamTexture.
    /// </summary>
    public void UpdateFaceTexture()
    {
        if (webcamController.DidUpdateThisFrame())
        {
            Mat rgbaMat = webcamController.WebcamMat;

            Imgproc.cvtColor(rgbaMat, grayscale, Imgproc.COLOR_RGB2GRAY);
            Imgproc.equalizeHist(grayscale, grayscale);

            if (cascade != null)
                cascade.detectMultiScale(grayscale, faces, 1.1, 2, 2,
                    new Size(grayscale.cols() * 0.2, grayscale.rows() * 0.2), new Size());

            faceLocations = faces.toArray();

            if (DenoiseTexture)
                Photo.fastNlMeansDenoising(grayscale, grayscale);

            for (int i = 0; i < faceLocations.Length; i++)
            {
                if (EqualizeTexture)
                    Imgproc.rectangle(grayscale, new Point(faceLocations[i].x, faceLocations[i].y), new Point(faceLocations[i].x + faceLocations[i].width, faceLocations[i].y + faceLocations[i].height), new Scalar(255, 255, 255, 255), 5);

                else
                    Imgproc.rectangle(rgbaMat, new Point(faceLocations[i].x, faceLocations[i].y), new Point(faceLocations[i].x + faceLocations[i].width, faceLocations[i].y + faceLocations[i].height), new Scalar(255, 255, 255, 255), 5);
            }

            if (EqualizeTexture)
                Utils.matToTexture2D(grayscale, faceTexture, webcamController.Colors);

            else
                Utils.matToTexture2D(rgbaMat, faceTexture, webcamController.Colors);
        }
    }
}
