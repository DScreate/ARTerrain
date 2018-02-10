using OpenCVForUnity;
using UnityEngine;

public class ColorDefs : MonoBehaviour
{
    public Scalar BlueHSVMin = new Scalar(92, 0, 0);
    public Scalar BlueHSVMax = new Scalar(124, 220, 240);
    public Scalar BlueColor = new Scalar(0, 0, 255);

    public Scalar GreemHSVMin = new Scalar(34, 50, 50);
    public Scalar GreenHSVMax  = new Scalar(80, 220, 200);
    public Scalar GreenColor = new Scalar(0, 255, 0);

    public Scalar YellowHSVMin = new Scalar(20, 124, 123);
    public Scalar YellowHSVMax = new Scalar(30, 256, 256);
    public Scalar YellowColor = new Scalar(255, 255, 0);

    public Scalar RedHSVMin = new Scalar(102, 0, 0);
    public Scalar RedHSVMax = new Scalar(255, 188, 142);
    public Scalar RedColor = new Scalar(255, 0, 0);
}
