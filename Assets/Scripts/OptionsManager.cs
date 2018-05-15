using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsManager : MonoBehaviour {

	public void SetGameDifficulty(int level) {
		Manager.difficultyLevel = level;
		EnableDifficultyPanel ();
	}

	public void EnableDifficultyPanel() {
		GameObject levelButton = null;
		switch(Manager.difficultyLevel) {
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
		eventSystem.GetComponent<UnityEngine.EventSystems.EventSystem> ().SetSelectedGameObject(levelButton);
	}
}
