using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Class <c>DataForTerrain</c> Calculates min and max height values for terrain generation. 
/// <see cref="AnimationCurve"/>
/// </summary>
/// <remark> 
/// Inherits from <see cref="UpdatableData"/>
/// </remark>
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