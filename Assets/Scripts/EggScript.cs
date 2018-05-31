using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Arduino;


public class EggScript : MonoBehaviour {

	// JUST FOR BETA
	Sprite sprite;
	//ParticlesAttractorScript particlesEmitter;
	float worldScreenHeight;
	float worldScreenWidth;

	public MonsterQueueManagerScript monsterQueueManager;
	public BuioAnimationScr buio;

	//AUDIO
	public AudioSource audioSource;
	public AudioClip shakeAudio;
	public AudioClip accXAudio;
	public AudioClip accYAudio;
	public AudioClip bendAudio;
	public AudioClip turnAudio;

	//TASK VARIABLES
	public string task = "";
	bool mustBendRight = true;
	bool mustAccRight = true;
	bool mustAccTop = true;
	int ruotaLevel = 0;
	string ruotaDir = "Dx";

	//To avoid one to play before the animations end
	public bool waitBeforeContinue = false;

	//Gyroscope variables
	//float basex_angle, basey_angle, basez_angle, basex_acc, basey_acc, basez_acc;
	//bool calibrate = true;

	int arduinoBufferSize=5;
	float[] x_angle, y_angle, z_angle, x_acc, y_acc, z_acc;
	int arduinoCounter=0;

	//Arduino Led Bar
	ArduinoManager am;
	bool ok = false;

	//Task oriented variables
	int MAX_INTENSITY = 4;
	int MAX_BRIGHTNESS = 200;

	int intensity = 1;
	int brightness = 20;
	string color = "ffffff";

	void Start () {

		sprite = gameObject.GetComponent<SpriteRenderer> ().sprite;
		//particlesEmitter = gameObject.GetComponent<ParticlesAttractorScript> ();
		worldScreenHeight = Camera.main.orthographicSize * 2f;
		worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;

		am = new ArduinoManager ("\\\\.\\COM27");
		ok = true;

		x_angle = new float[arduinoBufferSize];
		y_angle = new float[arduinoBufferSize];
		z_angle = new float[arduinoBufferSize];
		x_acc = new float[arduinoBufferSize];
		y_acc = new float[arduinoBufferSize];
		z_acc = new float[arduinoBufferSize];

		am.StartBar ();
		LightUpLedBar ();

		transform.localScale = new Vector3 (100, 100, 1);

	}

	void Update () {

		ReadFromArduino ();
		//UpdateEggLight ();


		/*JUST FOR TESTING
		if (Input.GetKeyDown (KeyCode.A)) {
			ChangeLightColor ((float)Random.Range (0, 256) / 255, (float)Random.Range (0, 256) / 255, (float)Random.Range (0, 256) /255);
		}
		if (Input.GetKeyDown (KeyCode.S)) {
			ChangeLightColor (1,1,1);
		}*/
		if (Input.GetKeyDown (KeyCode.Z)) {
			FillCompletionBar ();
		}

		if (Input.GetKeyDown (KeyCode.Q)) {
			am.SwitchOff ();
		}

		if (Input.GetMouseButtonDown (0)) {
			Vector2 spritePosition = GetSpritePositionOnScreen ();
			foreach (Touch touch in Input.touches) {
				if (touch.position.x > spritePosition.x + 5 * sprite.bounds.min.x &&
				    touch.position.x < spritePosition.x + 5 * sprite.bounds.max.x &&
				    touch.position.y > spritePosition.y + 5 * sprite.bounds.min.y &&
				    touch.position.y < spritePosition.y + 5 * sprite.bounds.max.y)
					FillCompletionBar ();
			}

			buio.PlayEyesAnimation ("occhiolino");
		}
	}

	public Vector2 GetSpritePositionOnScreen() {
		Vector2 position;
		position.x = (transform.position.x + worldScreenWidth/2) / worldScreenWidth * Screen.width;
		position.y = (transform.position.y + worldScreenHeight/2) / worldScreenHeight * Screen.height;
		return position;
	}

