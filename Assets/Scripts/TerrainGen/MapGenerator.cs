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
    public float noiseWeight;

    public const int mapChunkSize = 241;
    [Range(0,6)]
    public int levelOfDetail;

    //need to create getters? need to make it so these values can't be changed once they're set in Start()
    public int mapChunkWidth;
    public int mapChunkHeight;

    public int webcamRequestedWidth;
    public int webcamRequestedHeight;

    //public Vector2 numberOfChunks;

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
        if (imageMode == ImageMode.FromImage || imageMode == ImageMode.FromWebcam)
        {
            int width;
            int height;

            if (imageMode == ImageMode.FromWebcam)
            {
                /*if (!String.IsNullOrEmpty(requestedDeviceName))
                {
                    webcamtex = new WebCamTexture(requestedDeviceName, webcamRequestedWidth, webcamRequestedHeight);
                }

                else
                    webcamtex = new WebCamTexture(webcamRequestedWidth, webcamRequestedHeight);*/

                webcamtex = new WebCamTexture(640, 480);

                Debug.Log("Webcam request width: " + webcamtex.requestedWidth + ". Webcam requested height: " + webcamtex.requestedHeight);

                webcamRequestedWidth = webcamtex.requestedWidth;
                webcamRequestedHeight = webcamtex.requestedHeight;

                webcamtex.Play();

                width = webcamtex.requestedWidth;
                height = webcamtex.requestedHeight;
            }

            else
            {
                width = imageTex.width;
                height = imageTex.height;
            }

            while (width > 250 || height > 250)
            {
                if (width % 2 != 0)
                    Debug.Log("Width is not evenly divisble by 2");

                width /= 2;

                if (height % 2 != 0)
                    Debug.Log("Height is not evenly divisble by 2");

                height /= 2;         
            }

            //I forget why, but Sebastion explains in a video that these variables need to be chunk size + 1
            mapChunkWidth = width + 1;
            mapChunkHeight = height + 1;            
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

    public MeshData RequestMeshData(Vector2 coord)
    {
        MapData mapData = GenerateMapData(coord);

        return MeshGenerator.GenerateTerrainMesh(mapData.heightMap, terrainData.meshHeightMultiplier, terrainData.meshHeightCurve, levelOfDetail);
    }  

    MapData GenerateMapData(Vector2 coord)
    {
        //Change this so it only create noise map if ImageMode is PureNoise?
        float[,] noiseMap;
        Color[] colorMap = new Color[mapChunkSize * mapChunkSize];

        if(imageMode == ImageMode.PureNoise)
        {
            noiseMap =  Noise.GenerateNoiseMap(mapChunkSize, mapChunkSize, noiseData.seed, noiseData.noiseScale, noiseData.octaves, noiseData.persistance, noiseData.lacunarity, coord + noiseData.offset, noiseData.normalizeMode);
        }

        else if (imageMode == ImageMode.FromWebcam && Application.isPlaying)
        {
            Texture2D texture2DFromCamera = new Texture2D(webcamtex.requestedWidth, webcamtex.requestedHeight);
            
            for (int y = 0; y < texture2DFromCamera.height; y++)
            {
                for (int x = 0; x < texture2DFromCamera.height; x++)
                {
                    texture2DFromCamera.SetPixel(x, y, webcamtex.GetPixel(x, y));
                }
            }
            texture2DFromCamera.Apply();

            //Commented out to simplify things while I'm trying to figure out how to get the mesh's to only display a portion of the webcam feed
            //instead of all of it at once
            //Texture2D noisedTex = TextureGenerator.ApplyNoiseToTexture(texture2DFromCamera, noiseMap, noiseWeight, minGreyValue);

            //Need to somehow translate the center + offset in GenerateNoiseMap to the noiseMap generated here
            //noiseMap = TextureGenerator.TextureToNoise(noisedTex);

            noiseMap = TextureGenerator.TextureToNoiseChunk(texture2DFromCamera, coord, mapChunkWidth, mapChunkHeight);
        }

        else if (imageMode == ImageMode.FromImage && Application.isPlaying)
        {
            noiseMap = TextureGenerator.TextureToNoiseChunk(imageTex, coord, mapChunkWidth, mapChunkHeight);

            //Texture2D noisedTex = TextureGenerator.ApplyNoiseToTexture(imageTex, noiseMap, noiseWeight, minGreyValue);
            //noiseMap = TextureGenerator.TextureToNoise(noisedTex);
        }   
        
        else //added this because it won't let you use noiseMap below otherwise. this should probably be refactored
        {
            noiseMap = Noise.GenerateNoiseMap(mapChunkSize, mapChunkSize, noiseData.seed, noiseData.noiseScale, noiseData.octaves, noiseData.persistance, noiseData.lacunarity, coord + noiseData.offset, noiseData.normalizeMode);
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