using UnityEngine;
using System.Collections;

public class rockSplit : rockBase {

	public float releaseDelay = 1;
	public float destroyDelay = 5;
	public Transform destructionFX;

	private GameObject _player;
	private int targetLimb;

	private Transform splitrock1;
	private Transform splitrock2;

	public override void RockStart ()
	{
		base.RockStart ();
		splitrock1 = this.transform.Find ("Cube.001");
		splitrock2 = this.transform.Find ("Cube.002");
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
		splitrock1.gameObject.GetComponent<Rigidbody> ().isKinematic = false;
		splitrock2.gameObject.GetComponent<Rigidbody> ().isKinematic = false;
		yield return new WaitForSeconds (destroyDelay);
		Destroy (gameObject);
	}
}
