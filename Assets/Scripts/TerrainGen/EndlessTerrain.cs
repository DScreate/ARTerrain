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

    private int chunkWidth;
    private int chunkHeight;

    private int numChunkWidth;
    private int numChunkHeight;

    private FaceDetection _face;

    //int chunksVisibleInViewDst;

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

        _face = FindObjectOfType<FaceDetection>();

        numChunkWidth = mapGenerator.NumChunkWidth;
        numChunkHeight = mapGenerator.NumChunkHeight;

        terrainChunkArray = new TerrainChunk[numChunkWidth, numChunkHeight];

        InitializeChunks();
    }

    void Update()
    {
            if (webcamController.DidUpdateThisFrame())
            {
                _face.UpdateFaceTexture();

                mapGenerator.UpdateFullNoiseMap();

                foreach (TerrainChunk terrainChunk in terrainChunkArray)
                {
                    terrainChunk.UpdateTerrainChunk();
                }               
            }
        
    }

    void InitializeChunks()
    {
        for (int y = 0; y < numChunkHeight; y++)
        {
            for (int x = 0; x < numChunkWidth; x++)
            {
                terrainChunkArray[y, x] = new TerrainChunk(new Vector2(x, y), chunkWidth, chunkHeight, transform, mapMaterial);
            }
        }
    }

    public class TerrainChunk
    {
        GameObject meshObject;
        Vector2 coord;

        MeshRenderer meshRenderer;
        MeshFilter meshFilter;

        public TerrainChunk(Vector2 coord, int width, int height, Transform parent, Material material)
        {
            this.coord = coord;

            Vector2 position;
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
            MeshData meshData = mapGenerator.RequestMeshData(coord);

            if (meshFilter == null)
                meshFilter.mesh = meshData.CreateMesh();

            else
                meshData.UpdateMesh(meshFilter.mesh);
        }
    }
}
