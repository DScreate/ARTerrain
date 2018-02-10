using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using OpenCVForUnity;

public class ARTColorObject : MonoBehaviour {

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

            //TODO: use "calibration mode" to find HSV min
            //and HSV max values

            setHSVmin(new Scalar(0, 0, 0));
            setHSVmax(new Scalar(255, 220, 240));

            //BGR value for Blue:
            setColor(new Scalar(0, 0, 255));

        }
        if (name == "green")
        {

            //TODO: use "calibration mode" to find HSV min
            //and HSV max values

            setHSVmin(new Scalar(34, 50, 50));
            setHSVmax(new Scalar(80, 220, 200));

            //BGR value for Green:
            setColor(new Scalar(0, 255, 0));

        }
        if (name == "yellow")
        {

            //TODO: use "calibration mode" to find HSV min
            //and HSV max values

            setHSVmin(new Scalar(20, 124, 123));
            setHSVmax(new Scalar(30, 256, 256));

            //BGR value for Yellow:
            setColor(new Scalar(255, 255, 0));

        }
        if (name == "red")
        {

            //TODO: use "calibration mode" to find HSV min
            //and HSV max values

            setHSVmin(new Scalar(102, 0, 0));
            setHSVmax(new Scalar(255, 188, 142));

            //BGR value for Red:
            setColor(new Scalar(255, 0, 0));

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

