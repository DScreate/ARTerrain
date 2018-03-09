using UnityEngine;
using System;
using TerrainGenData;


//I think we can delete colormap?
public class MapGenerator : MonoBehaviour {

    public enum DrawMode { NoiseMap, ColorMap, Mesh };
    public enum ImageMode { PureNoise, FromImage, FromWebcam}
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

    public const int mapChunkSize = 241;
    [Range(0,6)]
    public int levelOfDetail;
    //public int mapWidth;
    //public int mapHeight;

    public bool autoUpdate;

    WebCamTexture webcamtex;

    void OnValuesUpdate()
    {
        if (!Application.isPlaying)
        {
            DrawMapInEditor();
        }
    }
    void OnTextureValuesUpdated()
    {
        textureData.ApplyToMaterial(terrainMaterial);
    }

    private void Start()
    {
        if (imageMode == ImageMode.FromWebcam)
        {
            if (!String.IsNullOrEmpty(requestedDeviceName))
            {
                webcamtex = new WebCamTexture(requestedDeviceName, mapChunkSize, mapChunkSize);
            }

            else
                webcamtex = new WebCamTexture(mapChunkSize, mapChunkSize);

            Debug.Log("Webcam width: " + webcamtex.width + ". Webcam height: " + webcamtex.height);
            webcamtex.Play();        
        }        
    }

    private void Update()
    {
        //DrawMapInEditor();
    }

    public void DrawMapInEditor()
    {
        MapData mapData = GenerateMapData(Vector2.zero);
        MapDisplay display = FindObjectOfType<MapDisplay>();
        if (drawMode == DrawMode.NoiseMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(mapData.heightMap));
        }
        else if (drawMode == DrawMode.ColorMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromColorMap(mapData.colorMap, mapChunkSize, mapChunkSize));
        }
        else if (drawMode == DrawMode.Mesh)
        {
            display.DrawMesh(MeshGenerator.GenerateTerrainMesh(mapData.heightMap, terrainData.meshHeightMultiplier, terrainData.meshHeightCurve, levelOfDetail)/*, TextureGenerator.TextureFromColorMap(colorMap, mapChunkSize, mapChunkSize)*/);
        }        
    }

    public MeshData RequestMeshData(Vector2 center)
    {
        MapData mapData = GenerateMapData(center);

        return MeshGenerator.GenerateTerrainMesh(mapData.heightMap, terrainData.meshHeightMultiplier, terrainData.meshHeightCurve, levelOfDetail);
    }

    MapData GenerateMapData(Vector2 center)
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(mapChunkSize, mapChunkSize, noiseData.seed, noiseData.noiseScale, noiseData.octaves, noiseData.persistance, noiseData.lacunarity, center + noiseData.offset, noiseData.normalizeMode);
        Color[] colorMap = new Color[mapChunkSize * mapChunkSize];

        if (imageMode == ImageMode.FromWebcam && Application.isPlaying)
        {
            Texture2D texture2DFromCamera = new Texture2D(mapChunkSize, mapChunkSize);
            
            for (int y = 0; y < mapChunkSize; y++)
            {
                for (int x = 0; x < mapChunkSize; x++)
                {
                    texture2DFromCamera.SetPixel(x, y, webcamtex.GetPixel(x, y));
                }
            }
            texture2DFromCamera.Apply();

            dfdf //I want to try removing this next line and just pass in texture2DFromCamera to TextureToNoise and see what happens
            //Intent: simplify things while I'm trying to figure out how to get the mesh's to only display a portion of the webcam feed
            //instead of all of it at once
            Texture2D noisedTex = TextureGenerator.ApplyNoiseToTexture(texture2DFromCamera, noiseMap, noiseWeight, minGreyValue);

            //Need to somehow translate the center + offset in GenerateNoiseMap to the noiseMap generated here
            noiseMap = TextureGenerator.TextureToNoise(noisedTex);
        }

        else if (imageMode == ImageMode.FromImage)
        {
            Texture2D noisedTex = TextureGenerator.ApplyNoiseToTexture(imageTex, noiseMap, noiseWeight, minGreyValue);
            noiseMap = TextureGenerator.TextureToNoise(noisedTex);
        }        
       
        textureData.UpdateMeshHeights(terrainMaterial, terrainData.minHeight, terrainData.maxHeight);

        return new MapData(noiseMap, colorMap);
    }

    private void OnValidate()
    {
        /*if (mapChunkSize < 1)
            mapWidth = 1;
        if (mapChunkSize < 1)
            mapHeight = 1;*/
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

public struct MapData
{
    public readonly float[,] heightMap;
    public readonly Color[] colorMap;
    public MapData(float[,] heightMap, Color[] colorMap)
    {
        this.heightMap = heightMap;
        this.colorMap = colorMap;
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