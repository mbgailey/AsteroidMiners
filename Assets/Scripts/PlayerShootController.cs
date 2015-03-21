using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerShootController : MonoBehaviour {

	public Slider powerSlider;
	public Text angleText;
	public Text currentWeaponText;
	public Text prevWeaponText;
	public Text nextWeaponText;

	public float localShootAngle;
	public float shootAngle;
	public float shootPower;
	float minPower;
	float maxPower;
	public float basePower;
	float changeAngleSpeed;
	float changePowerSpeed;

	PhaseManager phaseManager;
	GameObject muzzlePoint;
	GameObject gunBarrel;
	WeaponMessageRefresher weaponMessage;
	PlayerSpriteManager playerSpriteManager;
	SwitchWeaponCommands weaponGUI;
    ProjectileRegistry projectileRegistry;
    GUIManager guiManager;
	enum WeaponList {MiniRocket, ClusterRocketMk3, Grenade};
	public List<GameObject> weaponPrefabs = new List<GameObject>();
		
	WeaponList currentWeapon;
	public string currentWeaponStr;
	int currentWeaponInt;
	GameObject currentWeaponObj;
	string prevWeaponStr;
	string nextWeaponStr;
	int weaponCount;

	public bool allowTouchInput = true;
	public float minSwipeDist = 1.0f;
	public float swipePowerMult = 0.12f;
	private Vector2 swipeStartPos;
	private Vector2 swipeVect;
	private Vector2 swipeDir;
	private float swipeDist;
	private bool swipe = false;
    [HideInInspector]
    public bool fireButton = false;

    public bool startDrag = false;
    float minDragDist = 50.0f;
    //float maxDragDist = 10.0f;
    float dragDist;
    Vector2 dragDir;
    bool drag;
    DragButton dragButton;
    float minTouchDist = 50.0f;

    public enum TouchMode { SwipePower, DragPull, HoldThenSwipe };
    public TouchMode touchMode;
    GameObject projectile;

	CameraZoom camZoomControl;

	//@HideInInspector var pathPreview : ProjectilePathPreview;

	// Use this for initialization
	void Start () {
        touchMode = TouchMode.DragPull;
        
		shootPower = 10.0f;
		basePower = 50.0f;
		changeAngleSpeed = 50.0f;
		changePowerSpeed = 10.0f;
		localShootAngle = 90.0f;

		//minPower = powerSlider.minValue;
		//maxPower = powerSlider.maxValue;
		minPower = 10.0f;
		maxPower = 50.0f;
        

		phaseManager = GameObject.Find("GameManager").GetComponent<PhaseManager>();
		//weaponMessage = GameObject.Find("MainCamera").GetComponentInChildren<WeaponMessageRefresher>();
		gunBarrel = this.transform.FindChild ("Barrel").gameObject;
		muzzlePoint = gunBarrel.transform.FindChild ("MuzzlePoint").gameObject;
        //dragButton = muzzlePoint.transform.FindChild("ProjectileAimCanvas").FindChild("DragButton").GetComponent<DragButton>();
        dragButton = GameObject.Find("PlayerAimDisplay").GetComponent<DragButton>();
        playerSpriteManager = this.GetComponent<PlayerSpriteManager>();
        guiManager = GameObject.Find("GUICanvas").GetComponent<GUIManager>();
        projectileRegistry = GameObject.Find("GameManager").GetComponent<ProjectileRegistry>();
        
		weaponCount = System.Enum.GetValues(typeof(WeaponList)).Length;

		currentWeapon = WeaponList.MiniRocket;
		currentWeaponStr = currentWeapon.ToString();
		currentWeaponInt = 0;
		currentWeaponObj = weaponPrefabs[currentWeaponInt];

		

		camZoomControl = Camera.main.GetComponent<CameraZoom> ();
		//pathPreview = this.GetComponentInChildren(ProjectilePathPreview)

        //Link to GUI objects
        weaponGUI = GameObject.Find("GUICanvas").GetComponentInChildren<SwitchWeaponCommands>();
        powerSlider = GameObject.Find("PowerSlider").GetComponent<Slider>();
        powerSlider.maxValue = maxPower;
        angleText = GameObject.Find("AngleValue").GetComponent<Text>();

        UpdateGUIDisplay();	//Initially set GUI displays to default values;
        
	}
	
	// Update is called once per frame
	public IEnumerator Aim () {
	//This is the players weapon selection and aiming phase
        
        playerSpriteManager.FlashArrow ();	//Flash arrow at the beginning of the aim phase to show where current player is
		UpdateGUIDisplay();					//Update weapon GUI display when switching between players
		yield return new WaitForSeconds (2.5f); //Don't allow control until the arrow flashing is finished. It's annoying to be swiping on the prev player and accidentally fire.

        guiManager.EnableVelocityToolTip();
        CreateProjectile();
        dragButton.ShowGlowRingGUI(this.gameObject);

		swipe = false;
        drag = false;
        fireButton = false;
		while ( !Input.GetKeyDown(KeyCode.LeftControl) && !swipe && !fireButton && !drag) {
			//Look for touch controls
			SwipeControls();
			//Debug.Log (swipe);
			//Look for inputs
			if(Input.GetKey(KeyCode.LeftArrow)){
				localShootAngle += changeAngleSpeed * Time.deltaTime;
			}
			if(Input.GetKey(KeyCode.RightArrow)){
				localShootAngle -= changeAngleSpeed * Time.deltaTime;
			}
			if(Input.GetKey(KeyCode.UpArrow)){
				shootPower += changePowerSpeed * Time.deltaTime;
				shootPower = Mathf.Min (maxPower, Mathf.Max(minPower, shootPower));
			}
			if(Input.GetKey(KeyCode.DownArrow)){
				shootPower -= changePowerSpeed * Time.deltaTime;
				shootPower = Mathf.Min (maxPower, Mathf.Max(minPower, shootPower));
			}
			
			if(Input.GetKeyDown(KeyCode.A)){
				SwitchWeapons("up");
			}
			if(Input.GetKeyDown(KeyCode.Z)){
				SwitchWeapons("down");
			}
			if(Input.GetKeyDown(KeyCode.Alpha1)){
				SwitchWeapons(1);
			}
			if(Input.GetKeyDown(KeyCode.Alpha2)){
				SwitchWeapons(2);
			}
			if(Input.GetKeyDown(KeyCode.Alpha3)){
				SwitchWeapons(3);
			}
			if(Input.GetKeyDown(KeyCode.Alpha4)){
				SwitchWeapons(4);
			}

			//Update the screen display 
			shootAngle = localShootAngle + this.transform.eulerAngles.z;    //Convert local angle to global angle

			UpdateGUIDisplay();
			MoveBarrel();
			////weaponMessage.RefreshDisplay(localShootAngle,shootPower,currentWeaponStr);
			yield return null;
		}

		if (swipe && touchMode == TouchMode.SwipePower) {
        //If using the Swipe Power controls
			//shootAngle = Vector2.Angle(Vector2.right,swipeDir);
			shootAngle = Angle360(swipeDir);
			shootPower = swipeDist * swipePowerMult;

            AdjustPowerDisp(shootPower);

            //Debug.Log ("Swipe Dir: " + swipeDir);
            //Debug.Log ("Shoot Angle: " + shootAngle);
            //Debug.Log ("Swipe Power: " + swipeDist);
            //Debug.Log ("Shoot Power: " + shootPower);
		}

        if (drag && touchMode == TouchMode.DragPull)
        {
            //If using the Drag Pull controls
            //shootAngle = Vector2.Angle(Vector2.right,swipeDir);
            //Debug.Log("PLAYOAUO");
            //shootAngle = Angle360(dragDir);
            //shootPower = dragDist * swipePowerMult;

            //AdjustPowerDisp(shootPower);

            Debug.Log("Swipe Dir: " + swipeDir);
            Debug.Log("Shoot Angle: " + shootAngle);
            Debug.Log("Swipe Power: " + swipeDist);
            Debug.Log("Shoot Power: " + shootPower);
        }

        Debug.Log("Shoot Angle: " + shootAngle);
        Debug.Log("Shoot Power: " + shootPower);

        //If selected weapon is mining tool, start mining
        if (currentWeaponInt == 2) {
            StartCoroutine ("StartMining");
        }
        else //Otherwise fire projectile
        {
            StartCoroutine("FireRock");
        }
		yield return null;
	}

    public void CreateProjectile()
    {
        if (projectile != null)
        {
            DestroyImmediate(projectile);   //This isn't ideal as it is more intensive than Destroy. In future never destroy projectiles, just move them out of the play space and move them back in when needed.
        }

        //Spawn the currently selected weapon
        projectile = (GameObject)Instantiate(currentWeaponObj, muzzlePoint.transform.position, Quaternion.identity);
        projectile.transform.SetParent(muzzlePoint.transform);  //Make parent the muzzle point so that it moves with the muzzle point
        projectile.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
        if (projectile.rigidbody2D != null)
        {
            projectile.rigidbody2D.isKinematic = true;

        }
    }
	public void UpdateGUIDisplay() {
		AdjustPowerDisp(shootPower);
		AdjustAngleDisp(localShootAngle);
		//AdjustWeaponDisp();
		weaponGUI.ResetDisplay (currentWeaponInt + 1);
	}

	public void UpdateWeaponDisplay() {
		//AdjustPowerDisp(shootPower);
		//AdjustAngleDisp(localShootAngle);
		//AdjustWeaponDisp();
	}

	public void AdjustPowerDisp(float newPower) {
		shootPower = newPower;
		powerSlider.value = newPower;
	}

    public void SetPowerWithSlider()
    {
        shootPower = powerSlider.value;
        
    }

	public void AdjustAngleDisp(float newAngle) {
		angleText.text = newAngle.ToString ("F1");
	}

    public void ButtonAngleUp()
    {
        localShootAngle += changeAngleSpeed * Time.deltaTime;
        AdjustAngleDisp(localShootAngle);
    }

    public void ButtonAngleDown()
    {
        localShootAngle -= changeAngleSpeed * Time.deltaTime;
        AdjustAngleDisp(localShootAngle);
    }

	public void AdjustWeaponDisp() {

		//Set previous and next weapon strings for GUI display
		WeaponList prevWeapon = currentWeapon - 1;
		WeaponList nextWeapon = currentWeapon + 1;
		prevWeaponStr = prevWeapon.ToString ();
		nextWeaponStr = nextWeapon.ToString ();
		if (prevWeaponStr == "-1") {
			prevWeaponStr = "";
		}
		if (nextWeaponStr == weaponCount.ToString ()) {
			nextWeaponStr = "";
		}

		currentWeaponText.text = currentWeaponStr;
		prevWeaponText.text = prevWeaponStr;
		nextWeaponText.text = nextWeaponStr;
	}

	void MoveBarrel () {
		//Move player's gun barrel to match the selected angle
		gunBarrel.transform.localRotation = Quaternion.Euler(0f,0f,(float)localShootAngle-90f);	//Move reticle to local shoot angle. Zero degrees is to the player right, 90 degrees is player up
        dragButton.ShowAimIndicator((shootPower / maxPower), shootAngle);
    }

	public void SwitchWeapons (int num) {
	//Switch weapons by going directly to the number that is input	
		if (num > 0 && num <= weaponCount) {
			currentWeapon = (WeaponList)(num-1);		//Cast the integer as the enumerator type
			currentWeaponStr = currentWeapon.ToString();
			currentWeaponInt = (int)currentWeapon;
			currentWeaponObj = weaponPrefabs[currentWeaponInt];

            CreateProjectile();
		}
	}

	void SwitchWeapons(string dir) {
		//Switch between weapons by going up or down in the weapons list
		if (dir == "up") {
			currentWeapon+=1;
			
			currentWeapon = (WeaponList)Mathf.Min((int)currentWeapon,weaponCount-1);	//Don't allow a value greater than the indices in the weapon list
		}
		if (dir == "down") {
			currentWeapon-=1;
			currentWeapon = (WeaponList)Mathf.Max((int)currentWeapon,0);				//Don't allow a value less than 0
		}
		currentWeaponStr = currentWeapon.ToString();
		currentWeaponInt = (int)currentWeapon;
		currentWeaponObj = weaponPrefabs[currentWeaponInt];

        CreateProjectile();
	}

	IEnumerator Fire() {
		//Inform phase manager that projectile has been fired
		phaseManager.FiringPhase ();

		//Fire currently selected weapon
		GameObject projectile = (GameObject)Instantiate (currentWeaponObj, muzzlePoint.transform.position, Quaternion.identity);
		Vector2 direction = new Vector2(Mathf.Cos(shootAngle * Mathf.Deg2Rad),Mathf.Sin(shootAngle * Mathf.Deg2Rad)).normalized;
		Vector2 force = direction * shootPower * basePower;
		projectile.rigidbody2D.AddForce(force, ForceMode2D.Impulse);
        
		//Activate object zoom
//		camZoomControl.objectZoomOn = true;
//		camZoomControl.zoomTarget = projectile;

		//Activate camera follow
		camZoomControl.objectFollowOn = true;
		camZoomControl.followTarget = projectile;

		yield return null;
	}

    IEnumerator FireRock()
    {
        //Inform phase manager that projectile has been fired
        phaseManager.FiringPhase();

        guiManager.EnableVelocityToolTip(projectile);
        dragButton.HideGlowRing();

        //Fire currently selected weapon
        projectile.transform.parent = null; //De-parent from player
        projectileRegistry.AddToRegistry(projectile.gameObject);
        projectile.rigidbody2D.isKinematic = false;
        Vector2 direction = new Vector2(Mathf.Cos(shootAngle * Mathf.Deg2Rad), Mathf.Sin(shootAngle * Mathf.Deg2Rad)).normalized;
        Vector2 force = direction * shootPower * basePower;
        projectile.rigidbody2D.AddForce(force, ForceMode2D.Impulse);
        projectile.GetComponent<ActionOnCollision_Rocket>().isCollidable = true;
        projectile.transform.GetComponent<ProjectileDisplayController>().StartTrail();
        projectile.transform.GetComponent<ProjectileController>().controlsActive = true;
                
        //Activate camera follow
        camZoomControl.objectFollowOn = true;
        camZoomControl.followTarget = projectile;

        yield return null;
    }

    void StartMining()
    {
        //Inform phase manager that mining has started
        phaseManager.MiningPhase();

        dragButton.HideGlowRing();

        //Start mining
        projectile.GetComponent<MiningBehavior>().StartCoroutine("StartMining", shootAngle);

    }


	void SwipeControls()
	{
        //If using touchMode SwipePower which uses the swipe distance and direction
        if (touchMode == TouchMode.SwipePower)
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

        //If using touchMode DragPull which uses the drag distance and direction
        if (touchMode == TouchMode.DragPull)
        {
            //After aim button is pressed, look for a drag direction and magnitude
            if (Input.touchCount == 1)
            {
                Touch touch = Input.touches[0];
                Vector2 tp = touch.position;

                if (!startDrag)
                {
                    if ((tp - (Vector2)Camera.main.WorldToScreenPoint((Vector3)this.transform.position)).magnitude <= minTouchDist)
                    //Touch must be within minimum distance to player
                    {
                        startDrag = true;
                        camZoomControl.allowPan = false;  //Disable pan controls while trying to aim and fire
                    }
                }
                else {
                    Vector2 dragVect = tp - (Vector2)Camera.main.WorldToScreenPoint((Vector3)this.transform.position);
                    dragDist = dragVect.magnitude;
                    //dragDist = Mathf.Min(dist, maxDragDist);    //But limit distance to maximum
                    dragDir = dragVect.normalized;

                    if (dragDist >= minDragDist)
                    {
                        shootAngle = Angle360(dragDir);
                        localShootAngle = shootAngle - this.transform.eulerAngles.z;    //Convert global angle to local player angle
                        shootPower = dragDist * swipePowerMult;
                        shootPower = Mathf.Min(shootPower, maxPower);   //Limit to max power
                        AdjustPowerDisp(shootPower);
                        AdjustAngleDisp(localShootAngle);
                        dragButton.ShowAimIndicator((shootPower / maxPower), shootAngle);
                    }
                    switch (touch.phase)
                    {

                        case TouchPhase.Ended:
                            startDrag = false;
                            if (dragDist >= minDragDist)
                            {        //Drag distance must be at least the minimum
                                drag = true;
                                dragButton.HideAimIndicator();
                            }
                            camZoomControl.allowPan = true;  //Re-enable pan controls
                            
                            break;
                    }
                }
            }

            //Same controls as above except for the mouse
            if (Input.GetMouseButtonDown(0))
            {
                Vector2 mp = Input.mousePosition;

                if (!startDrag)
                {
                    if ((mp - (Vector2)Camera.main.WorldToScreenPoint((Vector3)this.transform.position)).magnitude <= minTouchDist)
                    //Touch must be within minimum distance to player
                    {
                        startDrag = true;
                    }
                }
            }
            if (startDrag)
            {
                Vector2 mp = Input.mousePosition;
                Vector2 dragVect = mp - (Vector2)Camera.main.WorldToScreenPoint((Vector3)this.transform.position);
                dragDist = dragVect.magnitude;
                dragDir = dragVect.normalized;

                if (dragDist >= minDragDist)
                {
                    shootAngle = Angle360(dragDir);
                    localShootAngle = shootAngle - this.transform.eulerAngles.z;    //Convert global angle to local player angle
                    shootPower = dragDist * swipePowerMult;
                    shootPower = Mathf.Min(shootPower, maxPower);   //Limit to max power
                    AdjustPowerDisp(shootPower);
                    AdjustAngleDisp(localShootAngle);
                    dragButton.ShowAimIndicator((shootPower / maxPower), shootAngle);
                }

                if (Input.GetMouseButtonUp(0) )
                {
                        startDrag = false;
                        if (dragDist >= minDragDist)
                        {        //Drag distance must be at least the minimum
                            drag = true;
                        }
                        dragButton.HideAimIndicator();
                }
            }
        }
	}

	public static float Angle360(Vector2 p_vector2)
	{
		return (Mathf.Atan2(p_vector2.y, p_vector2.x) * Mathf.Rad2Deg);

	}
	
	
}
