using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class RailSystem : MonoBehaviour {

    private Transform[] railNodes;
	// Use this for initialization
	void Start () {
        railNodes = GetComponentsInChildren<Transform>();
 
	}

    public Vector3 LinearPosition(int seg, float ratio)
    {
        Vector3 pointOne = railNodes[seg].position;
        Vector3 pointTwo = railNodes[seg+1].position;

        return Vector3.Lerp(pointOne,pointTwo,ratio);
    }
    public Quaternion Orientation(int seg, float ratio)
    {
        Quaternion quaterOne = railNodes[seg].rotation;
        Quaternion quaterTwo = railNodes[seg+1].rotation;

        return Quaternion.Lerp(quaterOne,quaterTwo,ratio);
    }

    private void OnDrawGizmos()
    {
        for(int i = 0; i < railNodes.Length - 1; i++)
        {
            Handles.DrawDottedLine(railNodes[i].position, railNodes[i + 1].position, 3.0f);
        }
    }

}
