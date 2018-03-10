using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EndlessTerrain : MonoBehaviour
{

    //determines number of mesh's created around viewer
    public const float maxViewDst = 250;

    //this needs to change, maybe removed. It's used to attach a viewer tranform to the script but we don't have a viwer
    public Transform viewer;

    public Material mapMaterial;

    public static Vector2 viewerPosition;
    static MapGenerator mapGenerator;
    int chunkSize;

    int chunkWidth;
    int chunkHeight;

    int chunksVisibleInViewDst;

    Vector2 numberOfChunks;

    Dictionary<Vector2, TerrainChunk> terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();

    TerrainChunk[,] terrainChunkArray;

    //don't think we need, sebastion uses to remove chunks that are no longer visible
    List<TerrainChunk> terrainChunksVisibleLastUpdate = new List<TerrainChunk>();

    void Start()
    {       
        mapGenerator = FindObjectOfType<MapGenerator>();

        if (mapGenerator.imageMode == MapGenerator.ImageMode.PureNoise)
        {
            chunkSize = MapGenerator.mapChunkSize - 1;

            //Number of chunks viewable equal to how many times we can divide the chunk size into the view distance
            chunksVisibleInViewDst = Mathf.RoundToInt(maxViewDst / chunkSize);

            viewerPosition = new Vector2(viewer.position.x, viewer.position.z);

            InitializeChunks();
        }

        else if(mapGenerator.imageMode == MapGenerator.ImageMode.FromImage || mapGenerator.imageMode == MapGenerator.ImageMode.FromWebcam)
        {
            //undoing when we added 1 in mapgen
            chunkWidth = mapGenerator.mapChunkWidth - 1;
            chunkHeight = mapGenerator.mapChunkHeight - 1;

            //Number of chunks equal to number of times width and height divide into our textures width and height

            if (mapGenerator.imageMode == MapGenerator.ImageMode.FromImage)
            {
                numberOfChunks.x = Mathf.RoundToInt(mapGenerator.imageTex.width / chunkWidth);
                numberOfChunks.y = Mathf.RoundToInt(mapGenerator.imageTex.height / chunkHeight);
            }
            else if(mapGenerator.imageMode == MapGenerator.ImageMode.FromWebcam)
            {
                numberOfChunks.x = Mathf.RoundToInt(mapGenerator.webcamRequestedWidth / chunkWidth);
                numberOfChunks.y = Mathf.RoundToInt(mapGenerator.webcamRequestedHeight / chunkHeight);
            }

            terrainChunkArray = new TerrainChunk[(int)numberOfChunks.y, (int)numberOfChunks.x];

            InitializeChunksForImageAndWebcam();
        }
    }

    //I think we can get rid of this update method?
    void Update()
    {
        if (mapGenerator.imageMode == MapGenerator.ImageMode.FromWebcam)
        {
            foreach (TerrainChunk terrainChunk in terrainChunkArray)
            {
                terrainChunk.UpdateTerrainChunk();
            }
        }
    }

    //sebastion's method for displaying noise map
    void InitializeChunks()
    {
        int currentChunkCoordX = Mathf.RoundToInt(viewerPosition.x / chunkSize);
        int currentChunkCoordY = Mathf.RoundToInt(viewerPosition.y / chunkSize);

        for (int yOffset = -chunksVisibleInViewDst; yOffset <= chunksVisibleInViewDst; yOffset++)
        {
            for (int xOffset = -chunksVisibleInViewDst; xOffset <= chunksVisibleInViewDst; xOffset++)
            {
                Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);

                terrainChunkDictionary.Add(viewedChunkCoord, new TerrainChunk(viewedChunkCoord, chunkSize, transform, mapMaterial));
            }
        }
    }

    void InitializeChunksForImageAndWebcam()
    {
        for(int y = 0; y < numberOfChunks.y; y++)
        {
            for(int x = 0; x < numberOfChunks.x; x++)
            {
                terrainChunkArray[y, x] = new TerrainChunk(new Vector2(x, y), chunkWidth, chunkHeight, transform, mapMaterial);
            }
        }
    }

    public class TerrainChunk
    {
        GameObject meshObject;
        Vector2 position;

        MeshRenderer meshRenderer;
        MeshFilter meshFilter;

        //parent is used to make the new planes/chunks children of the viewer. should change viewer to some other gameObject
        public TerrainChunk(Vector2 coord, int size, Transform parent, Material material)
        {
            position = coord * size;
            Vector3 positionV3 = new Vector3(position.x, 0, position.y);

            meshObject = new GameObject("Terrain Chunk");
            meshRenderer = meshObject.AddComponent<MeshRenderer>();
            meshFilter = meshObject.AddComponent<MeshFilter>();
            meshRenderer.material = material;

            meshObject.transform.position = positionV3;
            meshObject.transform.parent = parent;

            MeshData meshData = mapGenerator.RequestMeshData(position);

            meshFilter.mesh = meshData.CreateMesh();
        }

        public void UpdateTerrainChunk()
        {
            MeshData meshData = mapGenerator.RequestMeshData(position);
            meshFilter.mesh = meshData.CreateMesh();
        }

        public TerrainChunk(Vector2 coord, int width, int height, Transform parent, Material material)
        {
            position.x = coord.x * width;
            position.y = coord.y * height;

            meshObject = new GameObject("Terrain Chunk");
            meshRenderer = meshObject.AddComponent<MeshRenderer>();
            meshFilter = meshObject.AddComponent<MeshFilter>();
            meshRenderer.material = material;

            meshObject.transform.position = new Vector3(position.x, 0, position.y);
            meshObject.transform.parent = parent;

            //Something might be off with what I did. I'm not sure why I have to rotate the mesh atm but if I don't it's incorrect.
            meshObject.transform.Rotate(new Vector3(0, 180, 0));

            MeshData meshData = mapGenerator.RequestMeshData(coord);

            meshFilter.mesh = meshData.CreateMesh();
        }
    }
}
