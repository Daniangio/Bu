using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MonsterQueueManagerScript : MonoBehaviour {

	private List<string> monstersList;
	private GameObject shadowPrefab;
	private GameObject realPrefab;
	private GameObject currentMonster;

	private ShadowMonsterScript shadowMonster;
	private RealMonsterScript realMonster;

	public EggScript egg;
	public BuioAnimationScr buio;

	//LIGHT
	public Light ambientLight;
	public Light monsterLight;
	Color actualLightColor = Color.white;
	Color newLightColor = Color.white;

	float t = 0f;

	float actualLightRange = 28f;
	float newLightRange = 40f;

	float r = 0f;

	// Use this for initialization
	void Start () {
		shadowPrefab = (GameObject)Resources.Load ("Prefabs/ShadowMonsterPrefab", typeof(GameObject));
		realPrefab = (GameObject)Resources.Load ("Prefabs/RealMonsterPrefab", typeof(GameObject));

		monstersList = new List<string> ();
		LoadMonsters ();
		monstersList = monstersList.OrderBy (x => Random.value).ToList(); //Shuffle the list

		NextMonster ();
	}
	
	// Update is called once per frame
	void Update () {

		UpdateLight ();
		
	}

	private void LoadMonsters() {
		monstersList.Add ("Armadio");
		monstersList.Add ("Sedia");
		monstersList.Add ("Specchio");
		monstersList.Add ("Comodino");
		monstersList.Add ("Lampada");
		monstersList.Add ("Appendino");
	}

	public void NextMonster() {
		if (currentMonster != null)
			Destroy (currentMonster);

		string nextMonster;
		string queueMonster;

		//Ambient Light
		ChangeLightColor (0.7f, 0.7f, 0.7f);
		newLightRange = 40f;

		//Pick next monster
		if (monstersList.Count > 0) {
			nextMonster = monstersList [0];

			Manager.monsterName = nextMonster;

			if (monstersList.Count > 1)
				queueMonster = monstersList [1];
		
			monstersList.RemoveAt (0);

			Sprite spr_s = null, spr = null;
			string stateName = "";
			switch (nextMonster) {
			case "Armadio":
				spr_s = Resources.Load ("Sprites/armadio_s", typeof(Sprite)) as Sprite;
				spr = Resources.Load ("Sprites/armadio", typeof(Sprite)) as Sprite;
				egg.GetComponent<Animator> ().SetInteger ("state", 0);
				stateName = "ribalta";
				egg.task = "RIBALTA";
				break;
			case "Sedia":
				spr_s = Resources.Load ("Sprites/sedia_s", typeof(Sprite)) as Sprite;
				spr = Resources.Load ("Sprites/sedia", typeof(Sprite)) as Sprite;
				egg.GetComponent<Animator> ().SetInteger ("state", 1);
				stateName = "ruota";
				egg.task = "RUOTA";
				break;
			case "Specchio":
				spr_s = Resources.Load ("Sprites/specchio_s", typeof(Sprite)) as Sprite;
				spr = Resources.Load ("Sprites/specchio", typeof(Sprite)) as Sprite;
				egg.GetComponent<Animator> ().SetInteger ("state", 2);
				stateName = "shake";
				egg.task = "SHAKE";
				break;
			case "Comodino":
				spr_s = Resources.Load ("Sprites/comodino_s", typeof(Sprite)) as Sprite;
				spr = Resources.Load ("Sprites/comodino", typeof(Sprite)) as Sprite;
				egg.GetComponent<Animator> ().SetInteger ("state", 3);
				stateName = "x";
				egg.task = "X";
				break;
			case "Lampada":
				spr_s = Resources.Load ("Sprites/lampada_s", typeof(Sprite)) as Sprite;
				spr = Resources.Load ("Sprites/lampada", typeof(Sprite)) as Sprite;
				egg.GetComponent<Animator> ().SetInteger ("state", 4);
				stateName = "y";
				egg.task = "Y";
				break;
			case "Appendino":
				spr_s = Resources.Load ("Sprites/appendino_s", typeof(Sprite)) as Sprite;
				spr = Resources.Load ("Sprites/appendino", typeof(Sprite)) as Sprite;
				egg.GetComponent<Animator> ().SetInteger ("state", 0);
				stateName = "ribalta";
				egg.task = "RIBALTA";
				break;
			default:
				break;
			}

			egg.GetComponent<Animator> ().Play(stateName);
			
				currentMonster = Instantiate (shadowPrefab, new Vector3 (20f, -11f, 0), Quaternion.identity);
				currentMonster.GetComponent<SpriteRenderer> ().sprite = spr_s;
				currentMonster.transform.localScale = new Vector3 (8, 8, 1);
				shadowMonster = currentMonster.GetComponent<ShadowMonsterScript> ();
				shadowMonster.queued = false;
				currentMonster = Instantiate (realPrefab, new Vector3 (20f, -11f, 0), Quaternion.identity);
				currentMonster.GetComponent<SpriteRenderer> ().sprite = spr;
				currentMonster.transform.localScale = new Vector3 (8, 8, 1);
				realMonster = currentMonster.GetComponent<RealMonsterScript> ();
				realMonster.queued = false;
		}

		buio.PlayEyesAnimation ("shake");
		egg.waitBeforeContinue = false;
	}

	public void ShowMonster() {
		shadowMonster.Uncover ();
		realMonster.Uncover ();

		ChangeLightColor (1f, 1f, 1f);
		newLightRange = 28f;
	}

	private void UpdateLight() {
		if (ambientLight.color != newLightColor) {
			t += Time.deltaTime;
			ambientLight.color = new Color(
				Mathf.Lerp (actualLightColor.r, newLightColor.r, Mathf.SmoothStep (0, 1, t)),
				Mathf.Lerp (actualLightColor.g, newLightColor.g, Mathf.SmoothStep (0, 1, t)),
				Mathf.Lerp (actualLightColor.b, newLightColor.b, Mathf.SmoothStep (0, 1, t)),
				Mathf.Lerp (actualLightColor.a, newLightColor.a, Mathf.SmoothStep (0, 1, t)));
			if (ambientLight.color == newLightColor) {
				actualLightColor = newLightColor;
				t = 0f;
			}
		}

		if (actualLightRange != newLightRange) {
			if (actualLightRange > newLightRange)
				r += Time.deltaTime / 20;
			else
				r += Time.deltaTime / 5;
			actualLightRange = Mathf.Lerp (actualLightRange, newLightRange, Mathf.SmoothStep (0, 1, r));
			if (actualLightRange == newLightRange) {
				r = 0f;
			}

			monsterLight.range = actualLightRange;
		}
	}

	public void ChangeLightColor(float r, float g, float b) {
		newLightColor = new Color (r, g, b);
	}

	public void ChangeLightColor(Color color) {
		newLightColor = color;
	}

}
