using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is responsible for the calculations needed to generate a float[,] representing a perlin noise map.
/// It is additionally responsible for blending a <c>Texture2D</c> object (that in most cases represents a webcam image or some still image) with a noiseMap
/// before then returning it as a float[,].
/// In both cases, the float[,] represents a heightMap containing values at each point that represent the height of a given x,y coordinate. This is then used
/// to determine how "tall" that point should be in the 3D Unity worldspace as part of the mesh.
/// </summary>
public static class NoiseGenerator
{
    /// <summary>
    /// 
    /// </summary>
    public enum NormalizeMode { Local, Global };

    /// <summary>
    /// Generates the noise map based upon perlin noise. This creates a smooth transition between minimum and maximum value boundaries and allows for a more realistic overall look.
    /// <seealso href="https://en.wikipedia.org/wiki/Perlin_noise"/>
    /// </summary>
    /// <param name="mapWidth">Width of the map.</param>
    /// <param name="mapHeight">Height of the map. This is the "y" component of a 2D map, not the actual height of how "tall" something on the 3D mesh would be</param>
    /// <param name="seed">The seed used for the pseudo-random number generator</param>
    /// <param name="scale">The scale. This can be thought of "zooming" in and out of sections of the noise map</param>
    /// <param name="octaves">The octaves. This controls the large troughs and peaks on the noise map. This is akin to mountains and valleys</param>
    /// <param name="persistance">The persistance. This controls the medium level details on the noiseMap and can be thought of as ridges, hills or crevices</param>
    /// <param name="lacunarity">The lacunarity. This controls the small level details on the noiseMap and can be thought of as individual rocks/boulders or other small details</param>
    /// <param name="offset">The offset.</param>
    /// <param name="normalizeMode">The normalize mode.</param>
    /// <returns>A float[,] representing a NoiseMap generated via the method. In particular, values within the float[,] will range from 0 to a value controlled by the maxPossibleHeight variable</returns>
    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset, NormalizeMode normalizeMode)
    {
        float[,] noiseMap = new float[mapWidth, mapHeight];

        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffset = new Vector2[octaves];

        float maxPossibleHeight = 0;
        float amplitude = 1;
        float frequency = 1;

        for (int i = 0; i < octaves; i++)
        {
            float offsetX = prng.Next(-100000, 100000) + offset.x;
            float offsetY = prng.Next(-100000, 100000) + offset.y;
            octaveOffset[i] = new Vector2(offsetX, offsetY);

            maxPossibleHeight += amplitude;
            amplitude *= persistance;
        }

        if (scale <= 0)
        {
            scale = 0.0001f;
        }

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        float halfWidth = mapWidth / 2f;
        float halfHeight = mapHeight / 2f;

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                amplitude = 1;
                frequency = 1;
                float noiseHeight = 0;

                for (int i = 0; i < octaves; i++)
                {
                    float sampleX = (x - halfWidth + octaveOffset[i].x) / scale * frequency;
                    float sampleY = (y - halfHeight + octaveOffset[i].y) / scale * frequency;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    maxPossibleHeight += amplitude;
                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                if (noiseHeight > maxNoiseHeight)
                {
                    maxNoiseHeight = noiseHeight;
                }
                else if (noiseHeight < minNoiseHeight)
                {
                    minNoiseHeight = noiseHeight;
                }
                noiseMap[x, y] = noiseHeight;
            }
        }

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                if (normalizeMode == NormalizeMode.Local)
                {
                    noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
                }
                else
                {
                    float normalizedHeight = (noiseMap[x, y] + 1) / (maxPossibleHeight / 0.9f);
                    noiseMap[x, y] = Mathf.Clamp(normalizedHeight, 0, int.MaxValue);
                }
            }
        }

        return noiseMap;
    }

    /// <summary>
    /// Lerps the noise map with texture to noise chunk. This method is used to blend a texture with a generated noiseMap linearly in order to provide a controllable and uniform
    /// semi-randomization to the texture. This method combines a noiseMap with a Texture2D by sampling each x,y point and taking the linearly interpolated value generated based upon
    /// a weight and then places that new value into a new float[,] which is then returned once all points have been sampled
    /// </summary>
    /// <param name="texture">The texture.</param>
    /// <param name="noiseMap">The noise map.</param>
    /// <param name="noiseWeight">The noise weight.</param>
    /// <param name="minGreyValue">The minimum grey value.</param>
    /// <param name="chunkWidth">Width of the chunk.</param>
    /// <param name="chunkHeight">Height of the chunk.</param>
    /// <param name="offset">The offset.</param>
    /// <returns>A float[,] representing a heightMap that has been generated via the combination of the Texture2D and the noiseMap after the two have been blended together</returns>
    public static float[,] LerpNoiseMapWithTextureToNoiseChunk(Texture2D texture, float[,] noiseMap, float noiseWeight, float minGreyValue, int chunkWidth, int chunkHeight, Vector2 offset)
    {
        //Color[] noisePixels = new Color[noiseMap.GetLength(0) * noiseMap.GetLength(1)];
        float[,] noiseChunk = new float[chunkWidth, chunkHeight];
        float noiseSample;
        float texSample;

        //int chunkWidth = texture.width <= noiseMap.GetLength(0) ? texture.width : noiseMap.GetLength(0);
        //int chunkheight = texture.height <= noiseMap.GetLength(1) ? texture.height : noiseMap.GetLength(1);

        //Texture2D newTex = new Texture2D(chunkWidth, chunkheight);

        int offsetX = (int)offset.x * chunkWidth;
        int offsetY = (int)offset.y * chunkHeight;

        if (noiseMap.GetLength(0) != texture.width)
            Debug.Log("NoiseMap width " + noiseMap.GetLength(0) + "not equal to Texture width " + texture.width);

        if (noiseMap.GetLength(1) != texture.height)
            Debug.Log("NoiseMap height " + noiseMap.GetLength(1) + "not equal to Texture height " + texture.height);

        for (int y = 0; y < chunkHeight; y++)
        {
            for (int x = 0; x < chunkWidth; x++)
            {
                texSample = texture.GetPixel(x + offsetX, y + offsetY).grayscale;

                if (texSample > minGreyValue)
                {
                    try
                    {
                        noiseSample = noiseMap[x + offsetX, y + offsetY];
                    }
                    catch (IndexOutOfRangeException e)
                    {
                        noiseSample = texSample;
                    }

                    noiseChunk[x, y] = Mathf.Lerp(texSample, noiseSample, noiseWeight);
                }
                else
                {
                    noiseChunk[x, y] = texSample;
                }
            }
        }

        return noiseChunk;
    }
    /*
        public static float[,] TextureToNoiseChunk(Texture2D texture, Vector2 offset, int width, int height)
        {
            float[,] noiseMap = new float[width, height];

            int xOffset = (int)offset.x * width;
            int yOffset = (int)offset.y * height;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    noiseMap[x, y] = texture.GetPixel(x + xOffset, y + yOffset).grayscale;
                }
            }

            return noiseMap;
        }*/
}
