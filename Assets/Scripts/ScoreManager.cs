using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour {

	Text scoreText;
	PhaseManager phaseManager;
	public Animation scoreAnim;

	float p1Score = 0;
	float p2Score = 0;

	// Use this for initialization
	void Start () {
		scoreText = GameObject.Find ("ScoreText").GetComponent<Text> ();
		scoreAnim = GameObject.Find ("ScorePanel").GetComponent<Animation> ();
		phaseManager = this.GetComponent<PhaseManager> ();
	}
	
	public void AddToScore (float amt) {
		int playerNum = phaseManager.currentPlayerInt + 1;

		if (playerNum == 1) {
			p1Score += amt;
			UpdateScoreText(p1Score);
		}
		else {
			p2Score += amt;
			UpdateScoreText(p2Score);
		}

		if (amt > 0f) {
			scoreAnim.animation.Play ("GUIScoreAnimation");
		}

	}

	public void UpdateScoreText (float score) {
		scoreText.text = score.ToString ("F0");
	}
}
