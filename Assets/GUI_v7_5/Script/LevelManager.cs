using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System;
using UnityEngine.Audio;

public class LevelManager : MonoBehaviour {

	[Header("Pause Menu")]
	public GameObject pauseMenu;
	private GameManager gm;

	[Header("Volume")]
	public AudioMixer GameAudio;
	public AudioSource BGMAudio;
	public Slider BGMSlider;
	public Slider SFXSlider;
	public Text BGMText;
	public Text SFXText;
	public Toggle volumeMute;

//	private float BGMvolumetemp;
//	private float SFXvolumetemp;
	private float BGMmutetemp;
	private float SFXmutetemp;

	[Header("GameOver")]
	public GameObject gameOver;
	public Image GameOverImg;
	public Text GameOverText;
	private float FADE_DURATION = 0.25f;

	[Header("Score")]
	public Text scoreText;
	public ScoreSystem scoreSystem;

	public static int newScore; //save new score when gameover

	private void Start()
	{
		pauseMenu.SetActive (false);
		gameOver.SetActive (false);

		GameObject obj_gm = GameObject.Find ("Game Manager");
		gm = obj_gm.GetComponent<GameManager>();

		// Volume initialize

		float BGMtemp; // to prevent some stupid issue ..........
		float SFXtemp;

		BGMtemp = gm.getBGMVolume ();
		SFXtemp = gm.getSFXVolume ();

		BGMSlider.maxValue = 1;
		BGMSlider.minValue = 0;

		SFXSlider.maxValue = 1;
		SFXSlider.minValue = 0;

		BGMSlider.value = BGMtemp;
		SFXSlider.value = SFXtemp;

		Debug.Log ("SFX Slider :" + SFXSlider.value);

		BGMText.text = "BGM Volume : " + (int)(BGMSlider.value * 100.0F) + "%";
		SFXText.text = "SFX Volume : " + (int)(SFXSlider.value * 100.0F) + "%";


		VolumeController ();

		BGMAudio.Play ();

		//Fade In
		StartCoroutine(fadeIn());
	}

	IEnumerator fadeIn(){
		yield return new WaitForSeconds (gm.gameObject.GetComponent<Fading> ().BeginFade (Fading.fadeIn));
	}

	void Update(){
		scoreText.text = "Score: " + Convert.ToInt32 (scoreSystem.score).ToString ();
	}

	public void TogglePauseMenu()
	{
		pauseMenu.SetActive (!pauseMenu.activeSelf); //set inverse value
		if (pauseMenu.activeSelf) { //pause game
			Time.timeScale = 0F;
		} else {
			Time.timeScale = 1.0F;
		}
	}


		
	public void ToMenu()
	{
		Time.timeScale = 1.0F;
		gm.SwitchScene ("MainMenu");
	}

	/* Volume Management */
	public void VolumeController(){

		float outputdB; //temp value
		if (BGMSlider.value == 0)
			outputdB = -80.0f; //prevent log0
		else
			outputdB = 20.0f * Mathf.Log10 (BGMSlider.value);

		GameAudio.SetFloat ("volume_BGM", outputdB);

		if (SFXSlider.value == 0)
			outputdB = -80.0f; //prevent log0
		else
			outputdB = 20.0f * Mathf.Log10 (SFXSlider.value);

		GameAudio.SetFloat ("volume_SFX", outputdB);

		BGMText.text = "BGM Volume : " + (int)(BGMSlider.value * 100.0f) + "%";
		SFXText.text = "SFX Volume : " + (int)(SFXSlider.value * 100.0f) + "%";
	}
		

	public void VolumeMute(){
		if (volumeMute.isOn) {
			BGMmutetemp = BGMSlider.value;
			SFXmutetemp = SFXSlider.value;
			BGMSlider.value = 0;
			SFXSlider.value = 0;
			VolumeController ();
		} else {
			BGMSlider.value = BGMmutetemp;
			SFXSlider.value = SFXmutetemp;
			VolumeController ();
		}
	}



	//Handled by GameManager
	public void StoreNewScore(){
		Text inputField = GameObject.Find("Canvas/ScoreInputField/ScoreInputFieldText").GetComponent<Text>();
		//GameManager.SaveNewScore (Convert.ToInt32(inputField.text));
		//gm.GetComponent<GameManager>().SaveNewScore(Convert.ToInt32(inputField.text));

		//test Game Over score
		newScore = Convert.ToInt32(inputField.text);
	}

	//GameOver
	public void ToggleGameOver ()
	{
		gameOver.SetActive (true);
		updateGameOverScore(Convert.ToInt32 (scoreSystem.score));
		FadeOutGameOver ();
	}

	private void updateGameOverScore(int score){
		GameOverText.text = score.ToString ();
	}

	public void RestartGame()
	{
		print ("[LevelManager]: Saving new score: " + scoreSystem.score.ToString());
		gm.SaveNewScore (Convert.ToInt32 (scoreSystem.score));
		gm.SwitchScene (SceneManager.GetActiveScene ().name);
	}

	public void EndGame()
	{
		ToggleGameOver ();
		FadeInGameOver ();
		gm.SaveNewScore (Convert.ToInt32 (scoreSystem.score));
		ToMenu ();
	}

	private void FadeInGameOver() {
		GameOverImg.CrossFadeAlpha(0, FADE_DURATION, true);
	}

	private void FadeOutGameOver() {
		GameOverImg.CrossFadeAlpha(1, FADE_DURATION, true);
	}
		
}
