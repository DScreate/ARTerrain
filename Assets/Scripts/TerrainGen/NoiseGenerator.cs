using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NoiseGenerator
{

    public enum NormalizeMode { Local, Global };

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

    public static float[,] LerpNoiseMapWithTextureToNoiseChunk(FaceDetection faceDetection, float[,] noiseMap, float noiseWeight, float minGreyValue, int chunkWidth, int chunkHeight, Vector2 offset, Boolean FaceOnly)
    {
        //Color[] noisePixels = new Color[noiseMap.GetLength(0) * noiseMap.GetLength(1)];
        float[,] noiseChunk = new float[chunkWidth, chunkHeight];
        float noiseSample;
        float texSample;
        OpenCVForUnity.Rect[] square = new OpenCVForUnity.Rect[0];
        Texture2D texture = faceDetection.FaceTexture;

        if (FaceOnly)
            square = faceDetection.FaceLocations;

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

                    if (FaceOnly)
                    {
                        noiseChunk[x,y] = ContainsFace(square, Lerpator(texSample, noiseSample, noiseWeight), x, y, offsetX, offsetY);
                    }
                    else
                    {
                        noiseChunk[x,y] = Lerpator(texSample, noiseSample, noiseWeight);
                    }

                }
                else
                {
                    if (FaceOnly)
                    {
                        noiseChunk[x,y] = ContainsFace(square, texSample, x, y, offsetX, offsetY);
                    }
                    else
                    {
                        noiseChunk[x, y] = texSample;

                    }

                }
            }
        }

        return noiseChunk;
    }

    private static float ContainsFace( OpenCVForUnity.Rect[] rect, float heightNumber, int x, int y, int offsetX, int offsetY)
    {
        float num = 0.0f;
        if (rect.Length > 0)
        {
            if (rect[0].contains(x + offsetX, y + offsetY))
                num = heightNumber;
        }
        return num;
    }

    private static float Lerpator(float texSample, float noiseSample, float noiseWeight)
    {
        return Mathf.Lerp(texSample, noiseSample, noiseWeight);
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
