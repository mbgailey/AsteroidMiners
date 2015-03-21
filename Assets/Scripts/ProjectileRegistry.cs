using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ProjectileRegistry : MonoBehaviour {

	public List<GameObject> projectileList = new List<GameObject>();
	PhaseManager phaseManager;
	bool projectilesLeft = true;
	//public Transform[] projectileList;
	//[HideInInspector] public Vector3[] projectilePosList;
	
	// Use this for initialization
	void Start () {
		projectileList = new List<GameObject>(); 
		phaseManager = this.GetComponent<PhaseManager> ();

	}
	

	// Add item to the registry
	public void AddToRegistry (GameObject proj) {
		projectileList.Add (proj);
        Debug.Log(proj.name);
	}
	
	// Add item to the registry
	public void RemoveFromRegistry (GameObject proj) {
		projectileList.Remove(proj); 
		
	}

	public void CheckProjectilesLeft() {
		if (projectileList.Count > 0) {
			projectilesLeft = true;
		}
		else {
			projectilesLeft = false;
		}
        //Debug.Log(projectileList.Count);
	}

	IEnumerator CheckProjectiles() {

		InvokeRepeating ("CheckProjectilesLeft", 0.5f, 0.5f);

		while (projectilesLeft) {
			//Do nothing
			yield return null;
		}

		phaseManager.NoProjectilesLeft ();	//Alert phase manager when all projectiles are gone
		CancelInvoke ("CheckProjectilesLeft");
		projectilesLeft = true; 			//Set to true so that while loop will work again next time
		yield return null;
	}


}
