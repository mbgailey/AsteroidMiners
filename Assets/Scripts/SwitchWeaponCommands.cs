using UnityEngine;
using System.Collections;

public class SwitchWeaponCommands : MonoBehaviour {

	protected Animator animator;
	PhaseManager phaseManager;
	//public int selectedWeapon = 1;

	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator>();
		phaseManager = GameObject.Find("GameManager").GetComponent<PhaseManager>();
	}
	
	// Update is called once per frame
	public void SelectWeapon (int num) {

		animator.SetInteger ("NewSelect", num);
		phaseManager.SwitchCurrentPlayerWeapon (num);
	}

	public void ResetDisplay (int num) {
	//Use this when switching player shoot controllers. Don't involve the phase manager to avoid circular logic
		animator.SetInteger ("NewSelect", num);

	}
}