	private void ReadFromArduino() {
		if (ok) {
			string response = am.ReadFromArduino ();
			if (response != null) {
				int type = 0;
				try {type = int.Parse (response.Split ("," [0]) [0]);} catch {Debug.Log(response);}
				switch(type) {
				case (12):
					float.TryParse (response.Split ("," [0]) [1], out x_angle [arduinoCounter]);
					float.TryParse (response.Split ("," [0]) [2], out y_angle [arduinoCounter]);
					float.TryParse (response.Split ("," [0]) [3], out z_angle [arduinoCounter]);
					float.TryParse (response.Split ("," [0]) [4], out x_acc [arduinoCounter]);
					float.TryParse (response.Split ("," [0]) [5], out y_acc [arduinoCounter]);
					float.TryParse (response.Split ("," [0]) [6], out z_acc [arduinoCounter]);
					NormalizeAcceleration ();
					//Debug.Log (response);
					//Debug.Log ("X: "+x_angle[arduinoCounter]+"\tY: "+y_angle[arduinoCounter]+"\tZ: "+z_angle[arduinoCounter]+"\txAcc: "+x_acc[arduinoCounter]+"\tyAcc: "+y_acc[arduinoCounter]+"\tzAcc: "+z_acc[arduinoCounter]);
					break;
				default:
					//Debug.Log (response);
					break;
				}
			}
		}
	}

	private void NormalizeAcceleration() {
		x_acc[arduinoCounter] = (x_acc[arduinoCounter] ) / 16348;
		y_acc[arduinoCounter] = (y_acc[arduinoCounter] ) / 16348;
		z_acc[arduinoCounter] = (z_acc[arduinoCounter] ) / 16348;

		arduinoCounter++;
		if (arduinoCounter > arduinoBufferSize - 1) {
			arduinoCounter = 0;
			CheckActionTaken ();
		}
	}

	private void CheckActionTaken() {

		//CHECK RIGHT ACCELERATION (X ACC POSITIVE)
		float accSensitivity = 0.1f;
		bool incrementing = true;
		for (int i = 0; i < arduinoBufferSize; i++) {
			if (x_acc [i] < accSensitivity) {
				incrementing = false;
				break;
			}
		}
		if (incrementing)
			AccRight ();

		//CHECK LEFT ACCELERATION (X ACC NEGATIVE)
		accSensitivity = 0.1f;
		incrementing = true;
		for (int i = 0; i < arduinoBufferSize; i++) {
			if (x_acc [i] > -accSensitivity) {
				incrementing = false;
				break;
			}
		}
		if (incrementing)
			AccLeft ();

		//CHECK UP ACCELERATION (Y ACC POSITIVE)
		accSensitivity = 0.1f;
		incrementing = true;
		for (int i = 0; i < arduinoBufferSize; i++) {
			if (y_acc [i] < accSensitivity) {
				incrementing = false;
				break;
			}
		}
		if (incrementing)
			AccUp ();

		//CHECK DOWN ACCELERATION (Y ACC NEGATIVE)
		accSensitivity = 0.1f;
		incrementing = true;
		for (int i = 0; i < arduinoBufferSize; i++) {
			if (y_acc [i] > -accSensitivity) {
				incrementing = false;
				break;
			}
		}
		if (incrementing)
			AccDown ();

		//CHECK TOP ACCELERATION (X ACC POSITIVE)
		accSensitivity = 0.1f + 0.95f; // 0.95f is the default gravity force on the Z axis
		incrementing = true;
		for (int i = 0; i < arduinoBufferSize; i++) {
			if (z_acc [i] < accSensitivity) {
				incrementing = false;
				break;
			}
		}
		if (incrementing)
			AccTop ();

		//CHECK BOTTOM ACCELERATION (X ACC NEGATIVE)
		accSensitivity = -0.1f + 0.95f;
		incrementing = true;
		for (int i = 0; i < arduinoBufferSize; i++) {
			if (z_acc [i] > accSensitivity) {
				incrementing = false;
				break;
			}
		}
		if (incrementing)
			AccBottom ();
		
		//CHECK RIGHT BENDING (X ANGLE INCREASES)
		float angleSensitivity = 1.0f;
		incrementing = true;
		for (int i = 1; i < arduinoBufferSize; i++) {
			if (x_angle [i] < x_angle [i - 1] + angleSensitivity) {
				incrementing = false;
				break;
			}
		}
		if (incrementing)
			BendRight ();

		//CHECK LEFT BENDING (X ANGLE DECREASES)
		angleSensitivity = 1.0f;
		bool decrementing = true;
		for (int i = 1; i < arduinoBufferSize; i++) {
			if (x_angle [i] > x_angle [i - 1] - angleSensitivity) {
				decrementing = false;
				break;
			}
		}
		if (decrementing)
			BendLeft ();

		//CHECK DOWN BENDING (Y ANGLE INCREASES)
		angleSensitivity = 1.0f;
		incrementing = true;
		for (int i = 1; i < arduinoBufferSize; i++) {
			if (y_angle [i] < y_angle [i - 1] + angleSensitivity) {
				incrementing = false;
				break;
			}
		}
		if (incrementing)
			BendDown ();

		//CHECK UP BENDING (Y ANGLE DECREASES)
		angleSensitivity = 1.0f;
		decrementing = true;
		for (int i = 1; i < arduinoBufferSize; i++) {
			if (y_angle [i] > y_angle [i - 1] - angleSensitivity) {
				decrementing = false;
				break;
			}
		}
		if (decrementing)
			BendUp ();

		//CHECK COUNTERCLOCKWISE TURNING (Z ANGLE INCREASES)
		angleSensitivity = 1.0f;
		incrementing = true;
		for (int i = 1; i < arduinoBufferSize; i++) {
			if (z_angle [i] < z_angle [i - 1] + angleSensitivity) {
				incrementing = false;
				break;
			}
		}
		if (incrementing)
			TurnCounterClockWise ();

		//CHECK CLOCKWISE TURNING (Z ANGLE DECREASES)
		angleSensitivity = 1.0f;
		decrementing = true;
		for (int i = 1; i < arduinoBufferSize; i++) {
			if (z_angle [i] > z_angle [i - 1] - angleSensitivity) {
				decrementing = false;
				break;
			}
		}
		if (decrementing)
			TurnClockWise ();

		//CHECK SHAKING
		float x_sensitivity = 0.1f,  y_sensitivity = 0.01f,  z_sensitivity = 0.06f;
		int positive_x=0, positive_y=0, positive_z=0;
		for (int i = 0; i < arduinoBufferSize; i++) {
			if (x_acc[i] > x_sensitivity)
				positive_x += 1;
			if (y_acc[i] > y_sensitivity)
				positive_y += 1;
			if (z_acc[i] - 0.95f > z_sensitivity)
				positive_z += 1;
		}
		//Debug.Log (positive_x + "    " + positive_y + "    " + positive_z);
		if (positive_x > 1 && positive_x < 5 &&
		    positive_y > 1 && positive_y < 5 &&
		    positive_z > 1 && positive_z < 5)
			Shake ();
	}

