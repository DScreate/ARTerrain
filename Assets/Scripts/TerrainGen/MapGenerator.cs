using UnityEngine;
using System;
using TerrainGenData;


//I think we can delete colormap?
public class MapGenerator : MonoBehaviour
{

    public enum DrawMode { NoiseMap, Mesh };
    public enum ImageMode { PureNoise, FromImage, FromWebcam }
    public Boolean FaceOnly = false;
    [TooltipAttribute("Set the name of the device to use.")]

    public DrawMode drawMode;

    public DataForTerrain terrainData;
    public NoiseData noiseData;
    public TextureData textureData;

    public Material terrainMaterial;

    public ImageMode imageMode = ImageMode.FromWebcam;
    public Texture2D imageTex;

    [Range(0, 1)]
    public float minGreyValue;
    [Range(0, 1)]
    public float noiseWeight;

    [Range(0, 6)]
    public int levelOfDetail;

    //need to create getters? need to make it so these values can't be changed once they're set in Start()

    private int mapChunkWidth = 241;
    private int mapChunkHeight = 241;

    private int mapWidth;
    private int mapHeight;

    public int MapChunkWidth
    {
        get { return mapChunkWidth; }
    }

    public int MapChunkHeight
    {
        get { return mapChunkHeight; }
    }

    public int MapWidth
    {
        get { return mapWidth; }
    }

    public int MapHeight
    {
        get { return mapHeight; }
    }

    public bool autoUpdate;

    public static WebcamTextureController webcamController;
    //public Texture2D textureForNoise;

    public float[,] chunkNoiseMap;
    public float[,] fullNoiseMap;
    private float[,] heightMap;

    public GameObject Water;

    void OnValuesUpdate()
    {
        //if(Application.isPlaying)
            UpdateWaterHeight();

        if (!Application.isPlaying)
        {
            DrawMapInEditor();
        }
    }
    public void OnTextureValuesUpdated()
    {
        textureData.ApplyToMaterial(terrainMaterial);

        if (Application.isPlaying)
            UpdateWaterHeight();
    }

    private void Start()
    {
        if (imageMode == ImageMode.FromImage || imageMode == ImageMode.FromWebcam)
        {
            if (imageMode == ImageMode.FromWebcam)
            {
                webcamController = gameObject.GetComponent(typeof(WebcamTextureController)) as WebcamTextureController;

                webcamController.Initialize();

                mapChunkWidth = webcamController.webcamRequestedWidth;
                mapChunkHeight = webcamController.webcamRequestedHeight;

                mapWidth = webcamController.webcamRequestedWidth;
                mapHeight = webcamController.webcamRequestedHeight;

                fullNoiseMap = NoiseGenerator.GenerateNoiseMap(webcamController.WebcamTex.width, webcamController.WebcamTex.height, noiseData.seed, noiseData.noiseScale, noiseData.octaves, noiseData.persistance, noiseData.lacunarity, noiseData.offset, noiseData.normalizeMode);
                //textureForNoise = new Texture2D(webcamController.WebcamTex.width, webcamController.WebcamTex.height);
            }

            else
            {
                mapChunkWidth = imageTex.width;
                mapChunkHeight = imageTex.height;

                mapWidth = imageTex.width;
                mapHeight = imageTex.height;
            }

            while (mapChunkWidth > 250 || mapChunkHeight > 250)
            {
                if (mapChunkWidth % 2 != 0)
                    Debug.Log("Width " + mapChunkWidth + " is not evenly divisble by 2");

                mapChunkWidth /= 2;

                if (mapChunkHeight % 2 != 0)
                    Debug.Log("Height " + mapChunkHeight + " is not evenly divisble by 2");

                mapChunkHeight /= 2;
            }

            //I forget why, but Sebastion explains in a video that these variables need to be chunk size + 1
            mapChunkWidth = mapChunkWidth + 1;
            mapChunkHeight = mapChunkHeight + 1;

            textureData.ApplyToMaterial(terrainMaterial);
        }
    }

    public void DrawMapInEditor()
    {
        heightMap = GenerateMapData(Vector2.zero);
        MapDisplay display = FindObjectOfType<MapDisplay>();

        if (drawMode == DrawMode.NoiseMap)
        {
            if (imageMode == ImageMode.FromWebcam)
            {
                heightMap = TextureGenerator.TextureToNoise(FindObjectOfType<FaceDetection>().FaceTexture, mapWidth, mapHeight);
            }
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(heightMap));
        }
        else if (drawMode == DrawMode.Mesh)
        {
            display.DrawMesh(MeshGenerator.GenerateTerrainMesh(heightMap, terrainData.meshHeightMultiplier, terrainData.meshHeightCurve, levelOfDetail)/*, TextureGenerator.TextureFromColorMap(colorMap, mapChunkSize, mapChunkSize)*/);
        }
    }

    public MeshData RequestMeshData(Vector2 chunkPosition)
    {
        heightMap = GenerateMapData(chunkPosition);

        return MeshGenerator.GenerateTerrainMesh(heightMap, terrainData.meshHeightMultiplier, terrainData.meshHeightCurve, levelOfDetail);
    }

    float[,] GenerateMapData(Vector2 chunkPosition)
    {
        if (imageMode == ImageMode.PureNoise)
        {
            chunkPosition.x *= mapChunkWidth;
            chunkPosition.y *= mapChunkHeight;
            chunkNoiseMap = NoiseGenerator.GenerateNoiseMap(mapChunkWidth, mapChunkHeight, noiseData.seed, noiseData.noiseScale, noiseData.octaves, noiseData.persistance, noiseData.lacunarity, chunkPosition + noiseData.offset, noiseData.normalizeMode);
        }

        else if (imageMode == ImageMode.FromImage && Application.isPlaying)
        {
            chunkNoiseMap = TextureGenerator.TextureToNoiseChunk(imageTex, chunkPosition, mapChunkWidth, mapChunkHeight);
        }

        else if (imageMode == ImageMode.FromWebcam && Application.isPlaying)
        {
            //noiseMap = TextureGenerator.WebcamTextureToNoiseChunk(webcamController.WebcamTex, coord, mapChunkWidth, mapChunkHeight);
            FaceDetection fd = FindObjectOfType<FaceDetection>();
            chunkNoiseMap = NoiseGenerator.LerpNoiseMapWithTextureToNoiseChunk(fd, fullNoiseMap, noiseWeight, minGreyValue, mapChunkWidth, mapChunkHeight, chunkPosition, FaceOnly);
            //chunkNoiseMap = TextureGenerator.TextureToNoiseChunk(textureForNoise, coord, mapChunkWidth, mapChunkHeight);
        }

        else
        {
            chunkNoiseMap = NoiseGenerator.GenerateNoiseMap(1, 1, 1, 1, 1, 1, 1, new Vector2(1, 1), noiseData.normalizeMode);
            Debug.Log("Generate map data error");
        }

        textureData.UpdateMeshHeights(terrainMaterial, terrainData.minHeight, terrainData.maxHeight);

        return chunkNoiseMap;
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