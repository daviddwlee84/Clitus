using UnityEngine;
using System.Collections;

public class TestCrossScene : MonoBehaviour {

	private float numbers;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void Awake(){
		DontDestroyOnLoad (gameObject);

		numbers = 0.87f;
		PlayerPrefs.SetFloat ("number1", numbers);
	}

	public void LoadLevel(string levelName){
		UnityEngine.SceneManagement.SceneManager.LoadScene (levelName);
	}
	
}
