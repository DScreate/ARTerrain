using UnityEngine;

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