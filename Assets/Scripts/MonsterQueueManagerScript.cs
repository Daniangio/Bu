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

		if (monstersList.Count > 0) {
			nextMonster = monstersList [0];

			Manager.monsterName = nextMonster;

			if (monstersList.Count > 1)
				queueMonster = monstersList [1];
		
			monstersList.RemoveAt (0);

			Sprite spr_s = null, spr = null;
			if (nextMonster == "Armadio") {
				spr_s = Resources.Load ("Sprites/armadio_s", typeof(Sprite)) as Sprite;
				spr = Resources.Load ("Sprites/armadio", typeof(Sprite)) as Sprite;
				egg.GetComponent<Animator> ().SetInteger ("state", 0);
			}
			if (nextMonster == "Sedia") {
				spr_s = Resources.Load ("Sprites/sedia_s", typeof(Sprite)) as Sprite;
				spr = Resources.Load ("Sprites/sedia", typeof(Sprite)) as Sprite;
				egg.GetComponent<Animator> ().SetInteger ("state", 1);
			}
			if (nextMonster == "Specchio") {
				spr_s = Resources.Load ("Sprites/specchio_s", typeof(Sprite)) as Sprite;
				spr = Resources.Load ("Sprites/specchio", typeof(Sprite)) as Sprite;
				egg.GetComponent<Animator> ().SetInteger ("state", 2);
			}
			if (nextMonster == "Comodino") {
				spr_s = Resources.Load ("Sprites/comodino_s", typeof(Sprite)) as Sprite;
				spr = Resources.Load ("Sprites/comodino", typeof(Sprite)) as Sprite;
				egg.GetComponent<Animator> ().SetInteger ("state", 3);
			}
			if (nextMonster == "Lampada") {
				spr_s = Resources.Load ("Sprites/lampada_s", typeof(Sprite)) as Sprite;
				spr = Resources.Load ("Sprites/lampada", typeof(Sprite)) as Sprite;
				egg.GetComponent<Animator> ().SetInteger ("state", 4);
			}
			if (nextMonster == "Appendino") {
				spr_s = Resources.Load ("Sprites/appendino_s", typeof(Sprite)) as Sprite;
				spr = Resources.Load ("Sprites/appendino", typeof(Sprite)) as Sprite;
				egg.GetComponent<Animator> ().SetInteger ("state", 0);
			}
			egg.GetComponent<Animator> ().Play ("state");
			
				currentMonster = Instantiate (shadowPrefab, new Vector3 (20f, -11f, -1), Quaternion.identity);
				currentMonster.GetComponent<SpriteRenderer> ().sprite = spr_s;
				currentMonster.transform.localScale = new Vector3 (8, 8, 1);
				shadowMonster = currentMonster.GetComponent<ShadowMonsterScript> ();
				shadowMonster.queued = false;
				currentMonster = Instantiate (realPrefab, new Vector3 (20f, -11f, -1), Quaternion.identity);
				currentMonster.GetComponent<SpriteRenderer> ().sprite = spr;
				currentMonster.transform.localScale = new Vector3 (8, 8, 1);
				realMonster = currentMonster.GetComponent<RealMonsterScript> ();
				realMonster.queued = false;
		}
	}

	public void ShowMonster() {
		shadowMonster.Uncover ();
		realMonster.Uncover ();
	}

}
