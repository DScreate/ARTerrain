using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CameraMovement;
using UnityEditor;

/// <summary>
/// CameraRailUpdate class is a custom editor to add more functionality.
/// It is used when a set Rail System has a new node is 
/// added that it will update to the railNode list in the RailSystem Script.
/// </summary>
[CustomEditor(typeof(RailSystem), true)]
    public class CameraRailUpdate : Editor
    {

        /// <summary>
        /// This is the method used to add the button to the editor.
        /// When pressed it will update the railNode List of the rails Object current list of child node.
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            RailSystem data = (RailSystem)target;

            if (GUILayout.Button("Update"))
            {
                data.setNodes();
                EditorUtility.SetDirty(target);
            }
        }
    }
