using System;
using System.Collections;
using System.Collections.Generic;
using CameraMovement;
using TerrainGenData;
using UnityEngine;
/// <summary>
/// Class <c>MenuConrtoller</c> provides structured layout for a user friendly interface. 
/// </summary>
public class MenuController : MonoBehaviour {

    WebcamTextureController webcamController;
    public DataForTerrain terrainData;
    public NoiseData noiseData;
    public TextureData textureData;
    public MapGenerator mapGen;
    public CameraMover cameraMover;
    
    /// <summary>
    /// Method <c>Start</c> initializes webcamController object. 
    /// <see cref="webcamController"/>.
    /// </summary>
	void Update () {
        if(webcamController == null)
            webcamController = MapGenerator.webcamController;
	}

    /// <summary>
    /// Method <c>OnPlayButtonClick</c> calls Play() when play button is clicked. 
    /// </summary>
    #region Webcam
    public void OnPlayButtonClick()
    {
        webcamController.WebcamTex.Play();
    }

    /// <summary>
    /// Method <c>OnPauseButtonClick</c> calls <see cref="Pause()"/> when pause button is clicked. 
    /// </summary>
    public void OnPauseButtonClick()
    {
        webcamController.WebcamTex.Pause();
    }

    /// <summary>
    /// Method <c>OnChangeCameraButtonClick</c> calls <see cref="ChangeWebcamTextureToNextAvailable()"/> when change camera button is clicked. 
    /// </summary>
    public void OnChangeCameraButtonClick()
    {
        if(webcamController != null)
            webcamController.ChangeWebcamTextureToNextAvailable();
        else
            Debug.Log("null webcamController");
    }
    #endregion
    
    /// <summary>
    /// Method <c>SetUniformScale</c> sets the uniform scale for terrain, and updates height. 
    /// </summary>
    /// <param name="vin">new uniform scale value of type float</param>
    #region TerrainData
    public void SetUniformScale(float vin)
    {
        terrainData.uniformScale = vin;
        mapGen.UpdateWaterHeight();

    }

    /// <summary>
    /// Method <c>SetHeightMultiplier</c> takes a float and sets the height mutliplier for terrain, then updates height. 
    /// </summary>
    /// <param name="vin">new height multiplier value of type float</param>
    public void SetHeightMultiplier(float vin)
    {
        terrainData.meshHeightMultiplier = vin;
        mapGen.UpdateWaterHeight();
    }
    #endregion

    #region Noise
    /// <summary>
    /// Method <c>SetNoiseBlending</c> sets noise blending value. 
    /// </summary>
    /// <param name="vin"></param>
    public void SetNoiseBlending(float vin)
    {
        //terrainData.
    }

    //refactor and so that these variables are properties and they will change Updated to true inside setter?
    //But then how to access from inspector if they're properties?
    /// <summary>
    /// Method <c>SetNoiseScale</c> takes a value of type float and sets it as new noise scale.
    /// <see cref="noiseData"/> 
    /// </summary>
    /// <param name="vin">new noise scale value of type float</param>
    public void SetNoiseScale(float vin)
    {
        noiseData.noiseScale = vin;
        noiseData.Updated = true;
    }

    /// <summary>
    /// Method <c>SetNoiseOctaves</c> takes a value of type float and sets it as the new octave value rounded to an int. 
    /// <see cref="noiseData"/> 
    /// </summary>
    /// <param name="vin">new octave value of type float</param>
    public void SetNoiseOctaves(float vin)
    {
        noiseData.octaves = Mathf.RoundToInt(vin);
        noiseData.Updated = true;
    }
    
    /// <summary>
    /// Method <c>SetNoisePersistance</c> takes a value of type float and sets it as the new persistance value. 
    /// <see cref="noiseData"/> 
    /// </summary>
    /// <param name="vin">new persistance value of type float</param>
    public void SetNoisePersistance(float vin)
    {
        noiseData.persistance = vin;
        noiseData.Updated = true;
    }
    
    /// <summary>
    /// Method <c>SetNoiseLacunarity</c> takes a value of type float and sets it as the new lacunarity value. 
    /// <see cref="noiseData"/> 
    /// </summary>
    /// <param name="vin">new lacunarity value of type float</param>
    public void SetNoiseLacunarity(float vin)
    {
        noiseData.lacunarity = vin;
        noiseData.Updated = true;
    }
    #endregion
    
    #region TextureData
    /// <summary>
    /// Method <c>SetTextureTint</c> takes a value of type float and sets it as the new texture tint value. 
    /// </summary>
    /// <param name="color"></param>
    /// <param name="index"></param>
    public void SetTextureTint(Color color, int index)
    {
        
    }

