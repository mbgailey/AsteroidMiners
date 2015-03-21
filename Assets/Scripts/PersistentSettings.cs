using UnityEngine;
using System.Collections;

public class PersistentSettings : MonoBehaviour {

	public int numberOfPlayers = 1;
	public enum GameType {Artillery, Mining};
	public GameType gameType = GameType.Artillery;

	public enum LevelType {Asteriod, Planet};
	public LevelType levelType = LevelType.Planet;

    public float minZoom;
    public float maxZoom;
    public float defaultZoom;

    public Vector2 gameBounds;

	// Use this for initialization
	void Awake () {
		DontDestroyOnLoad (this.gameObject);
	}

}
