using UnityEngine;
using System.Collections;

using OpenCVForUnity;

namespace ARTScripts
{
    public class ARTColorObject
    {
        int xPos, yPos;
        string type;
        Scalar HSVmin, HSVmax;
        Scalar Color;

        public ARTColorObject()
        {
            //set values for default constructor
            setType("Object");
            setColor(new Scalar(0, 0, 0));
        }

        public ARTColorObject(string name)
        {
            setType(name);

            if (name == "blue")
            {
                setHSVmin(new Scalar(105, 100, 100));
                setHSVmax(new Scalar(128, 256, 256));

                setColor (new Scalar (0, 0, 255));
                //setColor(new Scalar(105, 105, 105));

            }
            if (name == "green")
            {
                setHSVmin(new Scalar(40, 100, 100));
                setHSVmax(new Scalar(80, 256, 256));

                setColor (new Scalar (0, 255, 0));
                //setColor(new Scalar(192, 192, 192));
            }
            if (name == "yellow")
            {
                setHSVmin(new Scalar(20, 124, 123));
                setHSVmax(new Scalar(30, 256, 256));

                setColor (new Scalar (255, 255, 0));
                //setColor(new Scalar(169, 169, 169));
            }
            if (name == "red")
            {
                setHSVmin(new Scalar(0, 100, 100));
                setHSVmax(new Scalar(10, 256, 256));

                setColor (new Scalar (255, 0, 0));
                //setColor(new Scalar(220, 220, 220));
            }
        }

        public int getXPos()
        {
            return xPos;
        }

        public void setXPos(int x)
        {
            xPos = x;
        }

        public int getYPos()
        {
            return yPos;
        }

        public void setYPos(int y)
        {
            yPos = y;
        }

        public Scalar getHSVmin()
        {
            return HSVmin;
        }

        public Scalar getHSVmax()
        {
            return HSVmax;
        }

        public void setHSVmin(Scalar min)
        {
            HSVmin = min;
        }

        public void setHSVmax(Scalar max)
        {
            HSVmax = max;
        }

        public string getType()
        {
            return type;
        }

        public void setType(string t)
        {
            type = t;
        }

        public Scalar getColor()
        {
            return Color;
        }

        public void setColor(Scalar c)
        {
            Color = c;
        }
    }
}