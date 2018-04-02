using System.Collections;
using System.Collections.Generic;
using TerrainGenData;
using UnityEngine;

public class MenuController : MonoBehaviour {

    WebcamTextureController webcamController;
    public DataForTerrain terrainData;

	void Start () {
        webcamController = MapGenerator.webcamController;
	    //terrainData = 
	}

    public void OnPlayButtonClick()
    {
        webcamController.Webcamtex.Play();
    }

    public void OnPauseButtonClick()
    {
        webcamController.Webcamtex.Pause();
    }

    public void OnChangeCameraButtonClick()
    {
        webcamController.ChangeWebcamTextureToNextAvailable();
    }

    public void SetUniformScale(float vin)
    {
        terrainData.uniformScale = vin;
    }

}
