using UnityEngine;
using System;
using TerrainGenData;


//I think we can delete colormap?
public class MapGenerator : MonoBehaviour
{
    //[Tooltip("Set the name of the device to use.")]

    public DataForTerrain terrainData;
    public NoiseData noiseData;
    public TextureData textureData;

    public Material terrainMaterial;

    public static WebcamTextureController webcamController;
    public GameObject Water;


    [Range(0, 1)]
    public float minGreyValue;
    [Range(0, 1)]
    public float noiseWeight;

    [Range(0, 6)]
    public int levelOfDetail;

    //need to create getters? need to make it so these values can't be changed once they're set in Start()    

    private int mapWidth;
    private int mapHeight;

    private int mapChunkWidth;
    private int mapChunkHeight;

    private int numChunkWidth;
    private int numChunkHeight;

    public int MapChunkWidth
    {
        get
        {
            return mapChunkWidth;
        }
    }

    public int MapChunkHeight
    {
        get
        {
            return mapChunkHeight;
        }
    }

    public int MapWidth
    {
        get
        {
            return mapWidth;
        }
    }

    public int MapHeight
    {
        get
        {
            return mapHeight;
        }
    }
    public int NumChunkWidth
    {
        get
        {
            return numChunkWidth;
        }
    }

    public int NumChunkHeight
    {
        get
        {
            return numChunkHeight;
        }
    }

    private float[,] chunkNoiseMap;
    private float[,] fullNoiseMap;
    private float[,] heightMap;

    private int test;

    private void Start()
    {       
        webcamController = gameObject.GetComponent(typeof(WebcamTextureController)) as WebcamTextureController;

        webcamController.Initialize();

        InitializeMapDimensions();

        fullNoiseMap = NoiseGenerator.GenerateNoiseMap(mapWidth, mapHeight, noiseData.seed, noiseData.noiseScale, noiseData.octaves, noiseData.persistance, noiseData.lacunarity, noiseData.offset, noiseData.normalizeMode);

        textureData.ApplyToMaterial(terrainMaterial);        
    }

    //I forget why, but Sebastion explains in a video that these variables need to be chunk size + 1
    private void InitializeMapDimensions()
    {                      
        InitializeWidthData();
        InitializeHeightData();
    }

    private void InitializeWidthData()
    {
        mapWidth = webcamController.WebcamWidth;

        mapChunkWidth = InitializeChunkData(mapWidth);

        numChunkWidth = mapWidth / mapChunkWidth;
    }
    private void InitializeHeightData()
    {
        mapHeight = webcamController.WebcamHeight;

        mapChunkHeight = InitializeChunkData(mapHeight);

        numChunkHeight = mapHeight / mapChunkHeight;
    }

    private int InitializeChunkData(int chunkSize)
    {
        while (chunkSize > 250)
        {
            if (chunkSize % 2 != 0)
                Debug.Log("Chunk size " + chunkSize + " is not evenly divisble by 2");

            chunkSize /= 2;
        }

        //I forget why, but Sebastion explains in a video that these variables need to be chunk size + 1
        return chunkSize++;
    }

    public MeshData RequestMeshData(Vector2 chunkPosition)
    {
        heightMap = GenerateMapData(chunkPosition);

        return MeshGenerator.GenerateTerrainMesh(heightMap, terrainData.meshHeightMultiplier, terrainData.meshHeightCurve, levelOfDetail);
    }

    float[,] GenerateMapData(Vector2 chunkPosition)
    {
        if (Application.isPlaying)
        {   
            chunkNoiseMap = NoiseGenerator.LerpNoiseMapWithTextureToNoiseChunk(FindObjectOfType<FaceDetection>().FaceTexture, fullNoiseMap, noiseWeight, minGreyValue, mapChunkWidth, mapChunkHeight, chunkPosition);
        }

        else
        {
            chunkNoiseMap = NoiseGenerator.GenerateNoiseMap(1, 1, 1, 1, 1, 1, 1, new Vector2(1, 1), noiseData.normalizeMode);
            Debug.Log("Generate map data error");
        }

        textureData.UpdateMeshHeights(terrainMaterial, terrainData.minHeight, terrainData.maxHeight);

        return chunkNoiseMap;
    }
    void OnValuesUpdate()
    {
        UpdateWaterHeight();
    }
    public void OnTextureValuesUpdated()
    {
        textureData.ApplyToMaterial(terrainMaterial);

        UpdateWaterHeight();
    }
    public void UpdateWaterHeight()
    {
        if (Water == null)
            Water = GameObject.Find("WaterProDaytime");

        float newHeight = textureData.layers[1].startHeight * terrainData.meshHeightMultiplier * terrainData.uniformScale;
        Vector3 curPos = Water.transform.position;
        Water.transform.position = new Vector3(curPos.x, newHeight, curPos.z);
    }

    public void UpdateFullNoiseMap()
    {
        if (noiseData.Updated)
        {
            fullNoiseMap = NoiseGenerator.GenerateNoiseMap(webcamController.WebcamTex.width, webcamController.WebcamTex.height, noiseData.seed, noiseData.noiseScale, noiseData.octaves, noiseData.persistance, noiseData.lacunarity, noiseData.offset, noiseData.normalizeMode);
            noiseData.Updated = false;
        }
    }

    private void OnValidate()
    {
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