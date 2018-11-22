using UnityEngine;
using System.Collections;

public class PlayerAction : MonoBehaviour {

	[Header("Map related settings")]
	[Tooltip("Specifies Layer of Rocks")]
	public LayerMask rockLayer = -1;
	[Tooltip("The maximum distance from released limb position to nearest rock")]
	[Range(0.5f,5.0f)]
	public float errorRange;
	[Tooltip("The whole body game object")]
	public GameObject wholeBody;

	[Header("Control related settings")]
	[Tooltip("Specifies Layer of limb control points here")]
	public LayerMask limbsLayer = -1;
	[Tooltip("Control points game object.\nA sphere collider is required in each control points.")]
	public GameObject[] controlPoints;
	[Tooltip("Limbs from human model. It is required each of them MATCHES the control points.")]
	public GameObject[] limbs;
	public GameObject[] warning;
	public GameObject[] error;
	[Tooltip("Feet for calculating new body position, can be either control points or model itself.")]
	public GameObject[] feet;
	[Tooltip("The maximum range between body and limbs.")]
	[Range(1.0f,5.0f)]
	public float rangeLimit;
	public GameObject bodyHeightReference;

	[Header("Sound Settings")]
	public AudioSource sfxOutputDevice;
	public AudioClip sndStep;
	public AudioClip sndGrab;

	[Header("Player Related Settings")]
	[Range(10.0f,200.0f)]
	public int maxHealth;
	[Range(10.0f,200.0f)]
	public int maxPower;

	[Header("UI Related Settings")]
	public GameObject healthBar;
	public int healthBarMaxWidth;
	public UnityEngine.UI.Text healthText;

	[Header("Game System Related")]
	public ScoreSystem scoreSystem;
	public GameObject gameOverMenu;
	[Range(-10.0f,-2.0f)]
	public float gameOverMenuThreshold = -5.0f;
	public GameObject rock2GrabIndicator;

	[Header("Death animation")]
	[Range(-1.0f,5.0f)]
	public float initialDroppingSpeed = 1.0f;
	[Range(-1.0f,0)]
	public float droppingGravity = -0.01f;

	[Header("Developer Settings")]
	public bool MessageOutput;

	private int limbselI;	//index of limbs or -1 means no selection
	private int feetCtlIndexI;
	private Vector3 originalPos;

	private int limbNotHold;

	private int healthi;
	private int poweri;

	private float heightDecented;
	private float droppingSpeed;

	private Vector3 referenceP;

	// Use this for initialization
	void Start () {
		limbselI = -1;
		feetCtlIndexI = 0;
		healthi = maxHealth;
		poweri = maxPower;
		for (int i = 0; i < feet.Length; i++) {
			int index = System.Array.FindIndex<GameObject> (limbs, x => x == feet[i]);
			if (index < 0) {
				index = System.Array.FindIndex<GameObject> (controlPoints, x => x == feet[i]);
			}
			feetCtlIndexI |= 1 << index;
		}
		print ("feetCtlIndexI: " + feetCtlIndexI.ToString ());
		originalPos = new Vector3 ();
		limbNotHold = -1;
		heightDecented = 0;
	}
	
	// Update is called once per frame
	void Update () {
		healthText.text = "HP: " + healthi.ToString("D3") + "/" + maxHealth.ToString("D3");
		RectTransform HPBar = healthBar.GetComponent<RectTransform>();
		HPBar.sizeDelta = new Vector2(((float)healthBarMaxWidth * ((float)healthi / (float)maxHealth)), 10);
		if (MessageOutput) {
			print ("new HealthbarWidth: " + ((float)healthBarMaxWidth * ((float)healthi / (float)maxHealth)).ToString ());
		}
	}

