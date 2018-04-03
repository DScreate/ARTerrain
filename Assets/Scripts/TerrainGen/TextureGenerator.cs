using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TextureGenerator {

    public static Texture2D TextureFromHeightMap(float[,] heightMap)
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);

        Color[] colorMap = new Color[width * height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                colorMap[y * width + x] = Color.Lerp(Color.black, Color.white, heightMap[x, y]);
            }
        }

        return TextureFromColorMap(colorMap, width, height);
    }

    public static Texture2D TextureFromColorMap(Color[] colorMap, int width, int height)
    {
        Texture2D texture = new Texture2D(width, height)
        {
            filterMode = FilterMode.Point,
            wrapMode = TextureWrapMode.Clamp
        };
        texture.SetPixels(colorMap);
        texture.Apply();
        return texture;
    }    

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
    }

    public static float[,] TextureToNoise(Texture2D texture, int width, int height)
    {
        float[,] noiseMap = new float[width, height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                noiseMap[x, y] = texture.GetPixel(x, y).grayscale;
            }
        }

        return noiseMap;
    }

    /*
        public static Texture2D ApplyNoiseToTexture(Texture2D texture, float[,] noiseMap, float noiseWeight, float minGreyValue)
        {
            Color[] noisePixels = new Color[noiseMap.GetLength(0) * noiseMap.GetLength(1)];
            float sample;
            Color texSample;

            int width = texture.width <= noiseMap.GetLength(0) ? texture.width : noiseMap.GetLength(0);
            int height = texture.height <= noiseMap.GetLength(1) ? texture.height : noiseMap.GetLength(1);

            Texture2D newTex = new Texture2D(width, height);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    sample = noiseMap[x,y];
                    texSample = texture.GetPixel(x, y);

                    if (texSample.grayscale > minGreyValue)
                    {
                        newTex.SetPixel(x, y, new Color(Mathf.Lerp(texSample.grayscale, sample, noiseWeight),
                            Mathf.Lerp(texSample.grayscale, sample, noiseWeight),
                            Mathf.Lerp(texSample.grayscale, sample, noiseWeight)));
                    } else
                    {
                        newTex.SetPixel(x, y, new Color(texSample.r, texSample.b, texSample.g));
                    }

                }
            }
            newTex.Apply();

            return newTex;
        }            

        public static float[,] WebcamTextureToNoiseChunk(WebCamTexture webCamTexture, Vector2 offset, int width, int height)
        {
            float[,] noiseMap = new float[width, height];

            int xOffset = (int)offset.x * width;
            int yOffset = (int)offset.y * height;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    noiseMap[x, y] = webCamTexture.GetPixel(x + xOffset, y + yOffset).grayscale;
                }
            }

            return noiseMap;
        }*/

}
