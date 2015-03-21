using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ProjectileGravity : MonoBehaviour {
	
	//[System.NonSerialized]
	//private PhaseManager phaseManager;
	private TerrainRegistry terrainRegistry;
	Vector3[] centersList;
	float[] massList;
	
	Vector3[] gravDirections;
	float[] squareDists;                 // declare a float built in array. Max number is fixed unless array is resized.
	Vector3[] gravForces;
	
	float projMass;
	Vector3 totalForce;			//Total force
	
	bool gravityOn = true;
	public float gravityConst = 6.673e-2f; //Actual gravitational constant is way too small (6.673e-11)
	//float thetaForcePercent = 0.5f; //Percent of gravity force to be applied in the theta direction (this gives a more orbital trajectory to projectiles)
	//float gravityAcc = 9.8f;

	PersistentSettings persistentSettings;
	public bool asteroidLevel;
	
	// Use this for initialization
	void Start () {
		//phaseManager = GameObject.Find("GameManager").GetComponent<PhaseManager>();
		terrainRegistry = GameObject.Find("GameManager").GetComponent<TerrainRegistry>();
		persistentSettings = GameObject.Find("GameSettings").GetComponent<PersistentSettings>();

		if (persistentSettings.levelType == PersistentSettings.LevelType.Asteriod) {
			this.rigidbody2D.gravityScale = 0.0f;		//If this is an asteroid level, set gravity to zero
			asteroidLevel = true;
		}
		else {

			this.rigidbody2D.gravityScale = 1.0f;		//If this is a planet level, set gravity to 1.0
			asteroidLevel = false;
		}

		gravityOn = true;
		
		centersList = new Vector3[10];
		massList = new float[10];
		
		gravDirections = new Vector3[10];
		squareDists = new float[10];                 // declare a float built in array. Max number is fixed unless array is resized.
		gravForces = new Vector3[10];

		projMass = this.rigidbody2D.mass;
		terrainRegistry.gravCentersList.CopyTo(centersList, 0);
		terrainRegistry.gravMassList.CopyTo(massList, 0);
		


	}
	
	// Update is called once per frame
	void FixedUpdate () {

		if (asteroidLevel) {
			int i = 0;
			totalForce = new Vector3(0f,0f,0f);
			foreach( float mass in massList ) {
				if (mass > 0) {
					gravDirections[i] = centersList[i] - this.transform.position; //Determine direction from projectile to each gravity center
					squareDists[i]  = gravDirections[i].sqrMagnitude;			//Calculate the squared distance between projectile and gravity center
					gravDirections[i] = gravDirections[i].normalized;
					gravForces[i] = gravityConst * mass * projMass / squareDists[i] * gravDirections[i];	//Calculate gravitational force
					totalForce += gravForces[i];
					i++;
				}
			}
			
			if ( gravityOn ) {
				//Apply gravity force
				this.rigidbody2D.AddForce(totalForce);
			}
		}
	}
}
