using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System.Collections.Generic;

public class GameManagerInit : MonoBehaviour {

	// Adjust volume
	public Transform GM;

	public List<int> scoreArray;

//	public Text highscoreText;

	[HideInInspector]
	public bool isLoading;

	public string loadingScene;

	// Use this for initialization
	void Start () {
		isLoading = true;
		GameObject gm_obj = GameObject.Find ("Game Manager");
		GameManager gm;
		if (gm_obj == null) {

			Transform gm_t = Instantiate (GM) as Transform;
			gm_t.gameObject.name = "Game Manager";
			gm_obj = GameObject.Find ("Game Manager");

			gm = gm_obj.GetComponent<GameManager> ();
		} else {
			gm = gm_obj.GetComponent<GameManager> ();
		}
		if (gm_obj != null) {
			gm.scoreArray = scoreArray;
			gm.loadingScene = loadingScene;
//			gm.highscoreText = highscoreText;

			isLoading = false;
			gm.StartAlternative ();
		} else {
			print ("[GameManagerInit]: Failed to initialize GameManager");
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
