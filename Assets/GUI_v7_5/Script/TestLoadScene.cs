using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TestLoadScene : MonoBehaviour {

	public Image loadingImage;
	public Text loadingText;

	[Range(0.0f,10.0f)]
	public float loadingImageRotationSpeed;

	private GameManager gm;

	// Use this for initialization
	void Start () {
		GameObject obj_gm = GameObject.Find ("Game Manager");
		if (obj_gm != null) {
			gm = obj_gm.GetComponent<GameManager>();
			print ("[TestLoadScene]: gm found!");
			print ("[TestLoadScene]: gm.sceneName = \"" + gm.sceneName + "\"");
			StartCoroutine (LoadSceneAsync ());
		}
	}

	IEnumerator LoadSceneAsync(){
		AsyncOperation async;
		float fadingTime = gm.gameObject.GetComponent<Fading> ().BeginFade (Fading.fadeIn);
		yield return new WaitForSeconds (fadingTime);
		async = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync (gm.sceneName);
		async.allowSceneActivation = false;
		yield return null;
		while (async.progress < 0.9f) {
			yield return null;
		}
		fadingTime = gm.gameObject.GetComponent<Fading> ().BeginFade (Fading.fadeOut);
		yield return new WaitForSeconds (fadingTime);
		async.allowSceneActivation = true;
	}

	// Update is called once per frame
	void Update () {
		
	}

	void FixedUpdate(){
		loadingImage.rectTransform.rotation = Quaternion.Euler(0,0,loadingImage.rectTransform.rotation.eulerAngles.z + loadingImageRotationSpeed);
	}
}
