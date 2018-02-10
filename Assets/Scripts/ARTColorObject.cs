using UnityEngine;
using OpenCVForUnity;

public class ARTColorObject : MonoBehaviour {

    int xPos, yPos;
    string type;
    Scalar HSVmin, HSVmax;
    Scalar Color;
    ColorDefs ColorDefs;

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

            setHSVmin(ColorDefs.BlueHSVMin);
            setHSVmax(ColorDefs.BlueHSVMin);

            //BGR value for Blue:
            setColor(ColorDefs.BlueColor);

        }
        if (name == "green")
        {

            //TODO: use "calibration mode" to find HSV min
            //and HSV max values

            setHSVmin(ColorDefs.GreemHSVMin);
            setHSVmax(ColorDefs.GreenHSVMax);

            //BGR value for Green:
            setColor(ColorDefs.GreenColor);

        }
        if (name == "yellow")
        {

            //TODO: use "calibration mode" to find HSV min
            //and HSV max values

            setHSVmin(ColorDefs.YellowHSVMin);
            setHSVmax(ColorDefs.YellowHSVMax);

            //BGR value for Yellow:
            setColor(ColorDefs.YellowColor);

        }
        if (name == "red")
        {

            //TODO: use "calibration mode" to find HSV min
            //and HSV max values

            setHSVmin(ColorDefs.RedHSVMin);
            setHSVmax(ColorDefs.RedHSVMax);

            //BGR value for Red:
            setColor(ColorDefs.RedColor);

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

