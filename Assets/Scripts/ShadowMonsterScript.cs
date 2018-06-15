using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowMonsterScript : MonoBehaviour {

	//public Material _mat;

	public bool queued = false;

	private bool uncovered = false;
	private float blendingValue = 1f;
	private float blendSpeed = 0.25f;

	//private MonsterQueueManagerScript queueManager;

	// Use this for initialization
	void Start () {
		//queueManager = GameObject.Find ("MonsterQueueManager").GetComponent<MonsterQueueManagerScript>();
	}
	
	// Update is called once per frame
	void Update () {
		if (!queued) {

			if (uncovered && blendingValue <= 0f) {
				Destroy (this.gameObject);
			} else if (uncovered && blendingValue > 0f) {
				blendingValue -= blendSpeed * Time.deltaTime;
			}

			//_mat.SetFloat ("_LerpValue", blendingValue);

			Color tmp = this.GetComponent<SpriteRenderer>().color;
			if (uncovered && blendingValue > 0f) {
				gameObject.GetComponent<SpriteRenderer> ().material.SetColor("_Color", new Color(34,18,55));
			}
			tmp.a = blendingValue;
			this.GetComponent<SpriteRenderer>().color = tmp;
		}
	}

	public void Uncover() {
		uncovered = true;
	}

}
