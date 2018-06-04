using UnityEngine;
/// <summary>
/// Class <c>NoiseData</c> handles data from <see cref="NoiseGenerator.NormalizeMode"/>
/// </summary>
/// <remarks>
/// Inherits from <see cref="UpdatableData"/>
/// </remarks>
namespace TerrainGenData
{
    [CreateAssetMenu()]
    public class NoiseData : UpdatableData
    {
        public NoiseGenerator.NormalizeMode normalizeMode;

        public float noiseScale;

        /// <summary> 
        /// Octave: One of the coherent-noise functions in a series of coherent-noise functions that are added together to form Perlin noise.
        /// </summary>
        [Range(0, 100)]
        public int octaves;

        [Range(0, 1)]
        /// <summary> 
        /// Persistance: A multiplier that determines how quickly the amplitudes diminish for each successive octave in a Perlin-noise function.
        /// </summary>
        public float persistance;
        /// <summary> 
        /// Lacunarity: A multiplier that determines how quickly the frequency increases for each successive octave in a Perlin-noise function.
        /// </summary>
        public float lacunarity;

        public int seed;
        public Vector2 offset;

        public bool Updated { get; set; }

        /// <summary>
        /// Method <c>OnValidate</c> checks values of lacunarity and octaves, then updates then and calls super OnValidate().
        /// </summary>
        protected override void OnValidate()
        {
            if (lacunarity < 1)
                lacunarity = 1;
            if (octaves < 0)
                octaves = 0;

            Updated = true;

            base.OnValidate();
        }
    }
}