	/*private void UpdateEggLight() {
		if (eggLight.color != newLightColor) {
			t += Time.deltaTime;
			eggLight.color = new Color(
				Mathf.Lerp (actualLightColor.r, newLightColor.r, Mathf.SmoothStep (0, 1, t)),
				Mathf.Lerp (actualLightColor.g, newLightColor.g, Mathf.SmoothStep (0, 1, t)),
				Mathf.Lerp (actualLightColor.b, newLightColor.b, Mathf.SmoothStep (0, 1, t)),
				Mathf.Lerp (actualLightColor.a, newLightColor.a, Mathf.SmoothStep (0, 1, t)));
			if (eggLight.color == newLightColor) {
				actualLightColor = newLightColor;
				t = 0f;
			}
		}
	}*/

	private void AccRight() {
		audioSource.clip = accXAudio;
		audioSource.Play ();
		Debug.Log ("MOVE RIGHT");
		if (task == "X" && mustAccRight) {
			mustAccRight = false;
		}
	}

	private void AccLeft() {
		audioSource.clip = accXAudio;
		audioSource.Play ();
		Debug.Log ("MOVE LEFT");
		if (task == "X" && !mustAccRight) {
			mustAccRight = true;
			FillCompletionBar ();
		}
	}

	private void AccTop() {
		audioSource.clip = accYAudio;
		audioSource.Play ();
		Debug.Log ("MOVE TOP");
		if (task == "Y" && mustAccTop) {
			mustAccTop = false;
		}
	}

	private void AccBottom() {
		audioSource.clip = accYAudio;
		audioSource.Play ();
		Debug.Log ("MOVE BOTTOM");
		if (task == "Y" && !mustAccTop) {
			mustAccTop = true;
			FillCompletionBar ();
		}
	}

	private void AccUp() {
		audioSource.clip = accXAudio;
		audioSource.Play ();
		Debug.Log ("MOVE UP");
		if (task == "X" && mustAccRight) {
			mustAccRight = false;
		}
	}

	private void AccDown() {
		audioSource.clip = accXAudio;
		audioSource.Play ();
		Debug.Log ("MOVE DOWN");
		if (task == "X" && !mustAccRight) {
			mustAccRight = true;
			FillCompletionBar ();
		}
	}

	private void BendRight() {
		audioSource.clip = bendAudio;
		audioSource.Play ();
		Debug.Log ("BEND RIGHT");
		if (task == "RIBALTA" && mustBendRight) {
			mustBendRight = false;
		}
		if (task == "RUOTA") {
			if (ruotaDir == "Dx") {
				if (ruotaLevel < 3)
					ruotaLevel += 1;
				else {
					ruotaLevel = 0;
					FillCompletionBar ();
				}
			} else {
				ruotaDir = "Dx";
				ruotaLevel = 1;
			}
		}
	}

