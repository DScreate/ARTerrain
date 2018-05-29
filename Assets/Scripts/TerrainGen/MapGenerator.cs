using UnityEngine;
using TerrainGenData;


/// <summary>
/// This class is used to generate various "map" objects that are needed for processing images.
/// Key features include the various processes used to validate mapData objects, the calls to the other
/// generators for combining maps together and then the Start method which begins the various initializers for
/// the different objects that combine to form a map. This map will then be passed to the MeshGenerator class to
/// be converted into a 3D mesh
/// </summary>
public class MapGenerator : MonoBehaviour
{
    //[Tooltip("Set the name of the device to use.")]

    /// <summary>
    /// The terrain data
    /// </summary>
    public DataForTerrain terrainData;

    /// <summary>
    /// The noise data
    /// </summary>
    public NoiseData noiseData;

    /// <summary>
    /// The texture data
    /// </summary>
    public TextureData textureData;

    /// <summary>
    /// The terrain material
    /// </summary>
    public Material terrainMaterial;

    /// <summary>
    /// The webcam controller
    /// </summary>
    public static WebcamTextureController webcamController;

    /// <summary>
    /// The face
    /// </summary>
    private static FaceDetection face;

    /// <summary>
    /// The water
    /// </summary>
    public GameObject Water;


    /// <summary>
    /// The minimum grey value used in assigning data to a given greyscale image
    /// This is possibly a legacy implementation but its use is to be assigned a float between
    /// 0 and 1 in which any given pixel in the greyscale image will be ignored if its calculated
    /// value for the height map is below that number. This allows for a "minimum" brightness level
    /// to be designated for generating a mesh with
    /// </summary>
    [Range(0, 1)]
    public float minGreyValue;
    /// <summary>
    /// The noise weight
    /// </summary>
    [Range(0, 1)]
    public float noiseWeight;

    /// <summary>
    /// The level of detail
    /// </summary>
    [Range(0, 6)]
    public int levelOfDetail;

    // need to create getters? need to make it so these values can't be changed once they're set in Start()    

    /// <summary>
    /// The map width
    /// </summary>
    private int mapWidth;

    /// <summary>
    /// The map height
    /// </summary>
    private int mapHeight;

    /// <summary>
    /// The map chunk width
    /// </summary>
    private int mapChunkWidth;

    /// <summary>
    /// The map chunk height
    /// </summary>
    private int mapChunkHeight;

    /// <summary>
    /// The number chunk width
    /// </summary>
    private int numChunkWidth;

    /// <summary>
    /// The number chunk height
    /// </summary>
    private int numChunkHeight;

    /// <summary>
    /// Gets the width of the map chunk.
    /// </summary>
    /// <value>
    /// The width of the map chunk.
    /// </value>
    public int MapChunkWidth
    {
        get
        {
            return this.mapChunkWidth;
        }
    }

    /// <summary>
    /// Gets the height of the map chunk.
    /// </summary>
    /// <value>
    /// The height of the map chunk.
    /// </value>
    public int MapChunkHeight
    {
        get
        {
            return this.mapChunkHeight;
        }
    }

    /// <summary>
    /// Gets the width of the number chunk.
    /// </summary>
    /// <value>
    /// The width of the number chunk.
    /// </value>
    public int NumChunkWidth
    {
        get
        {
            return this.numChunkWidth;
        }
    }

    /// <summary>
    /// Gets the height of the number chunk.
    /// </summary>
    /// <value>
    /// The height of the number chunk.
    /// </value>
    public int NumChunkHeight
    {
        get
        {
            return this.numChunkHeight;
        }
    }

    /// <summary>
    /// The chunk noise map
    /// </summary>
    private float[,] chunkNoiseMap;
    /// <summary>
    /// The full noise map
    /// </summary>
    private float[,] fullNoiseMap;
    /// <summary>
    /// The height map
    /// </summary>
    private float[,] heightMap;


    /// <summary>
    /// The start method used by Unity.
    /// This mostly contains calls to the various initializers for the different components used together to c
    /// </summary>
    private void Start()
    {       
        webcamController = this.gameObject.GetComponent(typeof(WebcamTextureController)) as WebcamTextureController;

        face = this.gameObject.GetComponent(typeof(FaceDetection)) as FaceDetection;

        webcamController.Initialize();

        this.InitializeMapSizes();

        this.InitializeChunkSizes();

        this.InitializeNumOfChunks();

        this.fullNoiseMap = NoiseGenerator.GenerateNoiseMap(this.mapWidth, this.mapHeight, this.noiseData.seed, this.noiseData.noiseScale, this.noiseData.octaves, this.noiseData.persistance, this.noiseData.lacunarity, this.noiseData.offset, this.noiseData.normalizeMode);

        this.textureData.ApplyToMaterial(this.terrainMaterial);        
    }

    /// <summary>
    /// Initializes the map sizes.
    /// </summary>
    private void InitializeMapSizes()
    {
        this.mapWidth = webcamController.WebcamWidth;
        this.mapHeight = webcamController.WebcamHeight;
    }

