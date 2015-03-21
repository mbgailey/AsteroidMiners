using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SliderValueText : MonoBehaviour {

	public Slider slider;
	public Text valueText;

	void Start () {
		valueText = this.gameObject.GetComponent<Text> ();
		valueText.text = slider.value.ToString("F1");
	}

	public void UpdateText () {
		valueText.text = slider.value.ToString("F1");
	}


}
