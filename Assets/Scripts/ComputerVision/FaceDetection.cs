using UnityEngine;
using OpenCVForUnity;

public class FaceDetection : MonoBehaviour
{

    WebcamTextureController webcamController;

    /// <summary>
    /// The gray mat.
    /// </summary>
    Mat grayscale;

    /// <summary>
    /// The texture.
    /// </summary>
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

    /// <summary>
    /// The cascade.
    /// </summary>
    CascadeClassifier cascade;

    /// <summary>
    /// The faces.
    /// </summary>
    MatOfRect faces;

    private OpenCVForUnity.Rect[] faceLocations;

    public bool EqualizeTexture;

    public bool DenoiseTexture;

    /*
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
    } */

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

    public void UpdateFaceTexture()
    {
        if (webcamController.DidUpdateThisFrame())
        {
            Mat rgbaMat = webcamController.WebcamMat;

            Imgproc.cvtColor(rgbaMat, grayscale, Imgproc.COLOR_RGB2GRAY);
            Imgproc.equalizeHist(grayscale, grayscale);

            if (cascade != null)
                cascade.detectMultiScale(grayscale, faces, 1.1, 2, 2, // TODO: objdetect.CV_HAAR_SCALE_IMAGE
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
