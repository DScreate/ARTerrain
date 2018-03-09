using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EndlessTerrain : MonoBehaviour
{

    //max size of the Mesh's combined? change to size of webcam?
    public const float maxViewDst = 450;

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
    }

    //I think we can get rid of this update method?
    void Update()
    {
        viewerPosition = new Vector2(viewer.position.x, viewer.position.z);
        UpdateVisibleChunks();
    }

    void UpdateVisibleChunks()
    {

        for (int i = 0; i < terrainChunksVisibleLastUpdate.Count; i++)
        {
            terrainChunksVisibleLastUpdate[i].SetVisible(false);
        }
        terrainChunksVisibleLastUpdate.Clear();

        //first get coordinate of the chunk that the viewer is standing on
        //need to change so that it's divided by webcam width and height?
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

                //want to create a terrain chunk at this coordinate if we haven't already so we create a dictionary of all the terrain chunks to prevent duplicates
                //not sure if this part is needed for us, maybe just a collection of terrain chunks? might not even need a collection?
                //probably don't need this if statement, just the else part at most
                if (terrainChunkDictionary.ContainsKey(viewedChunkCoord))
                {
                    terrainChunkDictionary[viewedChunkCoord].UpdateTerrainChunk();
                    if (terrainChunkDictionary[viewedChunkCoord].IsVisible())
                    {
                        terrainChunksVisibleLastUpdate.Add(terrainChunkDictionary[viewedChunkCoord]);
                    }
                }
                else
                {
                    terrainChunkDictionary.Add(viewedChunkCoord, new TerrainChunk(viewedChunkCoord, chunkSize, transform, mapMaterial));
                }

            }
        }
    }

    public class TerrainChunk
    {
        GameObject meshObject;
        Vector2 position;

        //don't need? used for enabling/disabling chunks
        Bounds bounds;

        MeshRenderer meshRenderer;
        MeshFilter meshFilter;

        //parent is used to make the new planes/chunks children of the viewer. we don't have a viewer so probably don't need this unless we make a game object to be the parent
        public TerrainChunk(Vector2 coord, int size, Transform parent, Material material)
        {
            position = coord * size;
            bounds = new Bounds(position, Vector2.one * size);
            Vector3 positionV3 = new Vector3(position.x, 0, position.y);

            meshObject = new GameObject("Terrain Chunk");
            meshRenderer = meshObject.AddComponent<MeshRenderer>();
            meshFilter = meshObject.AddComponent<MeshFilter>();
            meshRenderer.material = material;

            meshObject.transform.position = positionV3;
            meshObject.transform.parent = parent;
            SetVisible(false);

            MeshData meshData = mapGenerator.RequestMeshData();

            //This is wonky! Not updating mesh when using webcam but works with pure noise
            meshFilter.sharedMesh = meshData.CreateMesh();
        }
/*
        void OnMapDataReceived(MapData mapData)
        {
           // mapGenerator.RequestMeshData(mapData, OnMeshDataReceived);
        }

        void OnMeshDataReceived(MeshData meshData)
        {
            meshFilter.mesh = meshData.CreateMesh();
        }
        */

        public void UpdateTerrainChunk()
        {
            //enables and disables mesh objects depending on where the viewer is
            //I don't think we need this for our project
            float viewerDstFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));
            bool visible = viewerDstFromNearestEdge <= maxViewDst;
            //don't need?
            SetVisible(visible);
        }

        //don't need?
        public void SetVisible(bool visible)
        {
            meshObject.SetActive(visible);
        }

        //don't need?
        public bool IsVisible()
        {
            return meshObject.activeSelf;
        }

    }
}
