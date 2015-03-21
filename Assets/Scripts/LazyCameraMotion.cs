using UnityEngine;
using System.Collections;

public class LazyCameraMotion : MonoBehaviour {

	float minDriftTime;
	float maxDriftTime;
	float minSpeed;
	float maxSpeed;
	float driftSpeedX;
	float driftSpeedY;
	float driftSpeedZ;
	float elapsedTime;
	float currentCycleTime;


	// Use this for initialization
	void Start () {
		minDriftTime = 3.0f;
		maxDriftTime = 10.0f;
		minSpeed = -0.25f;
		maxSpeed = 0.25f;
		elapsedTime = 0.0f;

		SetRandomParameters ();
	}
	
	// Update is called once per frame
	void Update () {

		if (elapsedTime >= currentCycleTime) {
			elapsedTime = 0.0f;
			SetRandomParameters();
		}

		this.transform.Rotate( driftSpeedX * Time.deltaTime, driftSpeedY * Time.deltaTime, driftSpeedZ * Time.deltaTime);

		elapsedTime += Time.deltaTime;
	}

	void SetRandomParameters () {
		currentCycleTime = Random.Range (minDriftTime, maxDriftTime);
		driftSpeedX = Random.Range (minSpeed, maxSpeed);
		driftSpeedY = Random.Range (minSpeed, maxSpeed);
		driftSpeedZ = Random.Range (minSpeed, maxSpeed);
	}
}
