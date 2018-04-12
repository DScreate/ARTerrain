using System;
using System.Collections;
using System.Collections.Generic;
using TerrainGenData;
using UnityEngine;

public class MenuController : MonoBehaviour {

    WebcamTextureController webcamController;
    public DataForTerrain terrainData;
    public NoiseData noiseData;
    public TextureData textureData;
    public MapGenerator mapGen;

	void Start () {
        webcamController = MapGenerator.webcamController;
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
    
    #region TerrainData
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
    
    #region TextureData

    public void SetTextureTint(Color color, int index)
    {
        
    }

    public void StringToTexture0(String text)
    {
        float temp;
        float.TryParse(text, out temp);
        
        SetTextureHeight0(Mathf.Clamp01(temp));
    }
    
    public void StringToTexture1(String text)
    {
        float temp;
        float.TryParse(text, out temp);
        SetTextureHeight1(temp);
    }
    
    public void StringToTexture2(String text)
    {
        float temp;
        float.TryParse(text, out temp);
        SetTextureHeight2(temp);
    }
    
    public void StringToTexture3(String text)
    {
        float temp;
        float.TryParse(text, out temp);
        SetTextureHeight3(temp);
    }
    
    public void StringToTexture4(String text)
    {
        float temp;
        float.TryParse(text, out temp);
        SetTextureHeight4(temp);
    }
    
    public void StringToTexture5(String text)
    {
        float temp;
        float.TryParse(text, out temp);
        SetTextureHeight5(temp);
    }
    
    public void SetTextureHeight0(float value)
    {
        textureData.layers[0].startHeight = value;
        mapGen.OnTextureValuesUpdated();
    }
    
    public void SetTextureHeight1(float value)
    {
        textureData.layers[1].startHeight = value;
        mapGen.OnTextureValuesUpdated();
    }
    
    public void SetTextureHeight2(float value)
    {
        textureData.layers[2].startHeight = value;
        mapGen.OnTextureValuesUpdated();
    }
    
    public void SetTextureHeight3(float value)
    {
        textureData.layers[3].startHeight = value;
        mapGen.OnTextureValuesUpdated();
    }
    public void SetTextureHeight4(float value)
    {
        textureData.layers[4].startHeight = value;
        mapGen.OnTextureValuesUpdated();
    }

    public void SetTextureHeight5(float value)
    {
        textureData.layers[5].startHeight = value;
        mapGen.OnTextureValuesUpdated();
    }
    
    
    #endregion

}
