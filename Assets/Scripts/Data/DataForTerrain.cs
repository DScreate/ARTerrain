using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TerrainGenData
{
    [CreateAssetMenu()]
    public class DataForTerrain : UpdatableData
    {
        public float uniformScale = 2.5f;

        public float meshHeightMultiplier;
        public AnimationCurve meshHeightCurve;

        public float minHeight
        {
            get
            {
                return uniformScale * meshHeightMultiplier * meshHeightCurve.Evaluate(0);
            }
        } 
        public float maxHeight
        {
            get
            {
                return uniformScale * meshHeightMultiplier * meshHeightCurve.Evaluate(1);
            }
        }
    }
}