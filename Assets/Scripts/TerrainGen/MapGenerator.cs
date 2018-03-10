using UnityEngine;
using System;
using TerrainGenData;


//I think we can delete colormap?
public class MapGenerator : MonoBehaviour {

    public enum DrawMode { NoiseMap, Mesh };
    public enum ImageMode { PureNoise, FromImage, FromWebcam}
    [TooltipAttribute("Set the name of the device to use.")]
    
    public DrawMode drawMode;

    public DataForTerrain terrainData;
    public NoiseData noiseData;
    public TextureData textureData;

    public Material terrainMaterial;

    public ImageMode imageMode;
    public Texture2D imageTex;

    [Range(0, 1)]
    public float minGreyValue;
    public float noiseWeight;

    public const int mapChunkSize = 241;
    [Range(0,6)]
    public int levelOfDetail;

    //need to create getters? need to make it so these values can't be changed once they're set in Start()
    public int mapChunkWidth;
    public int mapChunkHeight;    


    public bool autoUpdate;    

    private float[,] noiseMap;

    public WebcamTextureHandler webcamHandler;

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
        if (imageMode == ImageMode.FromImage || imageMode == ImageMode.FromWebcam)
        {
            if(imageMode == ImageMode.FromWebcam)
            {
                webcamHandler = FindObjectOfType<WebcamTextureHandler>();

                webcamHandler.Initialize();

                mapChunkWidth = webcamHandler.webcamRequestedWidth;
                mapChunkHeight = webcamHandler.webcamRequestedHeight;
            }

            else
            {
                mapChunkWidth = imageTex.width;
                mapChunkHeight = imageTex.height;
            }

            while (mapChunkWidth > 250 || mapChunkHeight > 250)
            {
                if (mapChunkWidth % 2 != 0)
                    Debug.Log("Width is not evenly divisble by 2");

                mapChunkWidth /= 2;

                if (mapChunkHeight % 2 != 0)
                    Debug.Log("Height is not evenly divisble by 2");

                mapChunkHeight /= 2;         
            }

            //I forget why, but Sebastion explains in a video that these variables need to be chunk size + 1
            mapChunkWidth = mapChunkWidth + 1;
            mapChunkHeight = mapChunkHeight + 1;            
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
        else if (drawMode == DrawMode.Mesh)
        {
            display.DrawMesh(MeshGenerator.GenerateTerrainMesh(mapData.heightMap, terrainData.meshHeightMultiplier, terrainData.meshHeightCurve, levelOfDetail)/*, TextureGenerator.TextureFromColorMap(colorMap, mapChunkSize, mapChunkSize)*/);
        }        
    }

    public MeshData RequestMeshData(Vector2 coord)
    {
        MapData mapData = GenerateMapData(coord);

        return MeshGenerator.GenerateTerrainMesh(mapData.heightMap, terrainData.meshHeightMultiplier, terrainData.meshHeightCurve, levelOfDetail);
    }

    MapData GenerateMapData(Vector2 coord)
    {
        //Color[] colorMap = new Color[mapChunkSize * mapChunkSize];


        if(imageMode == ImageMode.PureNoise)
        {
            noiseMap =  Noise.GenerateNoiseMap(mapChunkSize, mapChunkSize, noiseData.seed, noiseData.noiseScale, noiseData.octaves, noiseData.persistance, noiseData.lacunarity, coord + noiseData.offset, noiseData.normalizeMode);
        }

        else if (imageMode == ImageMode.FromImage && Application.isPlaying)
        {
            noiseMap = TextureGenerator.TextureToNoiseChunk(imageTex, coord, mapChunkWidth, mapChunkHeight);
        }

        else if (imageMode == ImageMode.FromWebcam && Application.isPlaying)
        {
            noiseMap = TextureGenerator.WebcamTextureToNoiseChunk(webcamtex, coord, mapChunkWidth, mapChunkHeight);
        }
        
        else //added this because it won't let you use noiseMap below otherwise. this should probably be refactored
        {
            noiseMap = Noise.GenerateNoiseMap(mapChunkSize, mapChunkSize, noiseData.seed, noiseData.noiseScale, noiseData.octaves, noiseData.persistance, noiseData.lacunarity, coord + noiseData.offset, noiseData.normalizeMode);
        }
       
        textureData.UpdateMeshHeights(terrainMaterial, terrainData.minHeight, terrainData.maxHeight);

        return new MapData(noiseMap);
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
    //public readonly Color[] colorMap;
    public MapData(float[,] heightMap)
    {
        this.heightMap = heightMap;
        //this.colorMap = colorMap;
    }
}