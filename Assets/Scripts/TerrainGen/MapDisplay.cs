using UnityEngine;

public class MapDisplay : MonoBehaviour {

    public Renderer textureRenderer;
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;
    public Renderer webcamRenderer;

    public void DrawTexture(Texture2D texture)
    { 
        textureRenderer.sharedMaterial.mainTexture = texture;
        textureRenderer.transform.localScale = new Vector3(texture.width, 1, texture.height);
    }

    public void DrawTexture(Texture2D texture, WebCamTexture webCam)
    {
        textureRenderer.sharedMaterial.mainTexture = texture;
        textureRenderer.transform.localScale = new Vector3(texture.width, 1, texture.height);
        webcamRenderer.sharedMaterial.mainTexture = webCam;
        webcamRenderer.transform.localScale = new Vector3(webCam.width, 1, webCam.height);
    }

    public void DrawMesh(MeshData meshData, Texture2D texture)
    {
        meshFilter.sharedMesh = meshData.CreateMesh();
        meshRenderer.sharedMaterial.mainTexture = texture;
    }
    
    public void DrawMesh(MeshData meshData, Texture2D texture, WebCamTexture webCam)
    {
        meshFilter.sharedMesh = meshData.CreateMesh();
        meshRenderer.sharedMaterial.mainTexture = texture;
        textureRenderer.sharedMaterial.mainTexture = webCam;
    }
    public void DrawMesh(MeshData meshData, Texture2D texture, Texture2D plane)
    {
        meshFilter.sharedMesh = meshData.CreateMesh();
        meshRenderer.sharedMaterial.mainTexture = texture;
        textureRenderer.sharedMaterial.mainTexture = plane;
    }
}
