using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Class <c>UpdatableData</c> handles automatic updates for terrain generation data. 
/// </summary>
/// <remark> 
/// Inherits from <see cref="ScriptableObject"/>
/// </remark>
namespace TerrainGenData
{
    public class UpdatableData : ScriptableObject{

        public event System.Action OnValuesUpdated;
        public bool autoUpdate;

        protected virtual void OnValidate()
        {
#if UNITY_EDITOR
            if (autoUpdate)
            {
                UnityEditor.EditorApplication.update += NotifyOfUpdatedValues;
            }
#endif
        }

        public void NotifyOfUpdatedValues()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.update -= NotifyOfUpdatedValues;
            if (OnValuesUpdated != null)
                OnValuesUpdated();
#endif
        }
    }
}