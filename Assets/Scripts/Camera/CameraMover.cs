using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CameraMovement
{
    public class CameraMover : MonoBehaviour
    {

        public RailSystem rails;
        public Mode playMode;

        public float speed = 2.5f;
        public bool reversed;
        public bool looping;
        public bool pingpong;



        private int currentSegment;
        private float transition; //used in the raito in rail
        private bool isCompleted;


        // Update is called once per frame
        void Update()
        {
            if (!rails)
                return;
            if (!isCompleted)
                Play(!reversed);

        }

        private void Play(bool forward = true)
        {
            float magnitudeSegment = (rails.getRailNodes()[currentSegment + 1].position - rails.getRailNodes()[currentSegment].position).magnitude;
            float speedSegment = (Time.deltaTime * 1 / magnitudeSegment) * speed;
            transition += (forward) ? speedSegment : -speedSegment;

            if (transition > 1)
            {
                transition = 0;
                currentSegment++;

                if (currentSegment == rails.getRailNodes().Length - 1)
                {
                    if (looping)
                    {
                        if (pingpong)
                        {
                            transition = 1;
                            currentSegment = rails.getRailNodes().Length - 2;
                            reversed = !reversed;
                        }
                        else
                        {
                            currentSegment = 0;
                        }
                    }
                    else
                    {
                        isCompleted = true;
                        return;
                    }
                }
            }
            else if (transition < 0)
            {
                transition = 1;
                currentSegment--;
                if (currentSegment == -1)
                {
                    if (looping)
                    {
                        if (pingpong)
                        {
                            transition = 0;
                            currentSegment = 0;
                            reversed = !reversed;
                        }
                        else
                        {
                            currentSegment = rails.getRailNodes().Length - 2;
                        }
                    }
                    else
                    {
                        isCompleted = true;
                        return;
                    }
                }
            }

            transform.position = rails.PositionOnRailSystem(currentSegment, transition, playMode);
            transform.rotation = rails.Orientation(currentSegment, transition);
        }
    }
}