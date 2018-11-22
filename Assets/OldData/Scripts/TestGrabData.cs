using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TestGrabData : MonoBehaviour {

	public Slider slider;

	// Use this for initialization
	void Start () {
		if (PlayerPrefs.HasKey ("number1")) {
			float number = PlayerPrefs.GetFloat ("number1");

			print ("[TestGrabData] number: " + number.ToString());

			slider.value = number;

			print ("[TestGrabData] slider.value: " + slider.value.ToString ());
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
