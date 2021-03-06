﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RealMonsterScript : MonoBehaviour {

	//public Material _mat;

	public bool queued = false;

	private bool uncovered = false;
	private float blendingValue = 0f;
	private float blendSpeed = 0.25f;

	private MonsterQueueManagerScript queueManager;
	private BraccioBuioScript braccioBuio;

	// Use this for initialization
	void Start () {
		queueManager = GameObject.Find ("MonsterQueueManager").GetComponent<MonsterQueueManagerScript>();
		braccioBuio = GameObject.Find ("BraccioBuio").GetComponent<BraccioBuioScript>();
	}

	// Update is called once per frame
	void Update () {
		if (!queued) {

			if (uncovered && blendingValue >= 1f) {
				StartCoroutine(StartGreetingAnimation());
				uncovered = false;
			} else if (uncovered && blendingValue < 1f) {
				blendingValue += blendSpeed * Time.deltaTime;
			}

			//_mat.SetFloat ("_LerpValue", blendingValue);

			Color tmp = this.GetComponent<SpriteRenderer>().color;
			tmp.a = blendingValue;
			this.GetComponent<SpriteRenderer>().color = tmp;
		}
	}

	public void Uncover() {
		uncovered = true;
	}

	IEnumerator StartGreetingAnimation() {
		yield return new WaitForSeconds (5);
		braccioBuio.Animation_NextMonster ();
		yield return new WaitForSeconds (2.5f);
		queueManager.NextMonster ();
	}

}
