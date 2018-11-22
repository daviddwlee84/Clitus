using UnityEngine;
using System.Collections;

public class rockSpike : rockBase {

	[Range(-100.0f,100.0f)]
	public int healthIncrement;

	public override void OnHoldEventHandler (GameObject player, int triggerLimb)
	{
		player.GetComponent<PlayerAction> ().increaseHP (healthIncrement);
		//player.GetComponent<PlayerAction> ().increaseHP (healthIncrement);
		base.OnHoldEventHandler (player, triggerLimb);
	}

}
