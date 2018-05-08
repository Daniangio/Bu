using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsManager : MonoBehaviour {

	public int levelDifficulty = 1;

	public void SetGameDifficulty(int level) {
		levelDifficulty = level;
	}

	void OnEnable() {
		GameObject levelButton = null;
		switch(levelDifficulty) {
		case 1:
			levelButton = GameObject.Find ("EasyLevelButton");
			break;
		case 2:
			levelButton = GameObject.Find ("HardLevelButton");
			break;
		default:
			break;
		}
		
		GameObject eventSystem = GameObject.Find ("EventSystem");
		eventSystem.GetComponent<UnityEngine.EventSystems.EventSystem> ().SetSelectedGameObject (levelButton);
	}
}
