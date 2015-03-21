using UnityEngine;
using System.Collections;
using System;

public class DestroyAfterTime : MonoBehaviour {

	public float destroyTime = 3.0f;

	// Use this for initialization
	void Start () {
		Destroy (this.gameObject, destroyTime); //Destroy after 3 seconds
	}
	
	// Update is called once per frame
	void Update () {

	}
}
