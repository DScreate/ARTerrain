using System.Collections;
using System.Collections.Generic;
using TerrainGenData;
using UnityEngine;

public class MenuController : MonoBehaviour {

    WebcamTextureController webcamController;
    public DataForTerrain terrainData;
    public NoiseData noiseData;

	void Start () {
        webcamController = MapGenerator.webcamController;
	    //terrainData = 
	}

    #region Webcam
    public void OnPlayButtonClick()
    {
        webcamController.WebcamTex.Play();
    }

    public void OnPauseButtonClick()
    {
        webcamController.WebcamTex.Pause();
    }

    public void OnChangeCameraButtonClick()
    {
        webcamController.ChangeWebcamTextureToNextAvailable();
    }
    #endregion
    
    #region Texture
    public void SetUniformScale(float vin)
    {
        terrainData.uniformScale = vin;
    }

    public void SetHeightMultiplier(float vin)
    {
        terrainData.meshHeightMultiplier = vin;
    }
    #endregion

    #region Noise
    public void SetNoiseBlending(float vin)
    {
        //terrainData.
    }

    //refactor and so that these variables are properties and they will change Updated to true inside setter?
    //But then how to access from inspector if they're properties?
    public void SetNoiseScale(float vin)
    {
        noiseData.noiseScale = vin;
        noiseData.Updated = true;
    }
    
    public void SetNoiseOctaves(float vin)
    {
        noiseData.octaves = Mathf.RoundToInt(vin);
        noiseData.Updated = true;
    }
    
    public void SetNoisePersistance(float vin)
    {
        noiseData.persistance = vin;
        noiseData.Updated = true;
    }
    
    public void SetNoiseLacunarity(float vin)
    {
        noiseData.lacunarity = vin;
        noiseData.Updated = true;
    }
    #endregion

}
