using UnityEngine;
using System.Collections;

public class rockType1 : rockBase {

	[Range(-100.0f,100.0f)]
	public int healthIncrement;

	public override void OnHoldEventHandler (GameObject player, int triggerLimb)
	{
		HPincrement = healthIncrement;
		base.OnHoldEventHandler (player, triggerLimb);
	}

}
