using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Audio;
using System.Collections.Generic; // for List

public class MainMenu : MonoBehaviour {

	[Header("Volume")]
	public AudioMixer GameAudio;
	public Slider BGMSlider;
	public Slider SFXSlider;
	public Text BGMText;
	public Text SFXText;
	public Toggle volumeMute;
	public AudioSource BGMAudio; // Background volume

	private float BGMvolumetemp;
	private float SFXvolumetemp;

	[Header("High Score")]
	public Text highscoreText;
	public GameObject resetMenu;

	private Transform cameraTransform;
	private Transform cameraDesiredLookAt;

	[Header("Scene Loading")]
	public bool testAsyncLoading;
	public string loadingScene;

	private GameManager gm;

	private bool showPopUp = false;
	public List<int> scoreArray;



	private const float CAMERA_TRANSITION_SPEED = 5.0f;

	private void Start()
	{
		cameraTransform = Camera.main.transform;

		resetMenu.SetActive (false);

		BGMSlider.maxValue = 1;
		BGMSlider.minValue = 0;

		SFXSlider.maxValue = 1;
		SFXSlider.minValue = 0;

		float initBGMvolume;
		float initSFXvolume;

		GameAudio.GetFloat ("volume_BGM", out initBGMvolume);
		BGMSlider.value = Mathf.Pow (10.0f, initBGMvolume / 20.0f);
		GameAudio.GetFloat ("volume_SFX", out initSFXvolume);
		SFXSlider.value = Mathf.Pow (10.0f, initSFXvolume / 20.0f);

		UpdateVolumeTemp ();
		VolumeController ();

		BGMAudio.Play ();

        GameObject gm_obj = GameObject.Find("Game Manager");
        if (gm_obj != null)
        {
            gm = gm_obj.GetComponent<GameManager>();
            StartCoroutine(FadeIn());
        }

	}

    IEnumerator FadeIn(){
        yield return new WaitForSeconds(gm.gameObject.GetComponent<Fading>().BeginFade(Fading.fadeIn));
    }

	private void Update()
	{
		if (cameraDesiredLookAt != null) { //Moving Camera
			cameraTransform.rotation = Quaternion.Slerp 
				(cameraTransform.rotation, cameraDesiredLookAt.rotation, CAMERA_TRANSITION_SPEED * Time.deltaTime);
		}
	}

	private bool findGM(){
		if (gm == null) {
			GameObject gm_obj = GameObject.Find ("Game Manager");
			if (gm_obj != null) {
				gm = gm_obj.GetComponent<GameManager> ();
				return true;
			}
			return false;
		} else {
			return true;
		}
	}

	//For play button to load level
	public void LoadLevel(string _sceneName)
	{
		if (findGM ()) {
			VolumeController ();
			gm.SwitchScene (_sceneName);
		}
	}

	public void LookAtMenu(Transform menuTransform)
	{
		ReflashScoreBoard ();
		//Camera.main.transform.LookAt (menuTransform.position); //Camera LookAt immediately
		cameraDesiredLookAt = menuTransform; //Enable Camera LookAt Smoothly
	}

	public void Quit()
	{
		Application.Quit ();
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

		if (findGM ()) {
			gm.setBGMVolume (BGMSlider.value);
			gm.setSFXVolume (SFXSlider.value);
		}
	}

	public void UpdateVolumeTemp()
	{
		BGMvolumetemp = BGMSlider.value;
		PlayerPrefs.SetFloat ("BGMvolumetemp", BGMvolumetemp);
		Debug.Log ("[MainMenu] BGMvolumetemp has been set to " + BGMvolumetemp.ToString ());
		SFXvolumetemp = SFXSlider.value;
		PlayerPrefs.SetFloat ("SFXvolumetemp", SFXvolumetemp);
		Debug.Log ("[MainMenu] SFXvolumetemp has been set to " + SFXvolumetemp.ToString ());
	}

	public void VolumeMute(){
		if (volumeMute.isOn) {
			UpdateVolumeTemp ();
			BGMSlider.value = 0;
			SFXSlider.value = 0;
			VolumeController ();
		} else {
			BGMSlider.value = BGMvolumetemp;
			SFXSlider.value = SFXvolumetemp;
			VolumeController ();
		}
	}

	public void ReflashScoreBoard()
	{
        if (gm == null)
        {
            GameObject obj_gm = GameObject.Find("Game Manager");
            gm = obj_gm.GetComponent<GameManager>();
        }

		scoreArray = gm.GetScoreList ();

		// Update Score Board
		highscoreText.text = "";
		for (int i = 0; i < scoreArray.Count; i++) {
			highscoreText.text = highscoreText.text + (i+1).ToString("D2") + ": " + scoreArray [i].ToString () + "\n";
		}
	}

	// Reset High Score Data
	public void ShowResetWindow()
	{
		resetMenu.SetActive(true);
	}

	public void ResetScore(bool yes)
	{
		if (yes) {
			GameObject obj_gm = GameObject.Find ("Game Manager");
			gm = obj_gm.GetComponent<GameManager> ();
			PlayerPrefs.DeleteAll ();
			PlayerPrefs.SetInt ("ScoreBoardCount", 0);
			ReflashScoreBoard ();
		}
		resetMenu.SetActive (false);
	}
			


//	void OnGUI()
//	{
//		if (showPopUp)
//		{
//			GUI.Window(0, new Rect((Screen.width/2)-150, (Screen.height/2)-75
//				, 300, 250), ShowGUI, "Delete Data?");
//
//		}
//	}
//
//	public void PopWindow()
//	{
//		showPopUp = true;
//	}
//
//	void ShowGUI(int windowID)
//	{
//
//		GUI.Label(new Rect(65, 40, 200, 30), "Are you sure you want to delete all record?");
//
//		if (GUI.Button(new Rect(50, 150, 75, 30), "OK")) {
//
//			GameObject obj_gm = GameObject.Find ("Game Manager");
//			gm = obj_gm.GetComponent<GameManager>();
//
//			showPopUp = false;
//			PlayerPrefs.DeleteAll();
//			PlayerPrefs.SetInt ("ScoreBoardCount", 0);
//			ReflashScoreBoard ();
//		}
//		if (GUI.Button (new Rect (130, 150, 75, 30), "Cancel")) {
//			showPopUp = false;
//		}
//
//	}
}
