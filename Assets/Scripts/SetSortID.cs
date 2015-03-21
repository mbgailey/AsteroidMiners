using UnityEngine;
using System.Collections;

public class SetSortID : MonoBehaviour {
    //Sets the sort ID for a renderer
    public int sortID;

	// Use this for initialization
	void Start () {
        this.gameObject.renderer.sortingLayerID = sortID;
	}

}
