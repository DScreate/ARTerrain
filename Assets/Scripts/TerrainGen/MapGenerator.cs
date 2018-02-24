using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ColorTracking;
using System;
using System.Threading;

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

    public const int mapChunkSize = 241;
    [Range(0,6)]
    public int levelOfDetail;
    //public int mapChunkSize;
    //public int mapChunkSize;
    //public int mapChunkWidth;
    //public int mapChunkHeight;
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

    private Queue<MapThreadInfo<MapData>> mapDataThreadInfoQueue = new Queue<MapThreadInfo<MapData>>();
    private Queue<MapThreadInfo<MeshData>> meshDataThreadInfoQueue = new Queue<MapThreadInfo<MeshData>>();

    WebCamTexture _webcamtex;
    Texture2D _TextureFromCamera;
    ObjectTrackingBasedOnColor _trackObjectsBasedOnColor;

    private void Start()
    {
        /*
        if (imageMode == ImageMode.FromWebcam || imageMode == ImageMode.FromOpenCV)
        {
            //_TextureFromCamera = new Texture2D(mapWidth, mapHeight);

            if (!String.IsNullOrEmpty(requestedDeviceName))
            {
                _webcamtex = new WebCamTexture(requestedDeviceName, mapChunkSize, mapChunkSize);
            }

            else
                _webcamtex = new WebCamTexture(mapWidth, mapHeight);

            _webcamtex.Play();

            if (imageMode == ImageMode.FromOpenCV)            
                _trackObjectsBasedOnColor = new ObjectTrackingBasedOnColor(_webcamtex);            
        }   
        */
    }

    public void DrawMapInEditor()
    {
        MapData mapData = this.GenerateMapData();
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
            display.DrawMesh(MeshGenerator.GenerateTerrainMesh(mapData.heightMap, meshHeightMultiplier, meshHeightCurve, levelOfDetail), TextureGenerator.TextureFromColorMap(mapData.colorMap, mapChunkSize, mapChunkSize));
        }
    }

    public void RequestMapData(Action<MapData> callback)
    {
        ThreadStart threadStart = delegate { MapDataThread(callback); };

        new Thread(threadStart).Start();
    }

    void MapDataThread(Action<MapData> callback)
    {
        MapData mapData = GenerateMapData();
        lock (mapDataThreadInfoQueue)
        {
            mapDataThreadInfoQueue.Enqueue(new MapThreadInfo<MapData>(callback, mapData));
        }
    }

    public void RequestMeshData(MapData mapData, Action<MeshData> callback)
    {
        ThreadStart threadStart = delegate {
                MeshDataThread(mapData, callback);
            };
        new Thread(threadStart).Start();
    }

    void MeshDataThread(MapData mapData, Action<MeshData> callback)
    {
        MeshData meshData = MeshGenerator.GenerateTerrainMesh(
            mapData.heightMap,
            meshHeightMultiplier,
            meshHeightCurve,
            levelOfDetail);
        lock (meshDataThreadInfoQueue)
        {
            meshDataThreadInfoQueue.Enqueue(new MapThreadInfo<MeshData>(callback, meshData));
        }
    }

    private void Update()
    {

        if (mapDataThreadInfoQueue.Count > 0)
        {
            for (int i = 0; i < mapDataThreadInfoQueue.Count; i++)
            {
                MapThreadInfo<MapData> threadInfo = mapDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }

        if (meshDataThreadInfoQueue.Count > 0)
        {
            for (int i = 0; i < meshDataThreadInfoQueue.Count; i++)
            {
                MapThreadInfo<MeshData> threadInfo = meshDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }
        /*
        if (imageMode == ImageMode.FromWebcam)
        {
            for (int y = 0; y < mapChunkSize; y++)
            {
                for (int x = 0; x < mapChunkSize; x++)
                {
                    _TextureFromCamera.SetPixel(x, y, _webcamtex.GetPixel(x, y));
                }
            }
            _TextureFromCamera.Apply();
            GenerateMapData();
        }

        if (imageMode == ImageMode.FromOpenCV)
        {
            _TextureFromCamera = _trackObjectsBasedOnColor.UpdateGrayScale();
            //_TextureFromCamera = _trackObjectsBasedOnColor.UpdateGrayScale(_webcamtex);
            //_TextureFromCamera = new Texture2D(_webcamtex.width, _webcamtex.height, TextureFormat.RGB24, false);
            //_trackObjectsBasedOnColor.UpdateGrayScale(_webcamtex, _TextureFromCamera);
            GenerateMapData();
        }
        */


    }

    MapData GenerateMapData()
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(mapChunkSize, mapChunkSize, seed, noiseScale, octaves, persistance, lacunarity, offset);

        //if (imageMode == ImageMode.PureNoise)
        //{
            Color[] colorMap = new Color[mapChunkSize * mapChunkSize];

            for (int y = 0; y < mapChunkSize; y++)
            {
                for (int x = 0; x < mapChunkSize; x++)
                {
                    float currentHeight = noiseMap[x, y];
                    for (int i = 0; i < regions.Length; i++)
                    {
                        if (currentHeight <= regions[i].height)
                        {
                            colorMap[y * mapChunkSize + x] = regions[i].color;
                            break;
                        }
                    }
                }
            }

            return new MapData(noiseMap, colorMap);
        //}
        /*
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
        */
    }

    private void OnValidate()
    {
        if (lacunarity < 1)
            lacunarity = 1;
        if (octaves < 0)
            octaves = 0;
    }

    struct MapThreadInfo<T>
    {
        public readonly Action<T> callback;

        public readonly T parameter;

        public MapThreadInfo(Action<T> callback, T parameter)
        {
            this.callback = callback;
            this.parameter = parameter;
        }
    }
}

[System.Serializable]
public struct TerrainType
{
    public string name;
    public float height;
    public Color color;
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
