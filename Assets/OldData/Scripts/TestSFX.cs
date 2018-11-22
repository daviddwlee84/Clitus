using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;

public class TestSFX : MonoBehaviour {

	public AudioClip testSFX;
	public AudioSource sfxOutput;
	[Range(0,60)]
	public int fireRate;
	[Range(20,1000)]
	public int audioSourceEliminationTime;

	private int counter;
	private int counter_offset;
	private List<AudioSource> audioS;
	private List<int> audioElimTim;

	// Use this for initialization
	void Start () {
		counter = 0;
		audioS = new List<AudioSource> ();
		audioElimTim = new List<int> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void FixedUpdate(){
		if (Input.GetMouseButtonDown (0)) {
			counter_offset = counter % fireRate;
		}
		if (Input.GetMouseButton (0)) {
			if ((counter+counter_offset) % fireRate == 0) {
				AudioSource audiosource = gameObject.AddComponent<AudioSource> ();
				audiosource.clip = testSFX;
				audiosource.loop = false;
				audiosource.volume = 1;
				audiosource.playOnAwake = false;
				audiosource.Play ();
				audioS.Add (audiosource);
				audioElimTim.Add (counter + (int)(testSFX.length * 60.0f));
			}
		}
		counter++;
		if (audioElimTim.Count != 0) {
			for (int i = 0; i < audioElimTim.Count; i++) {
				if (counter > audioElimTim[i]) {
					Destroy (audioS [i]);
					audioS.RemoveAt (i);
					audioElimTim.RemoveAt (i);
					i--;
				}
			}
		}
	}
}
