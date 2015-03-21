using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ImagePulse : MonoBehaviour {
    //Pulses a UI image scale at a given rate

    float pulseRate = 1.0f;
    int direction = 1;
    float initialScale;
    //Vector3 initialScaleVect;
    float maxScale = 0.5f;
    RectTransform imageRect;

	// Use this for initialization
	void Start () {
	    imageRect = this.GetComponent<RectTransform>();
        initialScale = imageRect.localScale.x;
        //initialScaleVect = new Vector3(initialScale, initialScale, 1.0f);
	}
	
	// Update is called once per frame
	void Update () {

        Vector3 temp = imageRect.localScale;
        if (direction == 1)
        {
            temp.x = Mathf.Lerp(imageRect.localScale.x, maxScale, pulseRate * 4.0f * Time.deltaTime);
        }
        else
        {
            temp.x = Mathf.Lerp(imageRect.localScale.x, initialScale, pulseRate * Time.deltaTime);
        }
        temp.y = temp.x;
        imageRect.localScale = temp;

        if (direction == 1 && maxScale - imageRect.localScale.x < 0.05f)
        {
            direction *= -1; //Reverse direction
        }

        if (direction == -1 && imageRect.localScale.x - initialScale < 0.05f)
        {
            direction *= -1; //Reverse direction
        }

	}
}
