using UnityEngine;
using System.Collections;

public class rockType2 : rockBase {

	public float releaseDelay;
	public Transform destructionFX;

	private GameObject _player;
	private int targetLimb;

	public override void RockStart ()
	{
		base.RockStart ();
	}

	public override void OnHoldEventHandler (GameObject player, int triggerLimb)
	{
		_player = player;
		targetLimb = triggerLimb;
		StartCoroutine (Destruct ());
		base.OnHoldEventHandler (player, triggerLimb);
	}

	IEnumerator Destruct(){
		yield return new WaitForSeconds (releaseDelay);
		_player.GetComponent<PlayerAction> ().releaseLimb (targetLimb);
		if (destructionFX != null) {
			Object fx = Instantiate (destructionFX, gameObject.transform.position, gameObject.transform.rotation);
			fx.name = "Broken Rock";
		}
		Destroy (gameObject);
	}
}
