using UnityEngine;
using System.Collections;

public class PlayerRegistry : MonoBehaviour {

	[HideInInspector] public GameObject[] playerList;
	int numberOfPlayers = 2;
	int currentIndex = 0;

	// Use this for initialization
	void Start () {
		playerList = new GameObject[numberOfPlayers];
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void AddToRegistry (GameObject player) {
		playerList [currentIndex] = player;
		currentIndex++;
	}
}
