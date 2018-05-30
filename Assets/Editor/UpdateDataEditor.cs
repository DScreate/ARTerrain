using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TerrainGenData;

/// <summary>
/// UpdateDataEditor class is a custom editor to add more functionality to the editor.
/// It is used when there is an already set UpdatableData Asset with its values and 
/// the objects values are changed but not yet set to the UpdatableData this will set the new values to the Asset.
/// </summary>
[CustomEditor(typeof(UpdatableData),true)]
public class UpdateDataEditor : Editor {

    /// <summary>
    /// This is the method used to add the button to the editor.
    /// When pressed it will update the UpdatableData new values to the Asset.
    /// </summary>
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
