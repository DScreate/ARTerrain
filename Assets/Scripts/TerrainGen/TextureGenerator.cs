using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TextureGenerator {
    

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

    public static float[,] TextureToNoise(Texture2D texture, float[,] noiseMap, float noiseWeight)
    {

        int width = texture.width;
        int height = texture.height;

        noiseMap = new float[width, height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                noiseMap[x, y] = texture.GetPixel(x, y).grayscale;
            }
        }

        return noiseMap;
    }


    public static float[,] ConvertTextureToFloats(WebCamTexture texture)
    {
        float[,] floatMap = new float[texture.width, texture.height];
        
        for (int y = 0; y < texture.height; y++)
        {
            for (int x = 0; x < texture.width; x++)
            {
                floatMap[texture.width - x - 1, y] = 1 - texture.GetPixel(x, y).grayscale;
            }
        }

        return floatMap;
    }
    
    /*
    public static float[,] MergeTextureIntoNoise(Texture2D texture)
    {
        int width = texture.width;
        int height = texture.height;

        float[,] noiseMap = new float[width, height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                noiseMap[x, y] = texture.GetPixel(x, y).grayscale;
            }
        }
        
        float sample;
        Color texSample;
        
        //iterate through texture by x and y
        //iterate through noiseMap by x and y
        
        //at x,y of texture, lerp between texture.grayscale and noise value based on noiseWeight-> set into new float[,]
        
        //return new float
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
    }
    */
}
