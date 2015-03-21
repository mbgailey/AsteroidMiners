using UnityEngine;
using System.Collections;

public class PlayerSpriteManager : MonoBehaviour {

	public Sprite[] playerSprites;
	public Color[] playerColors;

	SpriteRenderer playerRenderer;
	SpriteRenderer arrowRenderer;


	// Use this for initialization
	void Awake () {
		playerRenderer = this.GetComponent<SpriteRenderer> ();
		arrowRenderer = this.transform.Find ("Arrow").GetComponent<SpriteRenderer> ();
	}
	
	// Update is called once per frame
	public void ChangeSprite (int playerNum) {
		playerRenderer.sprite = playerSprites [playerNum];
		arrowRenderer.color = playerColors [playerNum];
		arrowRenderer.enabled = false;
	}

	public void FlashArrow () {
		StartCoroutine ("FlashSprite", arrowRenderer);
	}

	IEnumerator FlashSprite (SpriteRenderer rend) {
		//Flash the sprite by making transparent
		////Color startColor = rend.color;
		//Color altColor = Color.
		int totalFlashes = 5;
		float flashTime = 0.25f;
		int flashCount = 0;
		while (flashCount < totalFlashes) {
			rend.enabled = true;
			yield return new WaitForSeconds(flashTime);
			rend.enabled = false;
			yield return new WaitForSeconds(flashTime);
			flashCount++;
			yield return null;
		}
		yield return null;
	}

}
