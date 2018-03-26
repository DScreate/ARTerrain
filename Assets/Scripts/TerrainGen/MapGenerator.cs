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

    public ImageMode imageMode = ImageMode.FromWebcam;
    public Texture2D imageTex;

    /*[Range(0, 1)]
    public float minGreyValue;
    public float noiseWeight;*/
    
    [Range(0,6)]
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

    static WebcamTextureController webcamController;

    private float[,] noiseMap;
    private float[,] heightMap;

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
                webcamController = FindObjectOfType<WebcamTextureController>();

                webcamController.Initialize();

                mapChunkWidth = webcamController.webcamRequestedWidth;
                mapChunkHeight = webcamController.webcamRequestedHeight;

                mapWidth = webcamController.webcamRequestedWidth;
                mapHeight = webcamController.webcamRequestedHeight;
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
        }
    }

    public void DrawMapInEditor()
    {
        heightMap = GenerateMapData(Vector2.zero);
        MapDisplay display = FindObjectOfType<MapDisplay>();
        
        if (drawMode == DrawMode.NoiseMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(heightMap));
        }
        else if (drawMode == DrawMode.Mesh)
        {
            display.DrawMesh(MeshGenerator.GenerateTerrainMesh(heightMap, terrainData.meshHeightMultiplier, terrainData.meshHeightCurve, levelOfDetail)/*, TextureGenerator.TextureFromColorMap(colorMap, mapChunkSize, mapChunkSize)*/);
        }        
    }

    public MeshData RequestMeshData(Vector2 coord)
    {
        heightMap = GenerateMapData(coord);

        return MeshGenerator.GenerateTerrainMesh(heightMap, terrainData.meshHeightMultiplier, terrainData.meshHeightCurve, levelOfDetail);
    }

    float[,] GenerateMapData(Vector2 coord)
    {
        if(imageMode == ImageMode.PureNoise)
        {
            coord.x *= mapChunkWidth;
            coord.y *= mapChunkHeight;
            noiseMap =  Noise.GenerateNoiseMap(mapChunkWidth, mapChunkHeight, noiseData.seed, noiseData.noiseScale, noiseData.octaves, noiseData.persistance, noiseData.lacunarity, coord + noiseData.offset, noiseData.normalizeMode);
        }

        else if (imageMode == ImageMode.FromImage && Application.isPlaying)
        {
            noiseMap = TextureGenerator.TextureToNoiseChunk(imageTex, coord, mapChunkWidth, mapChunkHeight);
        }

        else if (imageMode == ImageMode.FromWebcam && Application.isPlaying)
        {
            noiseMap = TextureGenerator.WebcamTextureToNoiseChunk(webcamController.Webcamtex, coord, mapChunkWidth, mapChunkHeight);
            noiseMap = TextureGenerator.WebcamTextureToNoiseChunk(webcamController.Webcamtex, coord, mapChunkWidth, mapChunkHeight);
        }
        
         else
        {
            noiseMap = Noise.GenerateNoiseMap(1, 1,1,1,1,1, 1, new Vector2(1, 1), noiseData.normalizeMode);
            Debug.Log("Generate map data error");
        }
       
        textureData.UpdateMeshHeights(terrainMaterial, terrainData.minHeight, terrainData.maxHeight);

        return noiseMap;
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