	void FixedUpdate(){
		Vector3 userpos;
		bool pressing;

		referenceP = bodyHeightReference.transform.position;

		if (bodyHeightReference.transform.position.y < gameOverMenuThreshold) {
			healthi = 0;
		}

		if (healthi <= 0) {
			Vector3 newpos;

			newpos = wholeBody.transform.position;
			newpos.y += droppingSpeed;
			wholeBody.transform.position = newpos;

			droppingSpeed += droppingGravity;

			if (newpos.y < gameOverMenuThreshold) {
				// Show Game over menu
				GameObject.Find("Level Manager").GetComponent<LevelManager>().ToggleGameOver();
				Destroy (gameObject);
			}

			return;
		}


		userpos = new Vector3 ();
		if (Input.touchSupported == true) {
			//Touch screen
			if (Input.touchCount > 0) {
				userpos = new Vector3 (Input.GetTouch (0).position.x, Input.GetTouch (0).position.y, 0.0f);
				pressing = true;
			} else {
				pressing = false;
			}
		} else {
			//Mouse
			userpos = Input.mousePosition;
			userpos.z = 0;
			if (Input.GetMouseButton (0)) {
				//Left mouse button down
				pressing = true;
			} else {
				pressing = false;
			}
		}
		if (pressing == true) {
			Ray ray = Camera.main.ScreenPointToRay (userpos);
			RaycastHit rayhit;
			if (Physics.Raycast (ray, out rayhit,Mathf.Infinity, limbsLayer.value)) {
				Vector3 newpos = Camera.main.ScreenToWorldPoint (new Vector3 (userpos.x, userpos.y, 11));
				newpos.z = 0;
				//Finding which limb is selected
				if (limbselI == -1) {
					for (int i = 0; i < 4; i++) {
						if (Vector3.Distance (controlPoints [i].transform.position, rayhit.transform.position) == 0) {
							limbselI = i;
							break;
						}
					}
					if ((limbNotHold != -1) && (limbselI != limbNotHold)) {
						limbselI = -1;
					} else {
						limbNotHold = -1;
					}
					if (limbselI != -1) {
						originalPos = controlPoints [limbselI].transform.position;
					}

				}
				if (limbselI != -1) {
					
					controlPoints [limbselI].transform.position = newpos;
					float dist2Body = Vector3.Distance(new Vector3(wholeBody.transform.position.x,bodyHeightReference.transform.position.y,wholeBody.transform.position.z),newpos);
					if (dist2Body > rangeLimit) {
						warning [limbselI].SetActive (true);
					} else {
						warning [limbselI].SetActive (false);
					}
				}
			}
		} else {	//pressing
			//User is not pressing
			if (limbselI != -1) {
				//User just release control
				Collider[] nearRocks = Physics.OverlapSphere(controlPoints[limbselI].transform.position,errorRange,rockLayer.value);
				if (MessageOutput) {
					print ("You've just released control");
					print ("Previous limbselI: " + limbselI.ToString ());
					print (nearRocks.Length.ToString () + " rocks found nearby");
				}
				//Finding nearest rock
				float mindist = errorRange;
				Vector3 nearestPos = originalPos;
				int nearestIndex = -1;
				for (int i = 0; i < nearRocks.Length; i++) {
					float dist = Vector3.Distance (controlPoints [limbselI].transform.position, nearRocks[i].transform.position);
					float dist2Body = Vector3.Distance(new Vector3(wholeBody.transform.position.x,referenceP.y,wholeBody.transform.position.z),nearRocks[i].transform.position);
					if (dist < mindist && dist2Body < rangeLimit) {
						mindist = dist;
						nearestPos = new Vector3 (nearRocks[i].transform.position.x + nearRocks[i].transform.localScale.x * 0f, nearRocks[i].transform.position.y + nearRocks[i].transform.localScale.y * 0, 0.0f);
						nearestIndex = i;
					}
				}
				controlPoints [limbselI].transform.position = (nearestIndex<0?originalPos:nearestPos);
				warning [limbselI].SetActive (false);
				// Triggers OnHold Handler
				if (nearestIndex >= 0) {
					rockBase rock = nearRocks [nearestIndex].gameObject.GetComponent<rockBase> ();
					if (rock != null) {
						rock.OnHoldEventHandler (gameObject,limbselI);
					}
				}

				//Playing Sound
				if ((feetCtlIndexI & (1 << limbselI)) > 0) {
					//Foot steps on rock
					SFXSystem.PlaySFX (sndStep,sfxOutputDevice.outputAudioMixerGroup);
				} else {
					//Hand grabs the rock
					SFXSystem.PlaySFX (sndGrab,sfxOutputDevice.outputAudioMixerGroup);
				}

				limbselI = -1;
			}	//limbselI == -1

		}	//pressing
		Vector3 bodyNewPos = new Vector3 ();
		for (int i = 0; i < controlPoints.Length; i++) {
			if (limbNotHold != i) {
				if (Vector3.Distance (controlPoints [i].transform.position, referenceP) > rangeLimit) {
					limbs [i].transform.position = controlPoints [i].transform.position * (rangeLimit/Vector3.Distance (controlPoints [i].transform.position, referenceP));
				} else {	
					limbs [i].transform.position = controlPoints [i].transform.position;
				}
				error [i].SetActive (false);
			} else {
				error [i].SetActive (true);
				controlPoints [i].transform.position = limbs [i].transform.position;
			}
		}
		foreach (GameObject obj in feet) {
			bodyNewPos.x += obj.transform.position.x / 2;
			bodyNewPos.z += obj.transform.position.z / 2;
		}
		bodyNewPos.y = (feet [0].transform.position.y < feet [1].transform.position.y ? feet [0].transform.position.y : feet [1].transform.position.y);

		#region Score Calculation
		float heightDiff = bodyNewPos.y - wholeBody.transform.position.y;
		if (heightDiff > 0) {
			if (heightDiff + heightDecented > 0) {
				scoreSystem.addScore ((heightDiff + heightDecented)*100);
				heightDecented = 0;
			} else {
				heightDecented = heightDiff + heightDecented;
			}
		} else {
			heightDecented += heightDiff;
		}
		#endregion


		wholeBody.transform.position = bodyNewPos;

		if (pressing) {
			Collider[] nearRocks = Physics.OverlapSphere (controlPoints [limbselI].transform.position, errorRange, rockLayer.value);
			float mindist = errorRange;
			int nearestIndex = -1;
			for (int i = 0; i < nearRocks.Length; i++) {
				float dist = Vector3.Distance (controlPoints [limbselI].transform.position, nearRocks [i].transform.position);
				float dist2Body = Vector3.Distance (new Vector3 (wholeBody.transform.position.x, referenceP.y, wholeBody.transform.position.z), nearRocks [i].transform.position);
				if (dist < mindist && dist2Body < rangeLimit) {
					mindist = dist;
					nearestIndex = i;
				}
			}
			if (nearestIndex >= 0) {
				rock2GrabIndicator.SetActive (true);
				rock2GrabIndicator.transform.position = new Vector3 (nearRocks [nearestIndex].transform.position.x, nearRocks [nearestIndex].transform.position.y, 0.0f);
			} else {
				rock2GrabIndicator.SetActive (false);
			}
		} else {
			rock2GrabIndicator.SetActive (false);
		}
	}	//FixedUpdate

	public void increaseHP(int increment){
		healthi += increment;
		if (healthi > maxHealth) {
			healthi = maxHealth;
		}
		if (healthi <= 0) {

		}
	}

	public void increasePP(int increment){
		poweri += increment;
		if (poweri > maxPower) {
			poweri = maxPower;
		}
	}

	public void releaseLimb(int limbID){
		if (limbNotHold < 0) {
			limbNotHold = limbID;
		} else {
			healthi = 0;
		}
	}
}


// TODO: Fix body falling apart.
// TODO: (Optional) Create an animation that hand moves to the rock instead of TELEPORTING.