using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuioAnimationScr : MonoBehaviour {

	Animator animator;

	void Start () {
		animator = GetComponent<Animator> ();
		transform.localScale = new Vector3 (120, 120, 1);
		PlayEyesAnimation ("risveglio");
	}

	void Update () {
		
	}

	public void PlayEyesAnimation(string animationName)
	{
		float time;
		switch (animationName) {
		case "blink":
			time = 5.0f;
			break;
		default:
			time = 5.0f;
			break;
		}
		StopCoroutine ("PlayAnimation");
		StartCoroutine(PlayAnimation(animationName, "blink", time));
	}

	IEnumerator PlayAnimation(string newAnimationName, string returnToThisAnimation, float time) {
		animator.Play (newAnimationName);

		yield return new WaitForSeconds (time);

		animator.Play (returnToThisAnimation);
	}
}
