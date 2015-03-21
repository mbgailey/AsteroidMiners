using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DragButton : MonoBehaviour {
    //Controls the UI display for aiming

    bool pulsing = false;
    bool aimOn = false;
    float pulseRate = 1.0f;
    int direction = 1;
    float initialScale;
    //Vector3 initialScaleVect;
    float maxScale = 1.0f;
    RectTransform thisRect;
    RectTransform glowRingRect;
    Image glowRingIm;
    RectTransform aimDirectionIndicatorRect;
    Vector2 defaultRect;
    float maxRectHt = 150.0f;
    Image aimDirectionIndicatorIm;

    GameObject targetObject;
    //PlayerShootController playerShootController;

	// Use this for initialization
	void Start () {
        thisRect = this.GetComponent<RectTransform>();
        glowRingRect = this.transform.FindChild("DragButton").GetComponent<RectTransform>();
        glowRingIm = this.transform.FindChild("DragButton").GetComponent<Image>();
        aimDirectionIndicatorRect = this.transform.FindChild("AimDirectionIndicator").GetComponent<RectTransform>();
        defaultRect = aimDirectionIndicatorRect.sizeDelta;
        aimDirectionIndicatorIm = this.transform.FindChild("AimDirectionIndicator").GetComponent<Image>();
        aimDirectionIndicatorIm.enabled = false;
        initialScale = glowRingRect.localScale.x;
        //initialScaleVect = new Vector3(initialScale, initialScale, 1.0f);

        //playerShootController = transform.GetComponentInParent<PlayerShootController>();

        HideGlowRing();
	}
	
	// Update is called once per frame
	void Update () {
        if (pulsing)
        {
            PulseImage();
        }
        if (aimOn && targetObject != null)
        {
            thisRect.position = Camera.main.WorldToScreenPoint(targetObject.transform.position);
        }
	}

    public void ShowGlowRing()
    {
        glowRingIm.enabled = true;
        aimOn = true;
        pulsing = true;
    }

    public void ShowGlowRingGUI(GameObject obj)
    {
        targetObject = obj;
        glowRingIm.enabled = true;
        aimOn = true;
        pulsing = true;
    }

    public void HideGlowRing()
    {
        glowRingIm.enabled = false;
        HideAimIndicator();
        aimOn = false;
        pulsing = false;
    }
    
    public void ShowAimIndicator(float scale, float shootAngle)
    {
        if (!aimDirectionIndicatorIm.enabled)
        {
            aimDirectionIndicatorIm.enabled = true;
        }
        Vector2 temp = defaultRect;
        float ht = maxRectHt * scale;
        temp.y = Mathf.Max(ht, defaultRect.y);
        aimDirectionIndicatorRect.sizeDelta = temp;
        aimDirectionIndicatorRect.transform.localRotation = Quaternion.Euler(0f, 0f, (float)shootAngle + 90f);	//Move reticle to local shoot angle. Zero degrees is to the player right, 90 degrees is player up
    }

    public void HideAimIndicator()
    {
        aimDirectionIndicatorIm.enabled = false;
        aimDirectionIndicatorRect.sizeDelta = defaultRect;
    }

    void PulseImage()
    {
        Vector3 temp = glowRingRect.localScale;
        if (direction == 1)
        {
            temp.x = Mathf.Lerp(glowRingRect.localScale.x, maxScale, pulseRate * 4.0f * Time.deltaTime);
        }
        else
        {
            temp.x = Mathf.Lerp(glowRingRect.localScale.x, initialScale, pulseRate * Time.deltaTime);
        }
        temp.y = temp.x;
        glowRingRect.localScale = temp;

        if (direction == 1 && maxScale - glowRingRect.localScale.x < 0.05f)
        {
            direction *= -1; //Reverse direction
        }

        if (direction == -1 && glowRingRect.localScale.x - initialScale < 0.05f)
        {
            direction *= -1; //Reverse direction
        }
    }

}
