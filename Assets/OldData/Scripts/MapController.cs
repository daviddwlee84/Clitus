using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;

public class MapController : MonoBehaviour {

	[Header("Map Settings")]
	[Range(0.0f,0.01f)]
	public float movingSpeed;

	[Range(0.0f,0.001f)]
	public float speedIncrementOverTime = 0.0001f;
	[Range(0.1f,20.0f)]
	public float timeInterval = 10.0f;

	public GameObject map;
	public GameObject player;
	public GameObject playerBody;
	public GameObject initialMap;
	[Range(0.0f,10.0f)]
	public float nextMapGenThreshold = 6.0f;
	[Range(-40.0f,-15.0f)]
	public float mapDestroyThreshold = -25.0f;

	[Tooltip("If player reaches almost at the top of the screen, this function will keep player in screen.")]
	[Range(0.0f,10.0f)]
	public float mapScrollBurstThreshold = 0.0f;
	[Tooltip("The time length for scroll burst")]
	[Range(0.0f,1000.0f)]
	public float mapScrollBurstLength = 240.0f;


	[Header("Music")]
	public AudioClip[] BGM;
	public AudioSource stingSource;

	[Header("Map List")]
	public Object[] easyMap;
	public Object[] midMap;
	public Object[] hardMap;

	private List<GameObject> mapList;

	public int diffculty;

	public const int easy = 0;
	public const int mid = 1;
	public const int hard = 2;

	// Use this for initialization
	void Start () {
		int BGMsel = Random.Range (0, BGM.Length);
		print ("BGM Length: " + BGM.Length.ToString ());
		stingSource.clip = BGM [BGMsel];
		stingSource.Play ();
		mapList = new List<GameObject> ();
		mapList.Add (initialMap);
		StartCoroutine (LevelStartProcess ());
		StartCoroutine (speedTimer ());
		diffculty = easy;
	}

	IEnumerator LevelStartProcess(){
		float fadeTime = GameObject.Find ("Game Manager").GetComponent<Fading> ().BeginFade (Fading.fadeIn);
		yield return new WaitForSeconds (fadeTime);
	}

	IEnumerator speedTimer(){
		while (true) {
			yield return new WaitForSeconds (timeInterval);
			movingSpeed += speedIncrementOverTime;
		}
	}

	// Update is called once per frame
	void Update () {
	
	}

	private void allShift(float shift){
		Vector3 newPos = map.transform.position;
		newPos.y -= shift;
		map.transform.position = newPos;

		if (player != null) {
			newPos = player.transform.position;
			newPos.y -= shift;
			player.transform.position = newPos;
		}
	}

	void FixedUpdate(){
		Vector3 newPos = map.transform.position;
		newPos.y -= movingSpeed;
		map.transform.position = newPos;

		if (player != null) {
			newPos = player.transform.position;
			newPos.y -= movingSpeed;
			player.transform.position = newPos;

		}
		if (playerBody != null) {
			if (playerBody.transform.position.y > mapScrollBurstThreshold) {
				allShift ((playerBody.transform.position.y-mapScrollBurstThreshold) / 240.0f);
			}
		}


		Transform lastMap;
		lastMap = mapList [mapList.Count - 1].transform;

		if (lastMap.position.y < nextMapGenThreshold) {
			Object nextMap = null;
			switch (diffculty) {
			case easy:
				nextMap = easyMap [Random.Range (0, easyMap.Length-1)];
				break;
			case mid:
				nextMap = midMap [Random.Range (0, midMap.Length-1)];
				break;
			case hard:
				nextMap = hardMap [Random.Range (0, hardMap.Length-1)];
				break;
			}
			if (nextMap != null) {
				Transform generatedMap = Instantiate (nextMap, new Vector3 (lastMap.position.x, lastMap.position.y + 60.0f * 0.3f,lastMap.position.z), lastMap.rotation, map.transform) as Transform;
				mapList.Add(generatedMap.gameObject);
				generatedMap.gameObject.transform.localScale = lastMap.localScale;
			}
		}

		foreach (GameObject gameobj in mapList) {
			if (gameobj.transform.position.y < mapDestroyThreshold) {
				mapList.Remove (gameobj);
				Destroy (gameobj);
			}
		}
	}
}
