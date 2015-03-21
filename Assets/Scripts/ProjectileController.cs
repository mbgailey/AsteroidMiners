using UnityEngine;
using System.Collections;

public class ProjectileController : MonoBehaviour {

	public float controlForce = 1.0f;
	float upForce;
	float sideForce;

	public bool allowTouchInput = true;
	public float swipeForceMult = 200.0f;
	float minSwipeDist;
	private Vector2 swipeStartPos;
	private Vector2 swipeVect;
	private Vector2 swipeDir;
	private float swipeDist;
	private bool swipe = false;
    public bool controlsActive = false;

    ProjectileDisplayController displayController;

	// Use this for initialization
	void Start () {
		minSwipeDist = 2.0f;
        displayController = this.GetComponent<ProjectileDisplayController>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {

        if (controlsActive)
        {
            //Look for inputs
            upForce = 0.0f;
            sideForce = 0.0f;

            if (Input.GetAxisRaw("Horizontal") != 0)
            {
                upForce = Input.GetAxisRaw("Horizontal") * controlForce * Time.deltaTime;
            }
            if (Input.GetAxisRaw("Vertical") != 0)
            {
                sideForce = Input.GetAxisRaw("Vertical") * controlForce * Time.deltaTime;
            }

            if (!swipe)
            {
                SwipeControls();
            }
            else
            {
                Vector2 swipeForce = swipeDir * swipeForceMult;
                //Debug.Log (swipeForce);
                this.rigidbody2D.AddForce(swipeForce, ForceMode2D.Impulse);
                displayController.EmitSteerParticles(-swipeForce.normalized);
                swipe = false;
                Debug.Log("Swipe");
            }

            Vector2 force = new Vector2(upForce, sideForce);
            if (force.sqrMagnitude != 0)
            {
                Debug.DrawLine(this.transform.position, this.transform.position + (Vector3)force.normalized * 5.0f, Color.red);
                displayController.EmitSteerParticles(-force.normalized);
                Debug.Log("force " + force);
            }
            this.rigidbody2D.AddForce(force);
        }

	}

	void SwipeControls()
	{
		if (Input.touchCount > 0) 	
		{
			
			Touch touch = Input.touches[0];
			
			switch (touch.phase) 
			{
			case TouchPhase.Began:
				swipeStartPos = touch.position;
				break;
				
			case TouchPhase.Ended:
				swipeVect = touch.position - swipeStartPos;
				swipeDist = swipeVect.magnitude;
				swipeDir = swipeVect.normalized;
				if (swipeDist > minSwipeDist) 
				{
					swipe = true;
				}
				break;
			}
		}
	}

}
