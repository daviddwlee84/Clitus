using UnityEngine;
using System.Collections;

public class rockBase : MonoBehaviour {

	protected int HPincrement = 5;

	// Use this for initialization
	public virtual void RockStart(){

	}

	void Start () {
		RockStart ();
	}
	
	// Update is called once per frame
	public virtual void RockUpdate(){

	}

	void Update () {
		RockUpdate ();
	}

	public virtual void RockFixedUpdate(){

	}

	void FixedUpdate(){
		RockFixedUpdate ();
	}

	/// <summary>
	/// The handler when step/ grab on this rock.
	/// Override this function if the rock has special action to player.
	/// </summary>
	/// <param name="player">Player.</param>
	public virtual void OnHoldEventHandler(GameObject player, int triggerLimb){
		player.GetComponent<PlayerAction> ().increaseHP (HPincrement);
	}
}
