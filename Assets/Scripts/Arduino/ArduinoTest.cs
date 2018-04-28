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
		
	}
	
	// Update is called once per frame
	void Update () {
		if (ok) {
			string response = am.ReadFromArduino ();
			if (response != null)
				Debug.Log(response);
		}

		if (Input.GetKeyDown ("space")) {
			WriteOnArduino ("abc");
		}
	}

	public void WriteOnArduino(string command) {
		am.WriteOnArduino (command);
	}
}

