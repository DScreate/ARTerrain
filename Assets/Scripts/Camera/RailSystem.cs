using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public enum Mode
{
    Linear,
    Catmull,
    Insta,
}

[ExecuteInEditMode]
public class RailSystem : MonoBehaviour {

    private Transform[] railNodes;
	// Use this for initialization
	void Start () {
        railNodes = GetComponentsInChildren<Transform>();
 
	}

    public Vector3 PositionOnRailSystem(int seg, float ratio, Mode playMode)
    {
        switch (playMode)
        {
            default:
            case Mode.Linear:
                return LinearPosition(seg, ratio);
            case Mode.Catmull:
                return CatmullPosition(seg, ratio);
            case Mode.Insta:
                return InstaPosition(seg,ratio);
        }
    }

    private Vector3 InstaPosition(int seg, float ratio)
    {
        Debug.Log(ratio);
        return railNodes[seg + 1].position;
    }

    public Quaternion Orientation(int seg, float ratio)
    {
        Quaternion quaterOne = railNodes[seg].rotation;
        Quaternion quaterTwo = railNodes[seg + 1].rotation;

        return Quaternion.Lerp(quaterOne, quaterTwo, ratio);
    }
    public Transform[] getRailNodes()
    {
        return railNodes;
    }

    private Vector3 CatmullPosition(int seg, float ratio)
    {
        Vector3 positionOne, positionTwo, positionThree, positionFour;

        if(seg == 0)
        {
            positionOne = railNodes[seg].position;
            positionTwo = positionOne;
            positionThree = railNodes[seg + 1].position;
            positionFour = railNodes[seg + 2].position;
        }
        else if(seg == railNodes.Length - 2)
        {
            positionOne = railNodes[seg - 1].position;
            positionTwo = railNodes[seg].position;
            positionThree = railNodes[seg + 1].position;
            positionFour = positionThree;
        }
        else
        {
            positionOne = railNodes[seg - 1].position;
            positionTwo = railNodes[seg].position;
            positionThree = railNodes[seg + 1].position;
            positionFour = railNodes[seg + 2].position;
        }

        float tPowerTwo = ratio * ratio;
        float tPowerThree = tPowerTwo * ratio;

        float x = 0.5f * ((positionTwo.x * 2.0f) + (-positionOne.x + positionThree.x) * ratio + 
            ((positionOne.x * 2.0f) - (positionTwo.x * 5.0f) + (positionThree.x * 4.0f) - positionFour.x) * tPowerTwo + 
            (-positionOne.x + (positionTwo.x * 3.0f) - (positionThree.x * 3.0f) + positionFour.x) * (tPowerThree));


        float y = 0.5f * ((positionTwo.y * 2.0f) + (-positionOne.y + positionThree.y) * ratio +
            ((positionOne.y * 2.0f) - (positionTwo.y * 5.0f) + (positionThree.y * 4.0f) - positionFour.y) * tPowerTwo +
            (-positionOne.y + (positionTwo.y * 3.0f) - (positionThree.y * 3.0f) + positionFour.y) * (tPowerThree));


        float z = 0.5f * ((positionTwo.z * 2.0f) + (-positionOne.z + positionThree.z) * ratio +
            ((positionOne.z * 2.0f) - (positionTwo.z * 5.0f) + (positionThree.z * 4.0f) - positionFour.z) * tPowerTwo +
            (-positionOne.z + (positionTwo.z * 3.0f) - (positionThree.z * 3.0f) + positionFour.z) * (tPowerThree));

        return new Vector3(x, y, z);
    }
    private Vector3 LinearPosition(int seg, float ratio)
    {
        Vector3 pointOne = railNodes[seg].position;
        Vector3 pointTwo = railNodes[seg+1].position;

        return Vector3.Lerp(pointOne,pointTwo,ratio);
    }
    private void OnDrawGizmos()
    {
        for(int i = 0; i < railNodes.Length - 1; i++)
        {
            Handles.DrawDottedLine(railNodes[i].position, railNodes[i + 1].position, 3.0f);
        }
    }

}
