using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TerrainGenData;

[CustomEditor(typeof(UpdatableData),true)]
public class UpdateDataEditor : Editor {

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        UpdatableData data = (UpdatableData)target;

        if (GUILayout.Button("Update"))
        {
            data.NotifyOfUpdatedValues();
            EditorUtility.SetDirty(target);
        }
    }
}
