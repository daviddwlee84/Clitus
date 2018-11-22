using UnityEngine;
using System.Collections;
using System.Collections.Generic; // for List
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	private static GameManager instance; //can be use anywhere in the game
	public static GameManager Instance{ get { return instance; } }

	private float BGMvolumetemp;
	private float SFXvolumetemp;

	private int scoreCnt;
	public List<int> scoreArray;

    public string loadingScene;

	[HideInInspector]
	public string sceneName{ get; set; }

	private void Awake () {

		instance = this;

		DontDestroyOnLoad (gameObject);

		if(PlayerPrefs.HasKey("ScoreBoardCount"))
		{ // We had a previous session, load data
			ReloadScore ();
		}
		else
		{ // Initialize score data
			PlayerPrefs.SetInt ("ScoreBoardCount", 0);
		}
	}

	private void ReloadScore()
	{
		scoreCnt = PlayerPrefs.GetInt("ScoreBoardCount");
		//Debug.Log ("load: " + scoreCnt);
		scoreArray = new List<int>();
		for(int i = 0; i < scoreCnt; i++)
			scoreArray.Add(PlayerPrefs.GetInt("ScoreBoardList_" + i.ToString()));
	}

	public List<int> GetScoreList(){
		ReloadScore ();
		return scoreArray;
	}
	 
	public void SaveNewScore(int score)
	{
		Debug.Log ("Fucking Saving");
		scoreArray.Add (score);
		scoreArray.Sort((x, y) => { return -x.CompareTo(y); });
		foreach (int a in scoreArray) {
			Debug.Log ("a :" + a);
		}
		UpdateScore ();
	}

	private void UpdateScore()
	{
		scoreCnt = scoreArray.Count;
		PlayerPrefs.DeleteAll();
		PlayerPrefs.SetInt ("ScoreBoardCount", scoreCnt);
		for(int i = 0; i < scoreCnt; i++)
			PlayerPrefs.SetInt("ScoreBoardList_" + i.ToString(), scoreArray[i]);
	}
		




	private void Start(){
		
	}

	public void StartAlternative(){
		// initial after start
	}


	public void setBGMVolume(float vol){
		BGMvolumetemp = vol;
	}
	public void setSFXVolume(float vol){
		SFXvolumetemp = vol;
	}


	public float getBGMVolume() {
		return BGMvolumetemp;
	}
	public float getSFXVolume() {
		return SFXvolumetemp;
	}

    IEnumerator _changeSceneProcess()
    {
        float fadingTime = gameObject.GetComponent<Fading>().BeginFade(Fading.fadeOut);
        yield return new WaitForSeconds(fadingTime);

        UnityEngine.SceneManagement.SceneManager.LoadScene(loadingScene);
    }

    public void SwitchScene(string targetScene)
    {
        sceneName = targetScene;
        StartCoroutine(_changeSceneProcess());
    }

}
