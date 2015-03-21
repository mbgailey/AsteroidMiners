using UnityEngine;
using UnityEditor;

// Comment this to disable script
[InitializeOnLoad]
public class UnityRemoteFix
{
	private static bool lastPlaying = false;
	static UnityRemoteFix()
	{
		EditorApplication.playmodeStateChanged += OnPlayModeChanged;
	}

	static void OnPlayModeChanged()
	{
		if (EditorApplication.isPlaying)
		{
			lastPlaying = true;
		}

		if (EditorApplication.isPlaying == false && lastPlaying == true)
		{
			lastPlaying = false;
			EditorApplication.ExecuteMenuItem("Edit/Project Settings/Editor");
			//Debug.Log(Application.dataPath + "/Scripts/Editor/UnityRemoteFix.ahk");
			System.Diagnostics.Process.Start(Application.dataPath + "/Scripts/Editor/UnityRemoteFix.ahk");
		}

	}
}
