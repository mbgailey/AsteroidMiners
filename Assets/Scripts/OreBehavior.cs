using UnityEngine;
using System.Collections;

public class OreBehavior : MonoBehaviour {

	public float pointValueMult = 1000.0f;
	float pointValue = 100.0f;
	ScoreManager scoreManager;


	// Use this for initialization
	void Start () {

		scoreManager = GameObject.Find ("GameManager").GetComponent<ScoreManager> ();
		pointValue = pointValueMult * this.transform.localScale.x;		//Set point value based on size of ore
	}
	
	// Update is called once per frame
	public void CollectOre () {

		scoreManager.AddToScore (pointValue);

		StartCoroutine (DestroyEffect());

	}

	IEnumerator DestroyEffect() {
		//Change alpha to 255
		Color temp = this.GetComponent<SpriteRenderer> ().color;
		temp.a = 255.0f;
		this.GetComponent<SpriteRenderer> ().color = temp;	

		yield return new WaitForSeconds(0.5f);	//Pause

		//Play particle effect

		GameObject.Destroy (this.gameObject);	//Destroy this object

	}
}
