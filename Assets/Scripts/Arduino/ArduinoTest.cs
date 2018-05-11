using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Arduino;

public class ArduinoTest : MonoBehaviour {

	ArduinoManager am;
	bool ok = false;

	// Use this for initialization
	void Start () {

		am = new ArduinoManager ("COM7");
		ok = true;

		am.StartProgressBar (10);

	}
	
	// Update is called once per frame
	void Update () {
		if (ok) {
			string response = am.ReadFromArduino ();
			if (response != null)
				Debug.Log(response);
		}

		if (Input.GetKeyDown ("space")) {
			am.ShowEffect (2, 1, 10000);
		}
	}

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

}

