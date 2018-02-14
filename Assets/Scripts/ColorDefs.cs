using OpenCVForUnity;
using UnityEngine;

namespace ARTScripts
{

    public class ColorDefs : MonoBehaviour
    {
		public static Scalar BlueHSVMin = new Scalar(135, 154, 192);
		public static Scalar BlueHSVMax = new Scalar(59, 85, 138);
        public static Scalar BlueColor = new Scalar(0, 0, 255);

        public static Scalar GreemHSVMin = new Scalar(34, 50, 50);
        public static Scalar GreenHSVMax = new Scalar(80, 220, 200);
        public static Scalar GreenColor = new Scalar(0, 255, 0);

        public static Scalar YellowHSVMin = new Scalar(20, 124, 123);
        public static Scalar YellowHSVMax = new Scalar(30, 256, 256);
        public static Scalar YellowColor = new Scalar(255, 255, 0);

        public static Scalar RedHSVMin = new Scalar(198, 128, 143);
        public static Scalar RedHSVMax = new Scalar(157, 67, 76);
        public static Scalar RedColor = new Scalar(255, 0, 0);
    }
}
