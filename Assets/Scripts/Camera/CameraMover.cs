using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMover : MonoBehaviour {

    public RailSystem rails;

    private int currentSegment;
    private float transition; //used in the raito in rail
    private bool isCompleted;

	
	// Update is called once per frame
	void Update () {
        if (!rails)
            return;
        if (!isCompleted)
            Play();
        
	}

    private void Play()
    {
        transition += Time.deltaTime * 1 / 2.5f;
        if(transition > 1)
        {
            transition = 0;
            currentSegment++;
        }
        else if (transition < 0)
        {
            transition = 1;
            currentSegment--;
        }

        transform.position = rails.LinearPosition(currentSegment, transition);
        transform.rotation = rails.Orientation(currentSegment, transition);
    }
}
