using UnityEngine;
using System.Collections;

public class SetSortID : MonoBehaviour {
    public int sortID;

	// Use this for initialization
	void Start () {
        this.gameObject.renderer.sortingLayerID = sortID;
	}

}
