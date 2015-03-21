using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class GUIManager : MonoBehaviour {

	//GUI links
	public Text turnText;
	public Text instructionText;
    public GameObject toolTip;
    public Text toolTipText;
    public Image toolTipImage;

    string baseText1 = "Velocity\n";
    string baseText2 = " m/s";
    bool velocityDisplayOn = true;
    GameObject trackedProjectile;

	// Use this for initialization
	void Start () {
        DisableVelocityToolTip();
	}
	
	public void RefreshTurnText (string currentPlayer) {
		turnText.text = currentPlayer;
	}

	public void EnableInstructionText () {
		turnText.enabled = true;
		instructionText.enabled = true;
	}

	public void DisableInstructionText () {
		turnText.enabled = false;
		instructionText.enabled = false;
	}

    public void EnableVelocityToolTip(GameObject projectile)
    {
    //Show velocity on screen and track a projectile    
        trackedProjectile = projectile;
        toolTipText.enabled = true;
        toolTipImage.enabled = true;
        InvokeRepeating("UpdateVelocity", 0.0f, 0.25f);
    }

    public void EnableVelocityToolTip()
    {
    //Show velocity on screen but don't track a projectile   
        trackedProjectile = null;
        toolTipText.enabled = true;
        toolTipImage.enabled = true;
        InvokeRepeating("UpdateVelocity", 0.0f, 0.25f);
    }

    public void DisableVelocityToolTip()
    {
        trackedProjectile = null;
        toolTipText.enabled = false;
        toolTipImage.enabled = false;
        CancelInvoke("UpdateVelocity");
    }

    void UpdateVelocity()
    {
        float vel = 0.0f;
        if (trackedProjectile != null)
        {
            vel = trackedProjectile.rigidbody2D.velocity.magnitude;
        }

        toolTipText.text = baseText1 + vel.ToString("F1") + baseText2;
        
    }
}
