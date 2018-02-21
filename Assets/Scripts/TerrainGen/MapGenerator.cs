using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ColorTracking;
using System;

public class MapGenerator : MonoBehaviour {

    public enum DrawMode { NoiseMap, ColorMap, Mesh };
    public enum ImageMode { PureNoise, FromImage, FromWebcam, FromOpenCV }
    [TooltipAttribute("Set the name of the device to use.")]
    public string requestedDeviceName = null;
    public DrawMode drawMode;

    public ImageMode imageMode;

    public Texture2D imageTex;

    [Range(0, 1)]
    public float minGreyValue;

    [Range(0, 1)]
    public float noiseWeight;

    public int mapWidth;
    public int mapHeight;
    public float noiseScale;

    public int octaves;
    [Range(0,1)]
    public float persistance;
    public float lacunarity;

    public int seed;
    public Vector2 offset;

    public float meshHeightMultiplier;
    public AnimationCurve meshHeightCurve;

    public bool autoUpdate;

    public TerrainType[] regions;

    WebCamTexture _webcamtex;
    Texture2D _TextureFromCamera;
    ObjectTrackingBasedOnColor _trackObjectsBasedOnColor;

    private void Start()
    {
        if (imageMode == ImageMode.FromWebcam || imageMode == ImageMode.FromOpenCV)
        {
            if (!String.IsNullOrEmpty(requestedDeviceName))            
                _webcamtex = new WebCamTexture(requestedDeviceName, mapWidth, mapHeight);
            
            else
                _webcamtex = new WebCamTexture(mapWidth, mapHeight);

            _webcamtex.Play();

            if (imageMode == ImageMode.FromOpenCV)            
                _trackObjectsBasedOnColor = new ObjectTrackingBasedOnColor(_webcamtex);            
        }        
    }

    private void Update()
    {
        if (imageMode == ImageMode.FromWebcam)
        {
            _TextureFromCamera = new Texture2D(mapWidth, mapHeight);
            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    _TextureFromCamera.SetPixel(x, y, _webcamtex.GetPixel(x, y));
                }
            }
            _TextureFromCamera.Apply();
            GenerateMap();
        }

        if (imageMode == ImageMode.FromOpenCV)
        {
            _TextureFromCamera = _trackObjectsBasedOnColor.UpdateGrayScale();
            //_TextureFromCamera = _trackObjectsBasedOnColor.UpdateGrayScale(_webcamtex);
            //_TextureFromCamera = new Texture2D(_webcamtex.width, _webcamtex.height, TextureFormat.RGB24, false);
            //_trackObjectsBasedOnColor.UpdateGrayScale(_webcamtex, _TextureFromCamera);
            GenerateMap();
        }


    }

    public void GenerateMap()
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(mapWidth, mapHeight, seed, noiseScale, octaves, persistance, lacunarity, offset);

        if (imageMode == ImageMode.PureNoise)
        {
            Color[] colorMap = new Color[mapWidth * mapHeight];

            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    float currentHeight = noiseMap[x, y];
                    for (int i = 0; i < regions.Length; i++)
                    {
                        if (currentHeight <= regions[i].height)
                        {
                            colorMap[y * mapWidth + x] = regions[i].color;
                            break;
                        }
                    }
                }
            }


            MapDisplay display = FindObjectOfType<MapDisplay>();
            if (drawMode == DrawMode.NoiseMap)
            {
                display.DrawTexture(TextureGenerator.TextureFromHeightMap(noiseMap));
            }
            else if (drawMode == DrawMode.ColorMap)
            {
                display.DrawTexture(TextureGenerator.TextureFromColorMap(colorMap, mapWidth, mapHeight));
            }
            else if (drawMode == DrawMode.Mesh)
            {
                display.DrawMesh(MeshGenerator.GenerateTerrainMesh(noiseMap, meshHeightMultiplier, meshHeightCurve), TextureGenerator.TextureFromColorMap(colorMap, mapWidth, mapHeight));
            }
        }
        else if (imageMode == ImageMode.FromImage)
        {
            Texture2D noisedTex = TextureGenerator.ApplyNoiseToTexture(imageTex, noiseMap, noiseWeight, minGreyValue);
            MapDisplay display = FindObjectOfType<MapDisplay>();

            Color[] colorMap = new Color[mapWidth * mapHeight];

            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    float currentHeight = noisedTex.GetPixel(x, y).grayscale;
                    for (int i = 0; i < regions.Length; i++)
                    {
                        if (currentHeight <= regions[i].height)
                        {
                            colorMap[y * mapWidth + x] = regions[i].color;
                            break;
                        }
                    }
                }
            }

            if (drawMode == DrawMode.NoiseMap)
            {
                display.DrawTexture(noisedTex);
            }
            else if (drawMode == DrawMode.ColorMap)
            {
                display.DrawTexture(TextureGenerator.TextureFromColorMap(colorMap, mapWidth, mapHeight));
            }
            else if (drawMode == DrawMode.Mesh)
            {
                display.DrawMesh(MeshGenerator.GenerateTerrainMesh(TextureGenerator.TextureToNoise(noisedTex), meshHeightMultiplier, meshHeightCurve), TextureGenerator.TextureFromColorMap(colorMap, mapWidth, mapHeight));
            }
        }
        else if (imageMode == ImageMode.FromWebcam || imageMode == ImageMode.FromOpenCV)
        {
            Texture2D noisedTex = TextureGenerator.ApplyNoiseToTexture(_TextureFromCamera, noiseMap, noiseWeight, minGreyValue);
            MapDisplay display = FindObjectOfType<MapDisplay>();

            Color[] colorMap = new Color[mapWidth * mapHeight];

            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    float currentHeight = noisedTex.GetPixel(x, y).grayscale;
                    for (int i = 0; i < regions.Length; i++)
                    {
                        if (currentHeight <= regions[i].height)
                        {
                            colorMap[y * mapWidth + x] = regions[i].color;
                            break;
                        }
                    }
                }
            }

            if (drawMode == DrawMode.NoiseMap)
            {
                display.DrawTexture(noisedTex);
            }
            else if (drawMode == DrawMode.ColorMap)
            {
                display.DrawTexture(TextureGenerator.TextureFromColorMap(colorMap, mapWidth, mapHeight));
            }
            else if (drawMode == DrawMode.Mesh)
            {
                display.DrawMesh(MeshGenerator.GenerateTerrainMesh(TextureGenerator.TextureToNoise(noisedTex), meshHeightMultiplier, meshHeightCurve), TextureGenerator.TextureFromColorMap(colorMap, mapWidth, mapHeight));
            }
        }
    }

    private void OnValidate()
    {
        if (mapWidth < 1)
            mapWidth = 1;
        if (mapHeight < 1)
            mapHeight = 1;
        if (lacunarity < 1)
            lacunarity = 1;
        if (octaves < 0)
            octaves = 0;
    }
}

[System.Serializable]
public struct TerrainType
{
    public string name;
    public float height;
    public Color color;
}
