using UnityEngine;
using System.Collections;

public class LoadMenuCommands : MonoBehaviour {

	PersistentSettings persistentSettings;

	// Use this for initialization
	void Start () {
		persistentSettings = GameObject.Find("GameSettings").GetComponent<PersistentSettings>();
	}
	
	// Update is called once per frame
	public void LoadLevel (int lvl) {
		if (lvl == 1) {
			persistentSettings.levelType = PersistentSettings.LevelType.Asteriod;
		}

		else if (lvl == 2) {
			persistentSettings.levelType = PersistentSettings.LevelType.Planet;
		}

		Application.LoadLevel (lvl);
	}
}
