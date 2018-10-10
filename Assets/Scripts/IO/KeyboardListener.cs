using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardListener : MonoBehaviour {
	void Update () {
		ListenESC();
	}

	public void ListenESC() {
		if(!Input.GetKeyDown(KeyCode.Escape)) return;
		
		if(Game.Instance.isPlaying()) Game.Instance.Pause();
		if(OptionsMenuUI.Instance.gameObject.activeSelf) OptionsMenuUI.Instance.Hide();
		else OptionsMenuUI.Instance.Display();
	}

	public void ListenDF() {
		if(!Input.GetKeyDown(KeyCode.D) && !Input.GetKeyDown(KeyCode.F)) return;
		Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		Game.Instance.Shot(mousePos);
	}
}
