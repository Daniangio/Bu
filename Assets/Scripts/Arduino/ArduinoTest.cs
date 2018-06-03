using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Arduino;

public class ArduinoTest : MonoBehaviour {

	ArduinoManager am;
	//bool ok = false;

	// Use this for initialization
	void Start () {

		//am = new ArduinoManager ("\\\\.\\COM27");
		//ok = true;

	}
	
	// Update is called once per frame
	void Update () {
		/*if (ok) {
			string response = am.ReadFromArduino ();
			if (response != null) {
				int type = int.Parse (response.Split ("," [0]) [0]);
				Debug.Log (type);
				switch(type) {
				case (13):
					int light = int.Parse (response.Split ("," [0]) [1]);
					Debug.Log (light);
					Manager.lightFlag = (light < 1000) ? true : false;
					Debug.Log (Manager.lightFlag);
					break;
				default:
					Debug.Log (response);
					break;
				}
			}
		}*/

		if (Input.GetMouseButtonDown (0)) {
			//am.SetLightFlag (true);
			Manager.lightFlag = true;
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

