using UnityEngine;
using ColorTracking;
using System;
using TerrainGenData;

public class MapGenerator : MonoBehaviour {

    public enum DrawMode { NoiseMap, ColorMap, Mesh };
    public enum ImageMode { PureNoise, FromImage, FromWebcam, FromOpenCV }
    [TooltipAttribute("Set the name of the device to use.")]
    public string requestedDeviceName = null;
    public DrawMode drawMode;

    public DataForTerrain terrainData;
    public NoiseData noiseData;
    public TextureData textureData;

    public Material terrainMaterial;

    public ImageMode imageMode;
    public Texture2D imageTex;

    [Range(0, 1)]
    public float minGreyValue;

    [Range(0, 1)]
    public float noiseWeight;

    public int mapWidth;
    public int mapHeight;

    public bool autoUpdate;

    private int MeshRatioWidth;
    private int MeshRatioHeight;

    //public TerrainType[] regions;

    WebCamTexture _webcamtex;
    Texture2D _TextureFromCamera;
    ObjectTrackingBasedOnColor _trackObjectsBasedOnColor;

    void OnValuesUpdate()
    {
        if (!Application.isPlaying)
        {
            GenerateMap();
        }
    }
    void OnTextureValuesUpdated()
    {
        textureData.ApplyToMaterial(terrainMaterial);
    }

    private void Start()
    {
        if (imageMode == ImageMode.FromWebcam || imageMode == ImageMode.FromOpenCV)
        {
            _TextureFromCamera = new Texture2D(mapWidth, mapHeight);

            if (!String.IsNullOrEmpty(requestedDeviceName))
            {
                _webcamtex = new WebCamTexture(requestedDeviceName, mapWidth, mapHeight);
            }

            else
                _webcamtex = new WebCamTexture(mapWidth, mapHeight);

            _webcamtex.Play();

            if (imageMode == ImageMode.FromOpenCV)            
                _trackObjectsBasedOnColor = new ObjectTrackingBasedOnColor(_webcamtex);

            int terrX , terrY;
            int counterX = 1 , counterY = 1;
           while ((terrX = _webcamtex.width / counterX) > 250)
            {
                counterX++;
            }
            while ((terrY = _webcamtex.height / counterY) > 250)
            {
                counterY++;
            }

            mapHeight = terrY;
            mapWidth = terrX;
            MeshRatioWidth = counterX - 1;
            MeshRatioHeight = counterY - 1;
        }        
    }

    private void Update()
    {
        if (imageMode == ImageMode.FromWebcam)
        {
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
        float[,] noiseMap = Noise.GenerateNoiseMap(mapWidth, mapHeight, noiseData.seed, noiseData.noiseScale, noiseData.octaves, noiseData.persistance, noiseData.lacunarity, noiseData.offset, noiseData.normalizeMode);

        if (imageMode == ImageMode.PureNoise)
        {
            Color[] colorMap = new Color[mapWidth * mapHeight];

            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    float currentHeight = noiseMap[x, y];
                    /*for (int i = 0; i < regions.Length; i++)
                    {
                        if (currentHeight <= regions[i].height)
                        {
                            colorMap[y * mapWidth + x] = regions[i].color;
                            break;
                        }
                    }*/
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
                display.DrawMesh(MeshGenerator.GenerateTerrainMesh(noiseMap, terrainData.meshHeightMultiplier, terrainData.meshHeightCurve));
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
                    /*for (int i = 0; i < regions.Length; i++)
                    {
                        if (currentHeight <= regions[i].height)
                        {
                            colorMap[y * mapWidth + x] = regions[i].color;
                            break;
                        }
                    }*/
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
                display.DrawMesh(MeshGenerator.GenerateTerrainMesh(TextureGenerator.TextureToNoise(noisedTex), terrainData.meshHeightMultiplier, terrainData.meshHeightCurve) /*, TextureGenerator.TextureFromColorMap(colorMap, mapWidth, mapHeight)*/);
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
                    /*for (int i = 0; i < regions.Length; i++)
                    {
                        if (currentHeight <= regions[i].height)
                        {
                            colorMap[y * mapWidth + x] = regions[i].color;
                            break;
                        }
                    }*/
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
                display.DrawMesh(MeshGenerator.GenerateTerrainMesh(TextureGenerator.TextureToNoise(noisedTex), terrainData.meshHeightMultiplier, terrainData.meshHeightCurve)/*, TextureGenerator.TextureFromColorMap(colorMap, mapWidth, mapHeight)*/);
            }
        }
        textureData.UpdateMeshHeights(terrainMaterial, terrainData.minHeight, terrainData.maxHeight);
    }

    private void OnValidate()
    {
        if (mapWidth < 1)
            mapWidth = 1;
        if (mapHeight < 1)
            mapHeight = 1;
        if (terrainData != null)
        {
            terrainData.OnValuesUpdated -= OnValuesUpdate;
            terrainData.OnValuesUpdated += OnValuesUpdate;
        }
        if (noiseData != null)
        {
            noiseData.OnValuesUpdated -= OnValuesUpdate;
            noiseData.OnValuesUpdated += OnValuesUpdate;
        }
        if (textureData != null)
        {
            textureData.OnValuesUpdated -= OnTextureValuesUpdated;
            textureData.OnValuesUpdated += OnTextureValuesUpdated;
        }

    }
}

/*
[System.Serializable]
public struct TerrainType
{
    public string name;
    public float height;
    public Color color;
}
*/