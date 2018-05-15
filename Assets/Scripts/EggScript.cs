using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Arduino;


public class EggScript : MonoBehaviour {

	public MonsterQueueManagerScript monsterQueueManager;

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
	string DEFAULT_COLOR = "ffffff";

	int intensity = 1;
	int brightness = 20;
	string color = "ffffff";

	void Start () {
		am = new ArduinoManager ("\\\\.\\COM26");
		ok = true;

		am.StartBar ();
		LightUpLedBar ();
	}

	void Update () {

		UpdateLedBar ();
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

	private void UpdateLedBar() {
		if (ok) {
			string response = am.ReadFromArduino ();
			if (response != null)
				Debug.Log(response);
		}
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

	private void FillCompletionBar () {
		switch (Manager.monsterName) {
		case "Mario":
			if (intensity < MAX_INTENSITY)
				intensity += 1;
			if (brightness < MAX_BRIGHTNESS) {
				brightness += 45;
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
		brightness = 20;
		color = "ffffff";
		LightUpLedBar ();

		switch(Manager.monsterName) {

			case("Mario"):
				ShowEffect (6, 10000);
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
