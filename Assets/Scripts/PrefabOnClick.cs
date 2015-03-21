﻿using UnityEngine;
using System.Collections;

public class PrefabOnClick : MonoBehaviour {

    public GameObject prefab;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0))
        {
            Instantiate(prefab,Camera.main.ScreenToWorldPoint(Input.mousePosition),Quaternion.identity);

        }
	}
}
