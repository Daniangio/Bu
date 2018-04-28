﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResizeSpriteScript : MonoBehaviour {

	void Update () {

		ResizeSpriteToScreen();

	}

	public void ResizeSpriteToScreen() {

		SpriteRenderer sr = gameObject.GetComponent<SpriteRenderer>();
		if (sr == null)
			return;

		float width = sr.sprite.bounds.size.x;
		float height = sr.sprite.bounds.size.y;

		float worldScreenHeight = Camera.main.orthographicSize * 2f;
		float worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;

		Vector3 localScaleVector = transform.localScale;
		localScaleVector.x = worldScreenWidth / width;
		localScaleVector.y = worldScreenHeight / height;

		transform.localScale = localScaleVector;
	} 

}
