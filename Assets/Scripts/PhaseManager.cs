using UnityEngine;
using System.Collections;

public class PhaseManager : MonoBehaviour {
	
	public enum GamePhase {Start, SpawnPlayers, Aim, Firing, Death, Win, Lose, End};
	public GamePhase gamePhase;
	public enum WhoseTurn {Player1, Player2};
	public WhoseTurn whoseTurn;

	int totalPlayers = 2;
	GameObject[] playerList = new GameObject[2];
	GameObject currentPlayer;
	[HideInInspector] public int currentPlayerInt;
	GameObject otherPlayer;

	GUIManager guiManager;
	SpawnManager spawnManager;
	ScoreManager scoreManager;
	PlayerShootController currentShootController;
	PersistentSettings persistentSettings;
	////CameraShake camShake;
	CameraZoom camZoomControl;
	PlayerRegistry playerRegistry;
	ProjectileRegistry projectileRegistry;
	/////TerrainRegistry terrainRegistry;
    PlayerIndicatorBehavior playerIndicator;

	//Settings variables
	PersistentSettings.GameType gameType;

	// Use this for initialization
	void Start () {
		projectileRegistry = this.GetComponent<ProjectileRegistry> ();
		spawnManager = this.GetComponent<SpawnManager> ();
		scoreManager = this.GetComponent<ScoreManager> ();
		////camShake = Camera.main.gameObject.GetComponent<CameraShake> ();
		camZoomControl = Camera.main.GetComponent<CameraZoom> ();
		/////terrainRegistry = GameObject.Find("GameManager").GetComponent<TerrainRegistry>();
		playerRegistry = GameObject.Find("GameManager").GetComponent<PlayerRegistry>();
		persistentSettings = GameObject.Find("GameSettings").GetComponent<PersistentSettings>();
		guiManager = GameObject.Find("GUICanvas").GetComponent<GUIManager>();
        playerIndicator = GameObject.Find("PlayerIndicator").GetComponent<PlayerIndicatorBehavior>();

		//Determine settings from persistent file
		gameType = persistentSettings.gameType;

		//Initialize state variables
		gamePhase = GamePhase.Start;
		currentPlayerInt = 0;
		whoseTurn = (WhoseTurn)currentPlayerInt;

		StartCoroutine("SpawnPhase");
	}

	IEnumerator SpawnPhase () {
		gamePhase = GamePhase.SpawnPlayers;
		//camZoomControl.StartCoroutine ("MaxZoom");
		//yield return new WaitForSeconds (2.0f);

		int i = 0;
		while (i < totalPlayers) {
			spawnManager.StartCoroutine("SpawnPlayer");

			yield return new WaitForSeconds (2.0f);
			i++;
		}

		ResetPlayerList ();	//Update the player list after all spawn ins
		currentPlayer = playerList [currentPlayerInt];	//Set the current player
		otherPlayer = playerList [1];	//Initialize otherPlayer object as the second player in the list for now
		currentShootController = currentPlayer.gameObject.GetComponent<PlayerShootController> ();
		AimPhase ();

		yield return null;
	}

	public void AimPhase () {
		gamePhase = GamePhase.Aim;


        playerIndicator.SetTargetObject(otherPlayer);

		//Activate object follow on current player
		camZoomControl.objectFollowOn = true;
		camZoomControl.followTarget = currentPlayer;
        camZoomControl.allowPan = true;  //Enable pan controls

		guiManager.EnableInstructionText (); //Enable instruction text during aim phase
		currentShootController.StartCoroutine("Aim");
	}

	public void FiringPhase () {
		gamePhase = GamePhase.Firing;
		guiManager.DisableInstructionText (); //Disable instruction text during firing phase

		//Activate object zoom on other player to keep them in frame while camera is following projectile
		camZoomControl.objectZoomOn = true;
		camZoomControl.zoomTarget = otherPlayer;
        camZoomControl.allowPan = false;  //Disable pan controls while projectile is firing

		projectileRegistry.StartCoroutine("CheckProjectiles");
	}

    public void MiningPhase()
    {
        gamePhase = GamePhase.Firing;
        guiManager.DisableInstructionText(); //Disable instruction text during firing phase

        //Activate object zoom on other player to keep them in frame while camera is following projectile
        camZoomControl.objectZoomOn = true;
        camZoomControl.zoomTarget = currentPlayer;

    }

	public void NoProjectilesLeft () {
		//Debug.Log ("PhaseManager NoProjectilesLeft");
		//camShake.StartCoroutine ("Shake");
		camZoomControl.zoomTarget = null;
		NextPlayer ();
		AimPhase ();
	}

    public void MiningComplete()
    {
        //Debug.Log ("PhaseManager NoProjectilesLeft");
        //camShake.StartCoroutine ("Shake");
        camZoomControl.zoomTarget = null;
        NextPlayer();
        AimPhase();
    }

	public void ResetPlayerList () {
		playerRegistry.playerList.CopyTo(playerList, 0);
	}

	public void NextPlayer () {
		currentPlayerInt++;
		if (currentPlayerInt > totalPlayers - 1) {
			currentPlayerInt = 0;
		}

		currentPlayer = playerList [currentPlayerInt];
		currentShootController = currentPlayer.gameObject.GetComponent<PlayerShootController> ();



		whoseTurn = (WhoseTurn)currentPlayerInt;
		guiManager.RefreshTurnText (whoseTurn.ToString ());

		//Set otherPlayer object
		if (currentPlayerInt == 0) {
			otherPlayer = playerList [1];
		}
		else {
			otherPlayer = playerList [0];
		}

		scoreManager.AddToScore (0f); 		//Update score to current player by adding zero points

	}

	public void SwitchCurrentPlayerWeapon (int num) {
		if (currentShootController != null) {
			currentShootController.SwitchWeapons (num);
		}
	}

    public void FireCurrentPlayer()
    {
        //Used if the standard GUI controls are used
        if (currentShootController != null)
        {
            currentShootController.fireButton = true;
        }
    }

    public void AngleUpCurrentPlayer()
    {
        //Used if the standard GUI controls are used
        if (currentShootController != null)
        {
            currentShootController.ButtonAngleUp();
        }
    }
    public void AngleDownCurrentPlayer()
    {
        //Used if the standard GUI controls are used
        if (currentShootController != null)
        {
            currentShootController.ButtonAngleDown();
        }
    }

    public void SetPowerCurrentPlayer()
    {
        //Used if the standard GUI controls are used
        if (currentShootController != null)
        {
            currentShootController.SetPowerWithSlider();
        }
    }

}
