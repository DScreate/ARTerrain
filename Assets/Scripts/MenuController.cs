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

    public void SetNoiseScale(float vin)
    {
        noiseData.noiseScale = vin;
    }
    
    public void SetNoiseOctaves(float vin)
    {
        noiseData.octaves = Mathf.RoundToInt(vin);
    }
    
    public void SetNoisePersistance(float vin)
    {
        noiseData.persistance = vin;
    }
    
    public void SetNoiseLacunarity(float vin)
    {
        noiseData.lacunarity = vin;
    }
    #endregion

}
