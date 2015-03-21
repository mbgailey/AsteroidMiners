using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour {

	public float playerHealth;
	Slider healthDisplay;
	public GameObject damageTextObj;
	public Text damageText;

	Vector3 textStartPos;
	float animSpeed = 4.0f;
	float animTime = 3.0f;
	float timer = 0.0f;

	// Use this for initialization
	void Start () {


		playerHealth = 100.0f;
		healthDisplay = this.GetComponentInChildren<Slider> ();
		damageTextObj = this.transform.FindChild("HealthBar/DamageText").gameObject;
		damageText = damageTextObj.GetComponent <Text> ();
		damageTextObj.SetActive(false);
		textStartPos = damageText.rectTransform.localPosition;
		healthDisplay.value = playerHealth;				//Update health bar display

	}

	public void TakeDamage (float damage) {
		playerHealth -= Mathf.Abs (damage);				//Take abs to not allow negative damage value
		playerHealth = Mathf.Max(0.0f,playerHealth); 	//Don't allow health to go below zero
		healthDisplay.value = playerHealth;				//Update health bar display
		StartCoroutine ("DamageText", damage);			//Start the damage text animation
	}

	//Display damage text and animate it
	public IEnumerator DamageText (float damage) {
		timer = 0.0f;									//Reset timer	
		damageText.text = "-" + damage.ToString ("F0");	//Set text value with no decimal digits
		damageTextObj.SetActive(true);					//Display the text

		while(timer < animTime) {						//Move text upwards locally for span of animTime
			Vector3 newPos = new Vector3 (textStartPos.x, damageText.rectTransform.localPosition.y + animSpeed * Time.deltaTime, textStartPos.z);
			damageText.rectTransform.localPosition = newPos;
			timer += Time.deltaTime;
			yield return null;
		}

		damageTextObj.SetActive(false);					//Hide the text
		damageText.rectTransform.localPosition = textStartPos; //Reset the position
		yield return null;
	}

}