    /// <summary>
    /// Initializes the map chunk sizes. The while loop is used to determine divisibility by 2. This was
    /// done to avoid bugs in which non-standard camera sizes would cause improper mesh generation
    /// </summary>
    private void InitializeChunkSizes()
    {
        this.mapChunkWidth = this.mapWidth;
        this.mapChunkHeight = this.mapHeight;

        while (this.mapChunkWidth > 250 || this.mapChunkHeight > 250)
        {
            if (this.mapChunkWidth % 2 != 0)
                Debug.Log("Width " + this.mapChunkWidth + " is not evenly divisble by 2");

            this.mapChunkWidth /= 2;

            if (this.mapChunkHeight % 2 != 0)
                Debug.Log("Height " + this.mapChunkHeight + " is not evenly divisble by 2");

            this.mapChunkHeight /= 2;
        }

        //I forget why, but Sebastion explains in a video that these variables need to be chunk size + 1
        this.mapChunkWidth++;
        this.mapChunkHeight++;        
    }

    /// <summary>
    /// Initializes the number of chunks.
    /// </summary>
    private void InitializeNumOfChunks()
    {
        this.numChunkWidth = this.mapWidth / (this.mapChunkWidth - 1);
        this.numChunkHeight = this.mapHeight / (this.mapChunkHeight - 1);
    }

    /// <summary>
    /// Requests the mesh data. Note that this sets the local heightMap object to a generated float[,] based on the current chunkPosition.
    /// </summary>
    /// <param name="chunkPosition">The chunk position.</param>
    /// <returns>MeshData object created via a combination of the heightmap, the meshHeightMultiplier, sample points from the meshHeightCurve and a specified level of detail</returns>
    public MeshData RequestMeshData(Vector2 chunkPosition)
    {
        this.heightMap = this.GenerateMapData(chunkPosition);

        return MeshGenerator.GenerateTerrainMesh(this.heightMap, this.terrainData.meshHeightMultiplier, this.terrainData.meshHeightCurve, this.levelOfDetail);
    }

    /// <summary>
    /// Generates the map data.
    /// </summary>
    /// <param name="chunkPosition">The chunk position.</param>
    /// <returns></returns>
    private float[,] GenerateMapData(Vector2 chunkPosition)
    {
        if (Application.isPlaying)
        {
            this.chunkNoiseMap = NoiseGenerator.LerpNoiseMapWithTextureToNoiseChunk(face.FaceTexture, this.fullNoiseMap, this.noiseWeight, this.minGreyValue, this.mapChunkWidth, this.mapChunkHeight, chunkPosition);
        }

        else
        {
            this.chunkNoiseMap = NoiseGenerator.GenerateNoiseMap(1, 1, 1, 1, 1, 1, 1, new Vector2(1, 1), this.noiseData.normalizeMode);
            Debug.Log("Generate map data error");
        }

        this.textureData.UpdateMeshHeights(this.terrainMaterial, this.terrainData.minHeight, this.terrainData.maxHeight);

        return this.chunkNoiseMap;
    }

    /// <summary>
    /// Called when [values update].
    /// </summary>
    private void OnValuesUpdate()
    {
        this.UpdateWaterHeight();
    }
    /// <summary>
    /// Called when [texture values updated].
    /// </summary>
    public void OnTextureValuesUpdated()
    {
        this.textureData.ApplyToMaterial(this.terrainMaterial);

        this.UpdateWaterHeight();
    }
    /// <summary>
    /// Updates the height of the water.
    /// </summary>
    public void UpdateWaterHeight()
    {
        if (this.Water == null) this.Water = GameObject.Find("WaterProDaytime");

        var newHeight = this.textureData.layers[1].startHeight * this.terrainData.meshHeightMultiplier * this.terrainData.uniformScale;
        var curPos = this.Water.transform.position;
        this.Water.transform.position = new Vector3(curPos.x, newHeight, curPos.z);
    }

    /// <summary>
    /// Updates the full noise map.
    /// </summary>
    public void UpdateFullNoiseMap()
    {
        if (this.noiseData.Updated)
        {
            this.fullNoiseMap = NoiseGenerator.GenerateNoiseMap(webcamController.WebcamTex.width, webcamController.WebcamTex.height, this.noiseData.seed, this.noiseData.noiseScale, this.noiseData.octaves, this.noiseData.persistance, this.noiseData.lacunarity, this.noiseData.offset, this.noiseData.normalizeMode);
            this.noiseData.Updated = false;
        }
    }

    /// <summary>
    /// Called when [validate].
    /// </summary>
    private void OnValidate()
    {
        if (this.terrainData != null)
        {
            this.terrainData.OnValuesUpdated -= this.OnValuesUpdate;
            this.terrainData.OnValuesUpdated += this.OnValuesUpdate;
        }
        if (this.noiseData != null)
        {
            this.noiseData.OnValuesUpdated -= this.OnValuesUpdate;
            this.noiseData.OnValuesUpdated += this.OnValuesUpdate;
        }
        if (this.textureData != null)
        {
            this.textureData.OnValuesUpdated -= this.OnTextureValuesUpdated;
            this.textureData.OnValuesUpdated += this.OnTextureValuesUpdated;
        }
    }
}