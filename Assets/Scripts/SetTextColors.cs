using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SetTextColors : MonoBehaviour {

	public Color normalTextColor;

	public Text[] texts = new Text[10];

	// Use this for initialization
	void Start () {
		texts = this.gameObject.GetComponentsInChildren<Text> ();
		foreach (Text txt in texts) {
			txt.color = normalTextColor;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
