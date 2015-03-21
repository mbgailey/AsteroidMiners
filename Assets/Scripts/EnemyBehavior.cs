using UnityEngine;
using System.Collections;

public class EnemyBehavior : MonoBehaviour {

	TerrainRegistry terrainRegistry;

	[System.NonSerialized]
	public Vector2 myTerrainCenter;
	[System.NonSerialized]
	public GameObject myTerrain;

	Vector2 terrainDirection;	

	
	// Use this for initialization
	void Awake () {
		terrainRegistry = GameObject.Find("GameManager").GetComponent<TerrainRegistry>();
		myTerrain = terrainRegistry.GetRandomTerrain ();	//Select a random terrain to target

	}


}
