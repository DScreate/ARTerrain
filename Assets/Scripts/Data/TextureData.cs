using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace TerrainGenData
{
    [CreateAssetMenu()]
    public class TextureData : UpdatableData
    {
        const int textureSize = 512;
        const TextureFormat textureFormat = TextureFormat.RGB565;

        public Layer[] layers;

        float savedMinHeight;
        float savedMaxHeight;

        public GameObject Water;

        public DataForTerrain terrainData;

        public void ApplyToMaterial(Material material)
        {
            material.SetInt("layerCount", layers.Length);
            material.SetColorArray("baseColors", layers.Select(x => x.tint).ToArray());
            material.SetFloatArray("baseStartHeights", layers.Select(x => x.startHeight).ToArray());
            material.SetFloatArray("baseBlends", layers.Select(x => x.blendStrength).ToArray());
            material.SetFloatArray("baseColorStrength", layers.Select(x => x.tintStrength).ToArray());
            material.SetFloatArray("baseTextureScales", layers.Select(x => x.textureScale).ToArray());
            Texture2DArray texturesArray = GenerateTextureArray(layers.Select(x => x.texture).ToArray());
            material.SetTexture("baseTextures", texturesArray);

            UpdateMeshHeights(material , savedMinHeight,savedMaxHeight);

            if(Application.isPlaying)
                UpdateWaterHeight(layers[1].startHeight * terrainData.meshHeightMultiplier * terrainData.uniformScale);
        }
        public void UpdateMeshHeights(Material material, float minHeight, float maxHeight)
        {
            savedMinHeight = minHeight;
            savedMaxHeight = maxHeight;

            material.SetFloat("minHeight",minHeight);
            material.SetFloat("maxHeight",maxHeight);
        }

        public void UpdateWaterHeight(float newHeight)
        {
            if(Water == null)
                Water = GameObject.Find("WaterProDaytime");

            Vector3 curPos = Water.transform.position;
            Water.transform.position = new Vector3(curPos.x, newHeight, curPos.z);
        }

        Texture2DArray GenerateTextureArray(Texture2D[] textures)
        {
            Texture2DArray textureArray = new Texture2DArray(textureSize, textureSize, textures.Length, textureFormat, true);
            for(int i = 0; i < textures.Length; i++)
            {
                textureArray.SetPixels(textures[i].GetPixels(), i);               
            }
            textureArray.Apply();
            return textureArray;
        }

        [System.Serializable]
        public class Layer
        {
            public Texture2D texture;
            public Color tint;
            [Range(0,1)]
            public float tintStrength;
            [Range(0, 1)]
            public float startHeight;
            [Range(0, 1)]
            public float blendStrength;
            public float textureScale;
        }
    }
}
