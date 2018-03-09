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
    int chunksVisibleInViewDst;

    Dictionary<Vector2, TerrainChunk> terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();

    //don't think we need, sebastion uses to remove chunks that are no longer visible
    List<TerrainChunk> terrainChunksVisibleLastUpdate = new List<TerrainChunk>();

    void Start()
    {
        mapGenerator = FindObjectOfType<MapGenerator>();
        chunkSize = MapGenerator.mapChunkSize - 1;

        //Number of chunks viewable equal to how many times we can divide the chunk size into the view distance
        chunksVisibleInViewDst = Mathf.RoundToInt(maxViewDst / chunkSize);

        viewerPosition = new Vector2(viewer.position.x, viewer.position.z);

        InitializeChunks();
    }

    //I think we can get rid of this update method?
    void Update()
    {        
        if (mapGenerator.imageMode == MapGenerator.ImageMode.FromWebcam)
            UpdateVisibleChunks();
    }

    void InitializeChunks()
    {
        int currentChunkCoordX = Mathf.RoundToInt(viewerPosition.x / chunkSize);
        int currentChunkCoordY = Mathf.RoundToInt(viewerPosition.y / chunkSize);

        //for all surrounding chunks
        for (int yOffset = -chunksVisibleInViewDst; yOffset <= chunksVisibleInViewDst; yOffset++)
        {
            for (int xOffset = -chunksVisibleInViewDst; xOffset <= chunksVisibleInViewDst; xOffset++)
            {
                //position of a terrain chunk surrounding the viewer's terrain chunk?
                //I think we need to change so that it's iterating through 2D array of chunks (number of chunks in rows and columns) and start from 0,0 and go left to right then go down the rows instead of using negatives and iterating around 0,0
                Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);

                terrainChunkDictionary.Add(viewedChunkCoord, new TerrainChunk(viewedChunkCoord, chunkSize, transform, mapMaterial));
            }
        }
    }

    //I think the problem is this. Chunks should always be updating when webcam is being used.
    void UpdateVisibleChunks()
    {
        foreach(KeyValuePair<Vector2, TerrainChunk> pair in terrainChunkDictionary) {
            TerrainChunk terrainChunk = pair.Value;
            terrainChunk.UpdateTerrainChunk();
        }
    }

    public class TerrainChunk
    {
        GameObject meshObject;
        Vector2 position;

        //don't need? used for enabling/disabling chunks
       // Bounds bounds;

        MeshRenderer meshRenderer;
        MeshFilter meshFilter;

        //MapData mapData;

        //parent is used to make the new planes/chunks children of the viewer. we don't have a viewer so probably don't need this unless we make a game object to be the parent
        public TerrainChunk(Vector2 coord, int size, Transform parent, Material material)
        {
            position = coord * size;
            //bounds = new Bounds(position, Vector2.one * size);
            Vector3 positionV3 = new Vector3(position.x, 0, position.y);

            meshObject = new GameObject("Terrain Chunk");
            meshRenderer = meshObject.AddComponent<MeshRenderer>();
            meshFilter = meshObject.AddComponent<MeshFilter>();
            meshRenderer.material = material;

            meshObject.transform.position = positionV3;
            meshObject.transform.parent = parent;

            //Need to get mapData?

            MeshData meshData = mapGenerator.RequestMeshData(position);

            meshFilter.mesh = meshData.CreateMesh();
        }

        public void UpdateTerrainChunk()
        {
            MeshData meshData = mapGenerator.RequestMeshData(position);
            meshFilter.mesh = meshData.CreateMesh();
        }
    }
}
