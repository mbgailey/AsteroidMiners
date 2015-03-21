using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerIndicatorBehavior : MonoBehaviour {

    RectTransform thisRect;
    GameObject targetObject;
    Image indicatorIm;
    Color initialColor;
    float targetAlpha = 0.05f;

    bool indicatorOn = false;
    int direction = -1;
    float pulseRate = 5.0f;
    

	// Use this for initialization
	void Start () {
	    thisRect = this.GetComponent<RectTransform>();
        indicatorIm = this.GetComponent<Image>();
        initialColor = indicatorIm.color;
	}
	
	// Update is called once per frame
	void Update () {
	    if (indicatorOn) 
        {
            thisRect.position = Camera.main.WorldToScreenPoint(targetObject.transform.position);
            Vector2 temp = (Vector2)targetObject.transform.up;
            float angle = Angle360(temp);
            thisRect.transform.localRotation = Quaternion.Euler(0f, 0f, angle-90f);
            PulseAlpha();
            
        }
	}

    public void SetTargetObject(GameObject obj)
    {
        targetObject = obj;
        indicatorOn = true;
    }

    public void HideIndicator()
    {
        indicatorOn = false;
        indicatorIm.color = initialColor;
    }


    public static float Angle360(Vector2 p_vector2)
    {
        return (Mathf.Atan2(p_vector2.y, p_vector2.x) * Mathf.Rad2Deg);

    }

    void PulseAlpha()
    {
        //pulse the image alpha from initial state to zero back and forth
        float alpha = indicatorIm.color.a;
        if (direction == 1)
        {
            alpha = Mathf.Lerp(indicatorIm.color.a, initialColor.a, pulseRate * 2.0f * Time.deltaTime);
        }
        else
        {
            alpha = Mathf.Lerp(indicatorIm.color.a, targetAlpha, pulseRate * Time.deltaTime);
        }

        Color temp = initialColor;
        temp.a = alpha;
        indicatorIm.color = temp;

        if (direction == 1 && indicatorIm.color.a >= initialColor.a-.01f)
        {
            direction *= -1; //Reverse direction
        }

        if (direction == -1 && indicatorIm.color.a <= targetAlpha+.01f)
        {
            direction *= -1; //Reverse direction
        }
    }
}
