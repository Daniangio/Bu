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
		monstersList.Add ("Mario");
		monstersList.Add ("Mario");
		monstersList.Add ("Mario");
		monstersList.Add ("Mario");
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

			if (nextMonster == "Mario") {
				currentMonster = Instantiate (shadowPrefab, new Vector3 (20f, -11f, -1), Quaternion.identity);
				shadowMonster = currentMonster.GetComponent<ShadowMonsterScript> ();
				shadowMonster.queued = false;
				currentMonster = Instantiate (realPrefab, new Vector3 (20f, -11f, -1), Quaternion.identity);
				realMonster = currentMonster.GetComponent<RealMonsterScript> ();
				realMonster.queued = false;
			}
		}
	}

	public void ShowMonster() {
		shadowMonster.Uncover ();
		realMonster.Uncover ();
	}

}
