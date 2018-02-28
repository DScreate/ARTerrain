using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenCVForUnity;
namespace ColorTracking { 
    public class ARTColorDefs : MonoBehaviour {
        public Scalar BlueHSVmin = new Scalar(105, 100, 100);
        public Scalar BlueHSVmax = new Scalar(128, 256, 256);
        Scalar BlueRGB = new Scalar(96, 96, 96);

        public Scalar GreenHSVmin = new Scalar(40, 100, 100);
        public Scalar GreenHSVmax = new Scalar(80, 256, 256);
        Scalar GreenRGB = new Scalar(224, 224, 224);

        public Scalar YellowHSVmin = new Scalar(20, 124, 123);
        public Scalar YellowHSVmax = new Scalar(30, 256, 256);
        Scalar YellowRGB = new Scalar(160, 160, 160);

        public Scalar RedHSVmin = new Scalar(0, 100, 100);
        public Scalar RedHSVmax = new Scalar(10, 256, 256);
        Scalar RedRGB = new Scalar(255, 255, 255);   
        
        public Scalar getHSVMin(string type)
        {
            if (type == ARTColor.BLUE)
                return BlueHSVmin;

            else if (type == ARTColor.GREEN)
                return GreenHSVmin;

            else if (type == ARTColor.YELLOW)
                return YellowHSVmin;

            else if (type == ARTColor.RED)
                return RedHSVmin;
            
            else
            {
                Debug.Log(type + " is not a valid color type.");
                return new Scalar(0, 0, 0);
            }
        }

        public Scalar getHSVMax(string type)
        {
            if (type == ARTColor.BLUE)
                return BlueHSVmax;

            else if (type == ARTColor.GREEN)
                return GreenHSVmax;

            else if (type == ARTColor.YELLOW)
                return YellowHSVmax;

            else if (type == ARTColor.RED)
                return RedHSVmax;

            else
            {
                Debug.Log(type + " is not a valid color type.");
                return new Scalar(0, 0, 0);
            }
        }

        public Scalar getRGB(string type)
        {
            if (type == ARTColor.BLUE)
                return BlueRGB;

            else if (type == ARTColor.GREEN)
                return GreenRGB;

            else if (type == ARTColor.YELLOW)
                return YellowRGB;

            else if (type == ARTColor.RED)
                return RedRGB;

            else
            {
                Debug.Log(type + " is not a valid color type.");
                return new Scalar(0, 0, 0);
            }
        }
    }
}
