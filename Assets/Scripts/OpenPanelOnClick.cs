using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OpenPanelOnClick : MonoBehaviour {

	public GameObject[] openThesePanels;
	public GameObject[] closeThesePanels;
	GameObject currentPanel;

	/*public void OpenPanel() {
		currentPanel = GameObject.FindGameObjectWithTag ("OptionsPanel");
		if (currentPanel == null)
			currentPanel = defaultPanel;
		
		panelOpen = !panelOpen;
		currentPanel.SetActive(panelOpen);
	}*/

	public void SwitchPanel() {
		for (int i=0; i<openThesePanels.Length; i++)
			openThesePanels[i].SetActive(true);

		for (int i = 0; i < closeThesePanels.Length; i++)
			closeThesePanels [i].SetActive (false);
		
		this.transform.parent.gameObject.SetActive (false);
	}

	public void ChangeScene(string sceneName) {
		SceneManager.LoadScene (sceneName);
	}

	void Update() {
		if (Input.GetKeyDown (KeyCode.X))
			ChangeScene ("TurnOffLightScene");
	}
}
