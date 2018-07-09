using UnityEngine;

/// <summary>
/// MeshGenerator is the class responsible for handling the calculations required to create the 3D object from a calculated heightmap.
/// The key method is GenerateTerrainMesh in which a heightMap representing a greyscale image converted into a float[,] is iterated through
/// and sampled at regular intervals in order to find a value for which to create a point in 3D space. These points are then combined into triangles
/// within a MeshData object and these MeshData objects are then combined to form the overall mesh
/// </summary>
public static class MeshGenerator {

    /// <summary>
    /// Generates the terrain mesh. The key process of this method is the iteration through the heightMap and the regular sampling that takes place.
    /// The heightMap is iterated for every combination point x,y at an interval equal to the meshSimplificationIncremement. This means that as the level
    /// of detail decreases, the granularity and resolution of the sampling will decrease and the generated mesh will contain less detail/vertices.
    /// </summary>
    /// <param name="heightMap">The height map value.</param>
    /// <param name="heightMultiplier">The height multiplier value.</param>
    /// <param name="heightCurve">The height curve value.</param>
    /// <param name="levelOfDetail">The level of detail value.</param>
    /// <returns></returns>
    public static MeshData GenerateTerrainMesh(float[,] heightMap, float heightMultiplier, AnimationCurve heightCurve, int levelOfDetail)
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);
        float topLeftX = (width - 1) / 2f;
        float topLeftZ = (height - 1) / 2f;

        int meshSimplificationIncrement = (levelOfDetail == 0) ? 1: levelOfDetail * 2;
        int verticesPerLine = (width - 1) / meshSimplificationIncrement + 1;

        MeshData meshData = new MeshData(width, height);
        int vertexIndex = 0;

        for (int y = 0; y < height; y += meshSimplificationIncrement)
        {
            for (int x = 0; x < width; x += meshSimplificationIncrement)
            {
                meshData.vertices[vertexIndex] = new Vector3(topLeftX - x, heightCurve.Evaluate(heightMap[x, y]) * heightMultiplier, topLeftZ - y);
                meshData.uvs[vertexIndex] = new Vector2(x / (float)width, y / (float)height);

                if(x < width - 1 && y < height - 1)
                {
                    meshData.AddTriangle(vertexIndex, vertexIndex + verticesPerLine + 1, vertexIndex + verticesPerLine);
                    meshData.AddTriangle(vertexIndex + verticesPerLine + 1, vertexIndex, vertexIndex + 1);
                }

                vertexIndex++;
            }
        }

        return meshData;
    }
}

/// <summary>
/// MeshData is the means by which individual meshes are constructed and then stored.
/// Each MeshData class contains three key data arrays: vertices, triangles and uvs.
/// Vertices is a collection of Vector3 objects that represent points in 3D space within the Unity engine
/// Triangles is a collection of "triangles" that are formed via the connection of the points stored within the vertices array
/// with points closest to one another connecting to form triangles. Note, the overall mesh is contiguous but it is built via the
/// connection and "sewing" together of triangles
/// UVs is the uv data for each triangle used to map textures onto the mesh through the triangles stored within the MeshData
/// </summary>
public class MeshData
{
    /// <summary>
    /// The vertices
    /// </summary>
    public Vector3[] vertices;

    /// <summary>
    /// The triangles
    /// </summary>
    public int[] triangles;

    /// <summary>
    /// The uvs
    /// </summary>
    public Vector2[] uvs;

    /// <summary>
    /// The triangle index
    /// </summary>
    int triangleIndex;

    /// <summary>
    /// Initializes a new instance of the <see cref="MeshData"/> class.
    /// </summary>
    /// <param name="meshWidth">Width of the mesh.</param>
    /// <param name="meshHeight">Height of the mesh.</param>
    public MeshData(int meshWidth, int meshHeight)
    {
        vertices = new Vector3[meshWidth * meshHeight];
        uvs = new Vector2[meshWidth * meshHeight];
        triangles = new int[(meshWidth - 1) * (meshHeight - 1) * 6];
    }

    /// <summary>
    /// Adds the triangle.
    /// </summary>
    /// <param name="a">side a.</param>
    /// <param name="b">side  b.</param>
    /// <param name="c">side c.</param>
    public void AddTriangle(int a, int b, int c)
    {
        triangles[triangleIndex] = c;
        triangles[triangleIndex+1] = b;
        triangles[triangleIndex+2] = a;

        triangleIndex += 3;
    }

    //Anyway to alter existing mesh rather than creating a new one?

    /// <summary>
    /// Creates the mesh.
    /// </summary>
    /// <returns></returns>
    public Mesh CreateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
        return mesh;
    }

    /// <summary>
    /// Updates the mesh.
    /// </summary>
    /// <param name="mesh">The mesh to be updated.</param>
    public void UpdateMesh(Mesh mesh)
    {
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
    }
}
