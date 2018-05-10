using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EggScript : MonoBehaviour {

	public Light eggLight;
	Color actualLightColor = Color.white;
	Color newLightColor = Color.white;

	float t = 0f;

	void Start () {
	}

	void Update () {
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

		if (Input.GetKeyDown (KeyCode.A)) {
			ChangeLightColor ((float)Random.Range (0, 256) / 255, (float)Random.Range (0, 256) / 255, (float)Random.Range (0, 256) /255);
		}
		if (Input.GetKeyDown (KeyCode.S)) {
			ChangeLightColor (1,1,1);
		}
	}

	public void ChangeLightColor(float r, float g, float b) {
		newLightColor = new Color (r, g, b);
	}

	public void ChangeLightColor(Color color) {
		newLightColor = color;
	}
}
