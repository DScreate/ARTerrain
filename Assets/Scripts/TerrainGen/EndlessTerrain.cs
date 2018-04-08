using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EndlessTerrain : MonoBehaviour
{

    //determines number of mesh's created around origin
    public const float pureNoiseChunks = 2;

    //this needs to change, maybe removed. It's used to attach a origin tranform to the script but we don't have a viwer
    public Transform origin;
    public static Vector2 viewerPosition;

    public Material mapMaterial;

    static MapGenerator mapGenerator;

    int chunkWidth;
    int chunkHeight;

    int chunksVisibleInViewDst;

    public Vector2 numberOfChunks;

    Dictionary<Vector2, TerrainChunk> terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();

    TerrainChunk[,] terrainChunkArray;

    //don't think we need, sebastion uses to remove chunks that are no longer visible
    List<TerrainChunk> terrainChunksVisibleLastUpdate = new List<TerrainChunk>();

    WebcamTextureController webcamController;

    void Start()
    {
        mapGenerator = FindObjectOfType<MapGenerator>();
        webcamController = gameObject.GetComponent<WebcamTextureController>();

        chunkWidth = mapGenerator.MapChunkWidth - 1;
        chunkHeight = mapGenerator.MapChunkHeight - 1;

        if (mapGenerator.imageMode == MapGenerator.ImageMode.PureNoise)
        {
            numberOfChunks.x = pureNoiseChunks;
            numberOfChunks.y = pureNoiseChunks;
        }

        else if (mapGenerator.imageMode == MapGenerator.ImageMode.FromImage)
        {
            numberOfChunks.x = mapGenerator.imageTex.width / chunkWidth;
            numberOfChunks.y = mapGenerator.imageTex.height / chunkHeight;
        }

        else if (mapGenerator.imageMode == MapGenerator.ImageMode.FromWebcam)
        {
            numberOfChunks.x = mapGenerator.MapWidth / chunkWidth;
            numberOfChunks.y = mapGenerator.MapHeight / chunkHeight;
        }

        terrainChunkArray = new TerrainChunk[(int)numberOfChunks.y, (int)numberOfChunks.x];

        InitializeChunks();

    }

    void Update()
    {
        if (mapGenerator.imageMode == MapGenerator.ImageMode.FromWebcam)
        {
            if (webcamController.DidUpdateThisFrame())
            {
                FindObjectOfType<FaceDetection>().UpdateFaceTexture();
                foreach (TerrainChunk terrainChunk in terrainChunkArray)
                {
                    terrainChunk.UpdateTerrainChunk();
                }
            }
        }
    }

    void InitializeChunks()
    {
        for (int y = 0; y < numberOfChunks.y; y++)
        {
            for (int x = 0; x < numberOfChunks.x; x++)
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

        public void UpdateTerrainChunk()
        {
            MeshData meshData = mapGenerator.RequestMeshData(position);

            if (meshFilter == null)
                meshFilter.mesh = meshData.CreateMesh();

            else
                meshData.UpdateMesh(meshFilter.mesh);
        }
    }
}