    /// <summary>
    /// Method <c>StringToTexture0</c> takes a value of type String and sets it as texture 0.
    /// </summary>
    /// <param name="text">String value to be parsed into texture</param>
    public void StringToTexture0(String text)
    {
        float temp;
        float.TryParse(text, out temp);
        SetTextureHeight0(Mathf.Clamp01(temp));
    }
    
    /// <summary>
    /// Method <c>StringToTexture1</c> takes a value of type String and sets it as texture 1.
    /// </summary>
    /// <param name="text">String value to be parsed into texture</param>
    public void StringToTexture1(String text)
    {
        float temp;
        float.TryParse(text, out temp);
        SetTextureHeight1(temp);
    }
    
    /// <summary>
    /// Method <c>StringToTexture2</c> takes a value of type String and sets it as texture 2.
    /// </summary>
    /// <param name="text">String value to be parsed into texture</param>
    public void StringToTexture2(String text)
    {
        float temp;
        float.TryParse(text, out temp);
        SetTextureHeight2(temp);
    }
    
    /// <summary>
    /// Method <c>StringToTexture3</c> takes a value of type String and sets it as texture 3.
    /// </summary>
    /// <param name="text">String value to be parsed into texture</param>
    public void StringToTexture3(String text)
    {
        float temp;
        float.TryParse(text, out temp);
        SetTextureHeight3(temp);
    }
    
    /// <summary>
    /// Method <c>StringToTexture4</c> takes a value of type String and sets it as texture 4.
    /// </summary>
    /// <param name="text">String value to be parsed into texture</param>
    public void StringToTexture4(String text)
    {
        float temp;
        float.TryParse(text, out temp);
        SetTextureHeight4(temp);
    }
    
    /// <summary>
    /// Method <c>StringToTexture5</c> takes a value of type String and sets it as texture 5.
    /// </summary>
    /// <param name="text">String value to be parsed into texture</param>
    public void StringToTexture5(String text)
    {
        float temp;
        float.TryParse(text, out temp);
        SetTextureHeight5(temp);
    }
    
    /// <summary>
    /// Method <c>SetTextureHeight0</c> takes a value of type float and sets it as a height for texture 0. 
    /// </summary>
    /// <param name="value"> new textuure height value of type float </param>
    public void SetTextureHeight0(float value)
    {
        textureData.layers[0].startHeight = value;
        mapGen.OnTextureValuesUpdated();
    }
    
    /// <summary>
    /// Method <c>SetTextureHeight0</c> takes a value of type float and sets it as a height for texture 1.
    /// </summary>
    /// <param name="value"> new textuure height value of type float </param>
    public void SetTextureHeight1(float value)
    {
        textureData.layers[1].startHeight = value;
        mapGen.OnTextureValuesUpdated();
    }
    
    /// <summary>
    /// Method <c>SetTextureHeight0</c> takes a value of type float and sets it as a height for texture 2 
    /// </summary>
    /// <param name="value"> new textuure height value of type float </param>
    public void SetTextureHeight2(float value)
    {
        textureData.layers[2].startHeight = value;
        mapGen.OnTextureValuesUpdated();
    }
    
    /// <summary>
    /// Method <c>SetTextureHeight0</c> takes a value of type float and sets it as a height for texture 3. 
    /// </summary>
    /// <param name="value"> new textuure height value of type float </param>
    public void SetTextureHeight3(float value)
    {
        textureData.layers[3].startHeight = value;
        mapGen.OnTextureValuesUpdated();
    }

    /// <summary>
    /// Method <c>SetTextureHeight0</c> takes a value of type float and sets it as a height for texture 4. 
    /// </summary>
    /// <param name="value"> new textuure height value of type float </param>
    public void SetTextureHeight4(float value)
    {
        textureData.layers[4].startHeight = value;
        mapGen.OnTextureValuesUpdated();
    }

    /// <summary>
    /// Method <c>SetTextureHeight0</c> takes a value of type float and sets it as a height for texture 5. 
    /// </summary>
    /// <param name="value"> new textuure height value of type float </param>
    public void SetTextureHeight5(float value)
    {
        textureData.layers[5].startHeight = value;
        mapGen.OnTextureValuesUpdated();
    }
    
    
    #endregion
    
    #region Camera
    /// <summary>
    /// Method <c>SetCameraSpeed</c> takes a value of type float and sets it as the new camera speed. 
    /// </summary>
    public void SetCameraSpeed(float vin)
    {
        cameraMover.speed = (int) vin;
    }
    #endregion

}
