﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TerrainGenData
{
    public class UpdatableData : ScriptableObject{

        public event System.Action OnValuesUpdated;
        public bool autoUpdate;

        protected virtual void OnValidate()
        {
            if (autoUpdate)
            {
                NotifyOfUpdatedValues();
            }
        }

        public void NotifyOfUpdatedValues()
        {
            if (OnValuesUpdated != null)
                OnValuesUpdated();
        }
    }
}