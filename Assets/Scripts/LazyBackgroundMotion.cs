using UnityEngine;
using System.Collections;

public class LazyBackgroundMotion : MonoBehaviour {
	
	float minDriftTime;
	float maxDriftTime;
	float minSpeed;
	float maxSpeed;
	public float driftSpeedX;
	public float driftSpeedY;
	float elapsedTime;
	float currentCycleTime;
	int dirX;
	int dirY;
	Vector2 initialPosition;
	Vector2 maxDisp;
	
	
	// Use this for initialization
	void Start () {
		minDriftTime = 3.0f;
		maxDriftTime = 10.0f;
		minSpeed = 0.1f;
		maxSpeed = 0.5f;
		elapsedTime = 0.0f;
		maxDisp = new Vector2 (75f, 75f);

		initialPosition = (Vector2)this.transform.position;
		SetRandomParameters ();
	}
	
	// Update is called once per frame
	void Update () {
		
		if (elapsedTime >= currentCycleTime) {
			elapsedTime = 0.0f;
			SetRandomParameters();
		}

		Vector2 currentDisp = (Vector2)this.transform.position - initialPosition;

		if (currentDisp.x > maxDisp.x) 
		{
			dirX *= -1;
		}

		if (currentDisp.x > maxDisp.x) 
		{
			dirY *= -1;
		}

		float posX = this.transform.position.x + driftSpeedX * Time.deltaTime * dirX;
		float posY = this.transform.position.y + driftSpeedY * Time.deltaTime * dirY;
		float posZ = this.transform.position.z;
		this.transform.position = new Vector3(posX, posY, posZ);
		//this.transform.position( Vector3(driftSpeedX * Time.deltaTime, driftSpeedY * Time.deltaTime));

		elapsedTime += Time.deltaTime;
	}
	
	void SetRandomParameters () {
		currentCycleTime = Random.Range (minDriftTime, maxDriftTime);
		driftSpeedX = Random.Range (minSpeed, maxSpeed);
		driftSpeedY = Random.Range (minSpeed, maxSpeed);

		dirX = Random.Range (0, 1);
		if (dirX == 0)
		{
			dirX = -1;
		}
		dirY = Random.Range (0, 1);
		if (dirY == 0)
		{
			dirY = -1;
		}

	}
}
