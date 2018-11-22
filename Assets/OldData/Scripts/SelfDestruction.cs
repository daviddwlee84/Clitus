using UnityEngine;
using System.Collections;

public class SelfDestruction : MonoBehaviour {

	public int destructionDelay;

	private int counter;

	// Use this for initialization
	void Start () {
		counter = 0;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void FixedUpdate(){
		if (counter++ > destructionDelay) {
			Destroy (gameObject);
		}
	}
}
