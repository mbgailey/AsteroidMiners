using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TerrainRegistry : MonoBehaviour {
	
	public GameObject[] terrainList;
	[HideInInspector] public Vector3[] gravCentersList;
	[HideInInspector] public float[] gravMassList;
	
	// Use this for initialization
	void Start () {
		//terrainList = new List<GameObject>(); 
		terrainList = GameObject.FindGameObjectsWithTag("Terrain");
		gravCentersList = new Vector3[10];
		gravMassList = new float[10];
		int i = 0;
		foreach(GameObject obj in terrainList) {
			
			gravCentersList[i] = obj.transform.position;
			gravMassList[i] = obj.GetComponent<TerrainProperties>().terrainMass;
			i++;
		}
	}
	
	// Add item to the registry
	void AddToRegistry (GameObject terr) {
		//terrainList.Add(terr); Add doesn't work for GameObject[] type lists
		//gravCentersList.Add(terr.transform.position);
		//gravMassList.Add(terr.GetComponent<TerrainProperties>().terrainMass);
	}

	public GameObject GetRandomTerrain () {
		int ind = Random.Range ((int)0, terrainList.Length);
		return terrainList [ind];
	}
	
	
}
