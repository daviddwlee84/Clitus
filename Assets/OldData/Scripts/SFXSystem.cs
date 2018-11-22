using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;

public class SFXSystem : MonoBehaviour {


	private static int counter;
	private static List<AudioSource> audioS;
	private static List<int> audioElimTim;

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

	public static void PlaySFX(AudioClip sound,AudioMixerGroup audioMixerGroup,bool loop = false, bool playOnAwake = false,float volume = 1.0f){
		GameObject SFXSystemInstance;

		SFXSystemInstance = GameObject.Find ("SFXSystem");

		AudioSource auds = SFXSystemInstance.AddComponent<AudioSource> ();
		auds.clip = sound;
		auds.loop = loop;
		auds.playOnAwake = playOnAwake;
		auds.volume = volume;
		auds.outputAudioMixerGroup = audioMixerGroup;

		auds.Play ();

		// Calculating delete time
		int elimTime = counter + (int)(sound.length * 60.0f);

		// Push into Audio Source List
		audioS.Add(auds);
		audioElimTim.Add (elimTime);
	}
}
