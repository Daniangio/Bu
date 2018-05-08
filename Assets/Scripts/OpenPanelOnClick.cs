using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenPanelOnClick : MonoBehaviour {

	bool panelOpen = false;
	public GameObject defaultPanel;
	GameObject currentPanel;

	public void OpenPanel() {
		currentPanel = GameObject.FindGameObjectWithTag ("OptionsPanel");
		if (currentPanel == null)
			currentPanel = defaultPanel;
		
		panelOpen = !panelOpen;
		currentPanel.SetActive(panelOpen);
	}

	public void SwitchPanel() {
		defaultPanel.SetActive(true);
		this.transform.parent.gameObject.SetActive (false);
	}
}
