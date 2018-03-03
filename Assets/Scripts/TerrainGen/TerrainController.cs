using System;
using System.Collections;
using System.Collections.Generic;
using ColorTracking;
using UnityEngine;

public class TerrainController : MonoBehaviour
{


	public enum ImageMode
	{
		FromNoise,
		FromWebcam
	};

	public ImageMode imageMode;

	public int mapWidth;
	public int mapHeight;

	public float noiseScale;

	public int octaves;
	[Range(0, 1)] public float persistance;
	public float lacunarity;

	public int seed;
	public Vector2 offset;

	public float meshHeightMultiplier;
	public AnimationCurve meshHeightCurve;

	public bool autoUpdate;


	public Terrain _terrain;
	public int HeightMapResolution;
	
	WebCamTexture _webcamtex;
	Texture2D _TextureFromCamera;

	public string requestedDeviceName = null;
	
	private void Start()
	{
		
		if (imageMode == ImageMode.FromWebcam)
		{
			//_TextureFromCamera = new Texture2D(mapWidth, mapHeight);
	
			if (!String.IsNullOrEmpty(requestedDeviceName))
			{
				_webcamtex = new WebCamTexture(requestedDeviceName, mapWidth, mapHeight);
			}
	
			else
				_webcamtex = new WebCamTexture(mapWidth, mapHeight);
	
			_webcamtex.Play();        
		}   
	
	}


	// Update is called once per frame
	public void GenerateTerrain()
	{
		if (imageMode == ImageMode.FromNoise)
		{
			float[,] noiseMap = Noise.GenerateNoiseMap(mapWidth, mapHeight, seed, noiseScale, octaves, persistance, lacunarity,
				new Vector2(0, 0) + offset);

			TerrainData terrainData = new TerrainData();
		
		
			terrainData.heightmapResolution = HeightMapResolution;
			//terrainData.baseMapResolution = 1024;
			//terrainData.SetDetailResolution(1024,terrainData.detailResolution);

			terrainData.size = new Vector3(mapWidth,meshHeightMultiplier, mapHeight);
			terrainData.SetHeights(0, 0, noiseMap);

		

			_terrain.terrainData = terrainData;
			_terrain.GetComponent<TerrainCollider>().terrainData = _terrain.terrainData;
		} else if (imageMode == ImageMode.FromWebcam)
		{
			/*
			float[,] noiseMap = Noise.GenerateNoiseMap(mapWidth, mapHeight, seed, noiseScale, octaves, persistance, lacunarity,
				new Vector2(0, 0) + offset);
			*/
			float[,] heightMap = TextureGenerator.ConvertTextureToFloats(_webcamtex);

			TerrainData terrainData = new TerrainData();
		
		
			terrainData.heightmapResolution = HeightMapResolution;
			//terrainData.baseMapResolution = 1024;
			//terrainData.SetDetailResolution(1024,terrainData.detailResolution);

			terrainData.size = new Vector3(mapWidth,meshHeightMultiplier, mapHeight);
			terrainData.SetHeights(0, 0, heightMap);

		

			_terrain.terrainData = terrainData;
			_terrain.GetComponent<TerrainCollider>().terrainData = _terrain.terrainData;
		}


	}

	void Update()
	{
		if (imageMode == ImageMode.FromWebcam)
		{
			/*
			for (int y = 0; y < mapHeight; y++)
			{
				for (int x = 0; x < mapWidth; x++)
				{
					_TextureFromCamera.SetPixel(x, y, _webcamtex.GetPixel(x, y));
				}
			}
			_TextureFromCamera.Apply();
			*/
			GenerateTerrain();
		}
	}

}
