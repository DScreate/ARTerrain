using System.Collections;
using System.Collections.Generic;
using TerrainGen;
using UnityEditor;
using UnityEngine;

[CustomEditor (typeof (TerrainController))]
public class TerrainControllerEditor : Editor {

	public override void OnInspectorGUI()
	{
		TerrainController tc = (TerrainController)target;

		if( DrawDefaultInspector())
		{
			if (tc.autoUpdate)
			{
				tc.GenerateTerrain();
			}
		}

		if(GUILayout.Button("Generate"))
		{
			tc.GenerateTerrain();
		}
	}
}
