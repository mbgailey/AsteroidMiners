using UnityEngine;
using System.Collections;

public class OreCreator : MonoBehaviour {

	public GameObject orePrefab;

	public float maxTerrainRadius = 12.6f;

	public int minVeins = 1;
	public int maxVeins = 3;

	public int minNodesPerVein = 5;
	public int maxNodesPerVein = 20;

	public float minRadialStep = 0.25f;
	public float maxRadialStep  = 0.75f;

	public float minThetaStep = 3.0f;
	public float maxThetaStep  = 8.0f;

	public int minOresPerNode = 5;
	public int maxOrePerNode  = 15;

	public float minOreSpacing = 0.1f;
	public float maxOreSpacing  = 0.6f;

	public float minScale = 0.15f;
	public float maxScale = 0.5f;

	// Use this for initialization
	void Start () {

		GameObject oreGroup = new GameObject ();			//Create empty group
		oreGroup.name = "OreGroup";
		oreGroup.transform.parent = this.transform;			//Make child of terrain object
		oreGroup.transform.localPosition = Vector3.zero;	//Set position at center of terrain

		//Choose random number of veins
		int veins = Random.Range (minVeins, maxVeins + 1);
		for (int v = 0; v < veins; v++) {

			float startTheta = Random.Range (0f, 360.0f);
			float startRadius = Random.Range (0f, maxTerrainRadius);			//Set max radius
			Vector3 startPos = CylindricalToCartesion(startRadius, startTheta);	//Initialize vein (node group) start position to a random location inside terrain

			//Debug.Log ("startPos" + startPos);

			GameObject nodeGroup = new GameObject ();			//Create empty group
			nodeGroup.name = "NodeGroup";
			nodeGroup.transform.parent = oreGroup.transform;	//Make child of oreGroup object
			nodeGroup.transform.localPosition = startPos;		//Set position at start location	//Will make z position zero

			float nodeRad = startRadius;							//Initialize node position
			float nodeTheta = startTheta;	

			int nodes = Random.Range (minNodesPerVein, maxNodesPerVein + 1);
			for (int n = 0; n < nodes; n++) {
				//Determine random scale for ore
				float scale = Random.Range (minScale, maxScale);
				//Make a random step for each new node. Scale step proportionally to 
				float radStep = Random.Range (minRadialStep, maxRadialStep) * RandomSign() * (scale + 0.5f);
				float thetaStep = Random.Range (minThetaStep, maxThetaStep) * RandomSign() * (scale + 0.5f);
				nodeRad += radStep;
				nodeRad = Mathf.Clamp(nodeRad, 0.0f, maxTerrainRadius); //Don't allow radius to be greater than terrain radius limit
				nodeTheta += thetaStep;
				//Debug.Log ("nodeRad" + nodeRad + "nodeTheta" + nodeTheta);
				Vector3 nodePos = CylindricalToCartesion(nodeRad, nodeTheta);
				//Debug.Log ("nodePos" + nodePos);
				//Debug.Log (" ");
				GameObject ore = (GameObject)GameObject.Instantiate(orePrefab); 
				ore.transform.parent = oreGroup.transform;			//Make child of oreGroup object that has center at center of terrain
				ore.transform.localPosition = nodePos;				//Set position relative to oreGroup
				ore.transform.parent = nodeGroup.transform;			//Make child of the nodeGroup
				ore.transform.localScale = new Vector3 (scale, scale, 1.0f);
				ore.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, Random.Range (0f, 360.0f));	//Give ore a random rotation
//				int ores = Random.Range (minOresPerNode, maxOrePerNode + 1);
//				Vector3 orePos = Vector3.zero;						//Reference position is 0,0,0 local to nodeGroup position
//				for (int r = 0; r < ores; r++) {
//
//				}



			}
		}

	}

	Vector3 CylindricalToCartesion(float rad, float thet) {
		//Returns the cartesion x, y coordinates given cylindrical coordinates radius and theta
		//Makes z coordinate zero
		float xCoord = rad * Mathf.Cos (thet * Mathf.Deg2Rad);
		float yCoord = rad * Mathf.Sin (thet * Mathf.Deg2Rad);

		return new Vector3 (xCoord, yCoord, 0f);
	}

	int RandomSign () {
		return Random.value < 0.5f? 1 : -1;
	}


}
