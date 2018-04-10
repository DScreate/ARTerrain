using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CameraMovement;
using UnityEditor;

[CustomEditor(typeof(RailSystem), true)]
    public class CameraRailUpdate : Editor
    {

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