	private void BendLeft() {
		audioSource.clip = bendAudio;
		audioSource.Play ();
		Debug.Log ("BEND LEFT");
		if (task == "RIBALTA" && !mustBendRight) {
			mustBendRight = true;
			FillCompletionBar ();
		}
		if (task == "RUOTA") {
			if (ruotaDir == "Sx") {
				if (ruotaLevel < 3)
					ruotaLevel += 1;
				else {
					ruotaLevel = 0;
					FillCompletionBar ();
				}
			} else {
				ruotaDir = "Sx";
				ruotaLevel = 1;
			}
		}
	}

	private void BendUp() {
		audioSource.clip = bendAudio;
		audioSource.Play ();
		Debug.Log ("BEND UP");
		if (task == "RIBALTA" && mustBendRight) {
			mustBendRight = false;
			FillCompletionBar ();
		}
		if (task == "RUOTA") {
			if (ruotaDir == "Up") {
				if (ruotaLevel < 3)
					ruotaLevel += 1;
				else {
					ruotaLevel = 0;
					FillCompletionBar ();
				}
			} else {
				ruotaDir = "Up";
				ruotaLevel = 1;
			}
		}
	}

	private void BendDown() {
		audioSource.clip = bendAudio;
		audioSource.Play ();
		Debug.Log ("BEND DOWN");
		if (task == "RIBALTA" && !mustBendRight) {
			mustBendRight = true;
			FillCompletionBar ();
		}
		if (task == "RUOTA") {
			if (ruotaDir == "Down") {
				if (ruotaLevel < 3)
					ruotaLevel += 1;
				else {
					ruotaLevel = 0;
					FillCompletionBar ();
				}
			} else {
				ruotaDir = "Down";
				ruotaLevel = 1;
			}
		}
	}

	private void TurnCounterClockWise() {
		audioSource.clip = turnAudio;
		audioSource.Play ();
		Debug.Log ("TURN COUNTERCLOCKWISE");
	}

	private void TurnClockWise() {
		audioSource.clip = turnAudio;
		audioSource.Play ();
		Debug.Log ("TURN CLOCKWISE");
	}

	private void Shake() {
		audioSource.clip = shakeAudio;
		audioSource.Play ();
		Debug.Log ("SHAKE");
		if (task == "SHAKE")
			FillCompletionBar ();
	}

	private void FillCompletionBar () {
		if (!waitBeforeContinue) {
			waitBeforeContinue = true;
			switch (Manager.monsterName) {
			default:
				if (intensity < MAX_INTENSITY)
					intensity += 1;
				
				if (brightness < MAX_BRIGHTNESS) {
					brightness += 90;
					LightUpLedBar ();
					buio.PlayEyesAnimation ("si");
					StartCoroutine(WaitBeforeContinue(1.5f));

				} else {
					buio.PlayEyesAnimation ("felice");
					ShowMonster ();
				}
				break;
			}
		}
	}

	IEnumerator WaitBeforeContinue(float time) {
		yield return new WaitForSeconds (time);
		waitBeforeContinue = false;
	}

	private void ShowMonster() {

		buio.PlayEyesAnimation ("felice");

		monsterQueueManager.ShowMonster ();

		//Arduino
		intensity = 1;
		brightness = 20;
		color = "ffffff";
		LightUpLedBar ();

		switch(Manager.monsterName) {

		case("Armadio"):
			ShowEffect (1, 9000);
			break;
		case("Sedia"):
			ShowEffect (2, 9000);
			break;
		case("Specchio"):
			ShowEffect (3, 9000);
			break;
		case("Comodino"):
			ShowEffect (9, 9000);
			break;
		case("Lampada"):
			ShowEffect (4, 9000);
			break;
		case("Appendino"):
			ShowEffect (6, 9000);
			break;
		default:
			break;
		}
	}

	//Methods for Arduino

	public void WriteOnArduino(string command) {
		am.WriteOnArduino (command);
	}

	void OnDestroy ()
	{
		if (am != null) {
			am.SwitchOff ();
			am.Destroy ();
		}
	}

	void OnApplicationQuit ()
	{
		if (am != null) {
			am.SwitchOff ();
			am.Destroy ();
		}
	}

	void LightUpLedBar() {
		am.SetIntensity (intensity);
		am.SetBrightness (brightness);
		am.SetColor (color);
	}

	public void ShowEffect(int effectNumber, int millis = 10000) {
		am.ShowEffect (2, effectNumber, millis);
	}

}
