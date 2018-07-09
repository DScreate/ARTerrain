using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// Handles the creation and update of all the meshes by delegating to a collection of TerrainChunk.
/// </summary>
/// <remarks>
/// Code modified from Sebastian Lague's video, Procedural Landmass Generation.
/// This class acts similar to a driver for the classes of the program that are involved in generating the terrain.
/// It has references to and calls updates to other classes. 
/// <remarks/>
[RequireComponent(typeof(WebcamTextureController), typeof(MapGenerator), typeof(FaceDetection))]
public class EndlessTerrain : MonoBehaviour
{
    [Tooltip("Assign the material to be used by the meshes.")]
    public Material mapMaterial;

    static MapGenerator mapGenerator;

    private int chunkWidth;
    private int chunkHeight;

    private int numChunkWidth;
    private int numChunkHeight;

    private FaceDetection _face;

    TerrainChunk[,] terrainChunkArray;

    WebcamTextureController webcamController;
    /// <summary>
    /// Initializes and assigns values to this class' data members.
    /// </summary>
    void Start()
    {
        mapGenerator = gameObject.GetComponent(typeof(MapGenerator)) as MapGenerator;
        webcamController = gameObject.GetComponent(typeof(WebcamTextureController)) as WebcamTextureController;

        chunkWidth = mapGenerator.MapChunkWidth - 1;
        chunkHeight = mapGenerator.MapChunkHeight - 1;

        _face = gameObject.GetComponent(typeof(FaceDetection)) as FaceDetection;

        numChunkWidth = mapGenerator.NumChunkWidth;
        numChunkHeight = mapGenerator.NumChunkHeight;

        terrainChunkArray = new TerrainChunk[numChunkWidth, numChunkHeight];

        InitializeChunks();
    }
    /// <summary>
    /// Calls the updates to the FaceTexture, MapGenerator and TerrainChunks if WebcamTexture updated this frame.
    /// </summary>
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
    /// <summary>
    /// This class handles the dynamic creation and update of an individual mesh.
    /// </summary>
    public class TerrainChunk
    {
        GameObject meshObject;
        Vector2 coord;

        MeshRenderer meshRenderer;
        MeshFilter meshFilter;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="coord">Used to determine where chunk should be generated. Uses positive (x,y) coordinates.</param>
        /// <param name="width">Width of chunk.</param>
        /// <param name="height">Height of chunk.</param>
        /// <param name="parent">Transform of class creating TerrainChunk.</param>
        /// <param name="material">Used by MeshRenderer.</param>
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

            //If we don't rotate the mesh, it will be upside down.
            meshObject.transform.Rotate(new Vector3(0, 180, 0));

            MeshData meshData = mapGenerator.RequestMeshData(coord);

            meshFilter.mesh = meshData.CreateMesh();
        }
        /// <summary>
        /// Gets updated MeshData and delegates update of mesh to MeshData class.
        /// </summary>
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
