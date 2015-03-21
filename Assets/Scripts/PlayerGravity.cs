using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerGravity : MonoBehaviour {
	
	//[System.NonSerialized]
	//PhaseManager phaseManager;
	TerrainRegistry terrainRegistry;
	PersistentSettings persistentSettings;
	Vector3[] centersList;
	float[] massList;
		
	float playerMass;

	[System.NonSerialized]
	public Vector2 myTerrainCenter;
	float myTerrainMass;
	public bool gravityOn = true;
	public float gravityConst = 6.673e-2f; //Actual gravitational constant is way too small (6.673e-11)
	Vector2 gravDirection;	
	bool ignoreCollision = false;

	bool asteroidLevel;

	////LayerMask terrainMask;
	
	// Use this for initialization
	void Awake () {
		//phaseManager = GameObject.Find("GameManager").GetComponent<PhaseManager>();
		persistentSettings = GameObject.Find("GameSettings").GetComponent<PersistentSettings>();
		terrainRegistry = GameObject.Find("GameManager").GetComponent<TerrainRegistry>();
		gravityOn = false;
		
		centersList = new Vector3[10];	// declare a float built in array. Max number is fixed unless array is resized.
		massList = new float[10];

		if (persistentSettings.levelType == PersistentSettings.LevelType.Asteriod) {
			this.rigidbody2D.gravityScale = 0.0f;		//If this is an asteroid level, set gravity to zero
			asteroidLevel = true;
		}
		else {
			this.rigidbody2D.gravityScale = 0.0f;		//If this is an asteroid level, set gravity to zero
			asteroidLevel = false;
		}
		playerMass = this.rigidbody2D.mass;
		terrainRegistry.gravCentersList.CopyTo(centersList, 0);
		terrainRegistry.gravMassList.CopyTo(massList, 0);

		//int terrainLayer = 10;
		//terrainMask = 1 << terrainLayer;

	}

	void Start () {
		//SetGravityCenter ();
		//Debug.Log ("centersList " + centersList);
		//Debug.Log ("massList " + massList);
		//Debug.Log (myTerrainCenter + "  " + myTerrainMass);
	}

	// Update is called once per frame
	void FixedUpdate () 
	{
		Vector2 gravForce;
		if (asteroidLevel) {
			gravDirection = myTerrainCenter - (Vector2)this.transform.position; //Determine direction from player to gravity center
			float squareDist = gravDirection.sqrMagnitude;			//Calculate the squared distance between player and gravity center
			gravDirection = gravDirection.normalized;
			gravForce = gravityConst * myTerrainMass * playerMass / squareDist * gravDirection;	//Calculate gravitational force
		}
		else {
			gravDirection = -Vector2.up;
			gravForce = gravDirection * 9.8f * playerMass;
		}
		if ( gravityOn ) {
			//Apply gravity force
			this.rigidbody2D.AddForce(gravForce);
			SetOrientation();
		}

	}

	public void SetOrientation () 
	{
		//Keep feet pointing in direction of gravity

		if (asteroidLevel) {
			gravDirection = myTerrainCenter - (Vector2)this.transform.position;
			gravDirection = gravDirection.normalized;
			this.transform.rotation = Quaternion.FromToRotation (Vector3.up, -gravDirection);
		}
		else {
			this.transform.rotation = Quaternion.FromToRotation (Vector3.up, -gravDirection);
		}

	}

	public void SetClosestGravityCenter () 
	{
		//Set gravity center as the closest terrain object
		int i = 0;
		float closestDist = 0f;	//Initialize with very large number

		foreach( float mass in massList ) 
		{
			if (mass > 0) 
			{
				Vector2 gravDirection = centersList[i] - this.transform.position; //Determine direction from projectile to each gravity center
				float squareDist = gravDirection.sqrMagnitude;				//Calculate the squared distance between player and gravity center

				if ( squareDist < closestDist || closestDist == 0f )		//If squareDist is null (first comparison) or less than last closest distance
				{
					myTerrainCenter = centersList[i];						//Set the player's terrain center as this one
					myTerrainMass = mass;
					closestDist = squareDist;								//Set the closestDist to this distance
				}
			}
			i++;
		}
		if ( myTerrainCenter == new Vector2(0f,0f) ) 			//If no center was chosen for some reason///THIS DOESN't work yet
		{
			myTerrainCenter = centersList[0];	//Provide the first terrain object in the array as the default
		}
	}

	void OnCollisionEnter2D(Collision2D coll) {
		//Stop player gravity whenever terrain is collided with
		//Debug.Log ("collided with " + coll.transform.parent.name);
		if (gravityOn && (coll.gameObject.layer == 10 || coll.transform.tag == "Ore") && !ignoreCollision) {	//Layer 10 is terrain
			gravityOn = false;
			this.rigidbody2D.isKinematic = true;
		}

	}

	public IEnumerator TemporarilyDisableGravity() {
		//Stop player gravity whenever terrain is collided with
		//Debug.Log ("collided with " + coll.transform.parent.name);
		this.rigidbody2D.isKinematic = false;
		gravityOn = true;
		ignoreCollision = true;

		yield return new WaitForSeconds (1.0f);

		ignoreCollision = false;

	}

    public void EnableGravity()
    {
        //Use when moving player for mining. Re-enable gravity after moving
        this.rigidbody2D.isKinematic = false;
        gravityOn = true;
        ignoreCollision = false;

    }

}
