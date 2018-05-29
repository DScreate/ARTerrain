using UnityEngine;
/// <summary>
/// Class <c>NoiseData</c> handles data from <see cref="NoiseGenerator.NormalizeMode"/>
/// </summary>
/// <remark>
/// Octave: One of the coherent-noise functions in a series of coherent-noise functions that are added together to form Perlin noise.
/// Persistance: A multiplier that determines how quickly the amplitudes diminish for each successive octave in a Perlin-noise function.
/// Lacunarity: A multiplier that determines how quickly the frequency increases for each successive octave in a Perlin-noise function.
/// Inherits from <see cref="UpdatableData"/>
/// </remark>
namespace TerrainGenData
{
    [CreateAssetMenu()]
    public class NoiseData : UpdatableData
    {
        public NoiseGenerator.NormalizeMode normalizeMode;

        public float noiseScale;

        [Range(0, 100)]
        public int octaves;

        [Range(0, 1)]
        public float persistance;
        public float lacunarity;

        public int seed;
        public Vector2 offset;

        public bool Updated { get; set; }

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