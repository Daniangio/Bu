using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BraccioBuioScript : MonoBehaviour {

	Animator animator;

	void Start () {
		animator = GetComponent<Animator> ();
		transform.localScale = new Vector3 (150, 150, 150);
		animator.Play ("Idle");
	}

	void Update () {
		
	}

	public void Animation_NextMonster() {
		StartCoroutine(ChangeMonster());
	}


	IEnumerator ChangeMonster()
	{
		animator.Play ("ChangeMonster");

		yield return new WaitForSeconds(4);

		animator.Play ("Idle");
	}
}
