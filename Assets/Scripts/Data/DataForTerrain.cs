using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TerrainGenData
{
    [CreateAssetMenu()]
    public class DataForTerrain : UpdatableData
    {

        public float meshHeightMultiplier;
        public AnimationCurve meshHeightCurve;

    }
}