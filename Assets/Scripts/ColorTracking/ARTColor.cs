using OpenCVForUnity;

namespace ColorTracking
{
    public class ARTColor
    {
        int xPos, yPos;
        string type;
        Scalar Color;

        public const string BLUE = "blue";
        public const string GREEN = "green";
        public const string YELLOW = "yellow";
        public const string RED = "red";

        public ARTColor()
        {
            //set values for default constructor
            setType("Object");
            setColor(new Scalar(0, 0, 0));
        }

        public ARTColor(string name)
        {
            setType(name);

            if (name == BLUE)
            {
                //setColor (new Scalar (0, 0, 255));
                setColor(new Scalar(96, 96, 96));

            }
            if (name == GREEN)
            {
                   //setColor (new Scalar (0, 255, 0));
                setColor(new Scalar(224, 224, 224));
            }
            if (name == YELLOW)
            {
                //setColor (new Scalar (255, 255, 0));
                setColor(new Scalar(160, 160, 160));
            }
            if (name == RED)
            {
                //setColor (new Scalar (255, 0, 0));
                setColor(new Scalar(255, 255, 255));
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