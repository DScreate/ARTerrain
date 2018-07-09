using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CameraMovement
{
    /// <summary>
    /// This class deals with the movement of the camera in the unity engine. 
    /// It uses the RailSystem which contains the references to the section of the Rail object that
    /// the in-engine camera will move. 
    /// </summary>
    /// <remarks>
    /// The Rail System must have a RailSystem Script to run the operations.
    /// The playMode which dictates how the camera will move when project is playing but one must be selected to run.
    /// </remarks>
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

        /// <summary>
        /// This is called once Per Frame while Program is playing.
        /// The Update first checks it can see the Rail System script and 
        /// if not it will just end the update and never move the camera.
        /// 
        /// From here it checks if the camera has completed its run along the Rail System and if it hasn't it will 
        /// contiune to play and move to the end of the Rails.
        /// </summary>
        void Update()
        {
            if (!rails)
                return;
            if (!isCompleted)
                Play(!reversed);

        }

        /// <summary>
        /// This Method dictates with the speed and position on the Rail System how it will transform from position
        /// to position when the product is running.
        /// </summary>
        /// <param name="forward">This tells if the camera is moving towards the end of the Rail System or the beginning </param>
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