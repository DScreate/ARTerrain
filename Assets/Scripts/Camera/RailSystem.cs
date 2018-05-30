using System;
using System.Collections;
using System.Collections.Generic;
//using UnityEditor;
using UnityEngine;

namespace CameraMovement
{
    public enum Mode
    {
        Linear,
        Catmull,
        Insta,
    }

    /// <summary>
    /// railNodes: List of positions from GameObject, which is known as Rails in the editor.
    /// Rail System Script is how the CameraMover script interacts with the railNodes.
    /// The CameraMover will do the calculations to find out how far it should move the camera
    /// and where to do it here it give it where to go and how to get there.
    /// </summary>
    /// <remarks>
    ///  This Script must be attacted to the GameObjects to be used as a Rail System.
    /// </remarks>
    public class RailSystem : MonoBehaviour
    {

        private Transform[] railNodes;
        
        /// <summary>
        /// Intialize the railNodes when product start running
        /// </summary>
        void Start()
        {
            setNodes();
        }

        /// <summary>
        /// Will get the the current list of positions from GameObjects used for RailSystem.
        /// </summary>
        public void setNodes()
        {
            Transform[] holder = GetComponentsInChildren<Transform>();
            railNodes = new Transform[holder.Length -1];
            for (int ix = 1; ix < holder.Length; ix++)
            {
                railNodes[ix - 1] = holder[ix];
            }
        }

        /// <summary>
        /// Dictates which type of movement the camera will do. Once determined it will pass the ratio and current segment.
        /// </summary>
        /// <param name="seg">Is the list segment the currently on in the railNode list</param>
        /// <param name="ratio"></param>
        /// <param name="playMode">Tells what Type of movement the camera will do</param>
        /// <returns>tThis returns the position the camera from the given playMode</returns>
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
                    return InstaPosition(seg);
            }
        }
        /// <summary>
        /// This method is used to give the camera's next position. This will cause the camera to 
        /// move straight to the next railNode position.
        /// </summary>
        /// <param name="seg">Current segment the camera is coming from</param>
        /// <returns>next position the camera will move to</returns>
        private Vector3 InstaPosition(int seg)
        {
            return railNodes[seg + 1].position;
        }

        /// <summary>
        /// This class deals with the orientation of the camera as it moves to the next point.
        /// </summary>
        /// <param name="seg">Current segment the camera is coming from</param>
        /// <param name="ratio">Current ratio it is to the next segment</param>
        /// <returns>The orientation of the camera according to its next position</returns>
        public Quaternion Orientation(int seg, float ratio)
        {
            Quaternion quaterOne = railNodes[seg].rotation;
            Quaternion quaterTwo = railNodes[seg + 1].rotation;

            return Quaternion.Lerp(quaterOne, quaterTwo, ratio);
        }
        /// <summary>
        /// This is to get access of the List of railNode positions.
        /// </summary>
        /// <returns>List of railNode positions</returns>
        public Transform[] getRailNodes()
        {
            return railNodes;
        }
        /// <summary>
        /// This Method deals with moving the Camera to another position in a ceratin stylized way.
        /// Way it works is picking the next four nodes position and does an equations on them.Those four node positions
        /// will make the camera move in a more smoother "S" shape from the normal linear movement.
        /// </summary>
        /// <param name="seg">Current segment of the railNodes the Camera is coming from</param>
        /// <param name="ratio">Current ratio to the next segment</param>
        /// <returns>The next position the camera will move to according to the next railNode</returns>
        private Vector3 CatmullPosition(int seg, float ratio)
        {
            Vector3 positionOne, positionTwo, positionThree, positionFour;

            if (seg == 0)
            {
                positionOne = railNodes[seg].position;
                positionTwo = positionOne;
                positionThree = railNodes[seg + 1].position;
                positionFour = railNodes[seg + 2].position;
            }
            else if (seg == railNodes.Length - 2)
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
        /// <summary>
        /// This method is used to give the camera's next position. This will cause the camera to 
        /// move in a linear moement style in basicially a straight line.
        /// </summary>
        /// <param name="seg">Current segment the Camera is on/coming from</param>
        /// <param name="ratio">Current ratio to the next segment</param>
        /// <returns>The next position the camera will move</returns>
        private Vector3 LinearPosition(int seg, float ratio)
        {
            Vector3 pointOne = railNodes[seg].position;
            Vector3 pointTwo = railNodes[seg + 1].position;

            return Vector3.Lerp(pointOne, pointTwo, ratio);
        }
        
        /// <summary>
        /// This method is only ran in the editor to draw lines to all the Nodes used for the Rail System.
        /// This method is many used just to make sure the camera is moving a correct way.
        /// </summary>
        #if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if(railNodes == null)
                setNodes();
            for (int i = 0; i < railNodes.Length - 1; i++)
            {
                Gizmos.DrawLine(railNodes[i].position, railNodes[i + 1].position);
            }
        }
        #endif
    }
}