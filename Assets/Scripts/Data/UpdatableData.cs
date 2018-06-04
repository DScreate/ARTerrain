using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Class <c>UpdatableData</c> handles automatic updates for terrain generation data. 
/// </summary>
/// <remarks> 
/// Inherits from <see cref="ScriptableObject"/>
/// </remarks>
namespace TerrainGenData
{
    public class UpdatableData : ScriptableObject{

        public event System.Action OnValuesUpdated;
        public bool autoUpdate;

        /// <summary>
        /// Method <c>OnValidate</c> checks auto updates 
        /// </summary>
        protected virtual void OnValidate()
        {
#if UNITY_EDITOR
            if (autoUpdate)
            {
                UnityEditor.EditorApplication.update += NotifyOfUpdatedValues;
            }
#endif
        }

        /// <summary>
        /// Method <c>NotifyOfUpdatedValues</c> checks if values updated, calls <see cref="OnValuesUpdated"/>
        /// </summary>
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