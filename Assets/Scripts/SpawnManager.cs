using UnityEngine;
using System.Collections;

public class SpawnManager : MonoBehaviour {

	public GameObject playerPrefab;

	int playerCount;

	float spawnHeight;		//Height above surface to end the spawn in travel

	Vector2 terrainCenter;
	LayerMask terrainMask;

	public Texture2D StampTex;

	TerrainRegistry terrainRegistry;
	PlayerRegistry playerRegistry;
	PersistentSettings persistentSettings;

	bool asteroidLevel;

	////CameraZoom camZoomControl;

	// Use this for initialization
	void Awake () {
		spawnHeight = 1.5f;
		terrainMask = 10;
		int terrainLayer = 10;
		terrainMask = 1 << terrainLayer;
		playerCount = 0;

		////camZoomControl = Camera.main.GetComponent<CameraZoom> ();
		terrainRegistry = GameObject.Find("GameManager").GetComponent<TerrainRegistry>();
		playerRegistry = GameObject.Find("GameManager").GetComponent<PlayerRegistry>();
		persistentSettings = GameObject.Find("GameSettings").GetComponent<PersistentSettings>();

		if (persistentSettings.levelType == PersistentSettings.LevelType.Asteriod) {
			asteroidLevel = true;
		}
		else {
			asteroidLevel = false;
		}

	}
	

	IEnumerator SpawnPlayer () {


		//Select random terrain object
		GameObject terrObj = terrainRegistry.GetRandomTerrain ();

		terrainCenter = terrObj.transform.position;
		Vector2 terrExtents = FindTerrainExtents (terrObj);	//Get max extents of terrain colliders from center

		Vector3 spawnPoint = new Vector3();
		float spawnTheta;

		if (asteroidLevel) {
			//If spawning on asteroid terrain
			float spawnRadius = terrExtents.magnitude + spawnHeight;	//Spawn at a height a little above max terrain extent
			spawnTheta = Random.Range(0, 360);				            //Choose a random angle
			spawnTheta = Mathf.Deg2Rad * spawnTheta; 				    //Convert angle to radians

			Vector3 testPoint = (Vector3)terrainCenter + new Vector3(Mathf.Cos(spawnTheta) * spawnRadius, Mathf.Sin(spawnTheta) * spawnRadius) ;	//Choose a point at theta outside of terrain

			RaycastHit2D line = Physics2D.Linecast(testPoint,terrainCenter,terrainMask); 	//Cast line towards center of asteroid terrain

			if (line) {
				float spawnPtX = line.point.x + spawnHeight * Mathf.Cos(spawnTheta);			//End travel at a point above the detected surface
				float spawnPtY = line.point.y + spawnHeight * Mathf.Sin(spawnTheta);
				spawnPoint = new Vector3(spawnPtX, spawnPtY, 0.0f);
			}

		}

		else {
			//If spawning on flat terrain
			spawnTheta = 0.0f;
			float xCoord = Random.Range (0f, 400f); //Choose random x point

			Vector3 testPoint = new Vector3(xCoord, 500f, 0f); //Select test point very high above the terrain

			RaycastHit2D line = Physics2D.Linecast(testPoint, new Vector3(xCoord,-500f,0f),terrainMask); 	//Cast line down

			Debug.Log (line.point);

			if (line) {
				float spawnPtX = xCoord;			//Select spawn point at a point above the detected surface
				float spawnPtY = line.point.y + spawnHeight;
				spawnPoint = new Vector3(spawnPtX, spawnPtY, 0.0f);
			}

		}
		GameObject playerObj = (GameObject) GameObject.Instantiate (playerPrefab, spawnPoint, Quaternion.identity);
		playerRegistry.AddToRegistry(playerObj);		//Add player to registry
		playerObj.GetComponent<PlayerSpriteManager> ().ChangeSprite (playerCount);
		playerCount++;

		PlayerGravity playerGrav = playerObj.GetComponent<PlayerGravity> ();		
		playerGrav.gravityOn = false;							//Turn off player gravity initially
		playerGrav.SetClosestGravityCenter ();							//Force player script to determine closest terrain object
		playerGrav.SetOrientation ();							//Force player script to set orientation towards gravity center	
		// Stamp a square area in terrain
		StampSpawnLocation (spawnPoint, spawnTheta);
		playerGrav.gravityOn = true;							//Turn on player gravity

		yield return null;
	}

	Vector2 FindTerrainExtents (GameObject terr) {
		Collider2D[] terrColliders;
		//terrColliders = terr.GetComponentsInChildren<EdgeCollider2D> ();
		terrColliders = terr.GetComponentsInChildren<PolygonCollider2D> ();
		Transform colliderObj = terr.transform.GetChild (0);
		Vector2 colliderScale = colliderObj.localScale;
		Vector2 colliderLocalPos = colliderObj.localPosition;
		float[] ptsX = new float[100000];
		float[] ptsY = new float[100000];
		int i = 0;
		//foreach (EdgeCollider2D coll in terrColliders) {
			//Vector2[] points = coll.points.;
		foreach (PolygonCollider2D coll in terrColliders) {
			Vector2[] points = coll.points;

			foreach (Vector2 pt in points) {
				ptsX[i] = Mathf.Abs (pt.x * colliderScale.x + colliderLocalPos.x);		//Get pt in coordinates relative to terrain center, not collider center
				ptsY[i] = Mathf.Abs (pt.y * colliderScale.y + colliderLocalPos.y);
				i++;
			}
		}
		//Debug.Log ("number of pts " + i);

		float maxX = Mathf.Max (ptsX);	//Max X absolute distance from center of collider object
		float maxY = Mathf.Max (ptsY);	//Max X absolute distance from center of collider object
		Vector2 collExtents = new Vector2 (maxX, maxY);
		//Debug.Log ("collExtents " + collExtents);
		return collExtents;
	}

	void StampSpawnLocation (Vector3 location, float angle) {
		Debug.Log ("Angle " + angle * Mathf.Rad2Deg);
		Vector2 Size = new Vector2(spawnHeight * 2.0f + 0.25f, 1.5f);
		//float Angle = 0.0f;
		float Hardness = 1.0f;

		D2D_Destructible.StampAll(location, Size, angle * Mathf.Rad2Deg, StampTex, Hardness, terrainMask);
	}

}
