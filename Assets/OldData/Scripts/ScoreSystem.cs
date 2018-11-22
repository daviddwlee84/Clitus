using UnityEngine;
using System.Collections;

public class ScoreSystem : MonoBehaviour {

	private float _score;
	public float score{ get { return _score; } }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void reset(){
		_score = 0;
	}

	public void addScore(float increment){
		_score += increment;
	}
}
