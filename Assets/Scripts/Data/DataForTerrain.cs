using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Class <c>DataForTerrain</c> Calculates min and max height values for terrain generation. 
/// <see cref="AnimationCurve"/>
/// </summary>
/// <remarks> 
/// Inherits from <see cref="UpdatableData"/>
/// </remarks>
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