using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarScript : MonoBehaviour {

	Sprite sprite;
	ParticlesAttractorScript particlesEmitter;
	float worldScreenHeight;
	float worldScreenWidth;

	bool canEmit = true;

	void Start () {

		sprite = gameObject.GetComponent<SpriteRenderer> ().sprite;
		particlesEmitter = gameObject.GetComponent<ParticlesAttractorScript> ();
		worldScreenHeight = Camera.main.orthographicSize * 2f;
		worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;

	}

	void Update () {

		Vector2 spritePosition = GetSpritePositionOnScreen ();

		foreach(Touch touch in Input.touches) {
			if (touch.position.x > spritePosition.x + 15 * sprite.bounds.min.x &&
			    touch.position.x < spritePosition.x + 15 * sprite.bounds.max.x &&
			    touch.position.y > spritePosition.y + 15 * sprite.bounds.min.y &&
			    touch.position.y < spritePosition.y + 15 * sprite.bounds.max.y && canEmit) {

				canEmit = false;
				StartCoroutine (SpawnParticles ());

			}

		}
		
	}

	IEnumerator SpawnParticles() {
		particlesEmitter.StartParticles ();
		yield return new WaitForSeconds (1);
		particlesEmitter.StopParticles ();

		canEmit = true;
	}

	public Vector2 GetSpritePositionOnScreen() {
		Vector2 position;
		position.x = (transform.position.x + worldScreenWidth/2) / worldScreenWidth * Screen.width;
		position.y = (transform.position.y + worldScreenHeight/2) / worldScreenHeight * Screen.height;
		return position;
	}
}
