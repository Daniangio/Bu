using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MonsterQueueManagerScript : MonoBehaviour {

	private List<string> monstersList;
	private GameObject prefab;
	private GameObject currentMonster;

	// Use this for initialization
	void Start () {
		prefab = (GameObject)Resources.Load ("Prefabs/MonsterPrefab", typeof(GameObject));

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

			if (monstersList.Count > 1)
				queueMonster = monstersList [1];
		
			monstersList.RemoveAt (0);

			if (nextMonster == "Mario") {
				currentMonster = Instantiate (prefab, new Vector3 (-4.5f, 0, -1), Quaternion.identity);
				currentMonster.GetComponent<MonsterScript> ().queued = false;
			}
		}
	}

}
