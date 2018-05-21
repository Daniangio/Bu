using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Arduino;


public class EggScript : MonoBehaviour {

	public MonsterQueueManagerScript monsterQueueManager;

	//TASK VARIABLES
	string task = "BENDING HOR";
	bool mustBendRight = true;

	//Gyroscope variables
	//float basex_angle, basey_angle, basez_angle, basex_acc, basey_acc, basez_acc;
	//bool calibrate = true;

	int arduinoBufferSize=5;
	float[] x_angle, y_angle, z_angle, x_acc, y_acc, z_acc;
	int arduinoCounter=0;

	//Arduino Led Bar
	ArduinoManager am;
	bool ok = false;

	//Light Effects on Screen
	public Light eggLight;
	Color actualLightColor = Color.white;
	Color newLightColor = Color.white;

	float t = 0f;

	//Task oriented variables
	int MAX_INTENSITY = 4;
	int MAX_BRIGHTNESS = 200;

	int intensity = 1;
	int brightness = 50;
	string color = "ffffff";

	void Start () {
		am = new ArduinoManager ("\\\\.\\COM26");
		ok = true;

		x_angle = new float[arduinoBufferSize];
		y_angle = new float[arduinoBufferSize];
		z_angle = new float[arduinoBufferSize];
		x_acc = new float[arduinoBufferSize];
		y_acc = new float[arduinoBufferSize];
		z_acc = new float[arduinoBufferSize];

		am.StartBar ();
		LightUpLedBar ();
	}

	void Update () {

		ReadFromArduino ();
		UpdateEggLight ();


		//JUST FOR TESTING
		if (Input.GetKeyDown (KeyCode.A)) {
			ChangeLightColor ((float)Random.Range (0, 256) / 255, (float)Random.Range (0, 256) / 255, (float)Random.Range (0, 256) /255);
		}
		if (Input.GetKeyDown (KeyCode.S)) {
			ChangeLightColor (1,1,1);
		}
		if (Input.GetKeyDown (KeyCode.Z)) {
			FillCompletionBar ();
		}
	}

	private void ReadFromArduino() {
		if (ok) {
			string response = am.ReadFromArduino ();
			if (response != null) {
				int type = int.Parse (response.Split ("," [0]) [0]);
				switch(type) {
				case (12):
					float.TryParse (response.Split ("," [0]) [1], out x_angle [arduinoCounter]);
					float.TryParse (response.Split ("," [0]) [2], out y_angle [arduinoCounter]);
					float.TryParse (response.Split ("," [0]) [3], out z_angle [arduinoCounter]);
					float.TryParse (response.Split ("," [0]) [4], out x_acc [arduinoCounter]);
					float.TryParse (response.Split ("," [0]) [5], out y_acc [arduinoCounter]);
					float.TryParse (response.Split ("," [0]) [6], out z_acc [arduinoCounter]);
					NormalizeAcceleration ();
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
		
		//CHECK RIGHT BENDING (X ANGLE INCREASES)
		float angleSensitivity = 1.0f;
		bool incrementing = true;
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

	private void UpdateEggLight() {
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
	}

	private void BendRight() {
		Debug.Log ("BEND RIGHT");
		if (task == "BENDING HOR" && mustBendRight) {
			mustBendRight = false;
			FillCompletionBar ();
		}
	}

	private void BendLeft() {
		Debug.Log ("BEND LEFT");
		if (task == "BENDING HOR" && !mustBendRight) {
			mustBendRight = true;
			FillCompletionBar ();
		}
	}

	private void BendUp() {
		Debug.Log ("BEND UP");
	}

	private void BendDown() {
		Debug.Log ("BEND DOWN");
	}

	private void TurnCounterClockWise() {
		Debug.Log ("TURN COUNTERCLOCKWISE");
	}

	private void TurnClockWise() {
		Debug.Log ("TURN CLOCKWISE");
	}

	private void Shake() {
		Debug.Log ("SHAKE");
	}

	private void FillCompletionBar () {
		switch (Manager.monsterName) {
		case "Mario":
			if (intensity < MAX_INTENSITY)
				intensity += 1;
			if (brightness < MAX_BRIGHTNESS) {
				brightness += 50;
				LightUpLedBar ();
			}
			else
				ShowMonster ();
			break;
		default:
			break;
		}
	}

	private void ShowMonster() {
		monsterQueueManager.ShowMonster ();
		intensity = 1;
		brightness = 50;
		color = "ffffff";
		LightUpLedBar ();

		switch(Manager.monsterName) {

			case("Mario"):
				ShowEffect (2, 10000);
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

	//Methods for Egg's light

	public void ChangeLightColor(float r, float g, float b) {
		newLightColor = new Color (r, g, b);
	}

	public void ChangeLightColor(Color color) {
		newLightColor = color;
	}

}
