using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour {

    WebcamTextureController webcamController;

	void Start () {
        webcamController = MapGenerator.webcamController;
    }

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

}
