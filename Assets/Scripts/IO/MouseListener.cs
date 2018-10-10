using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseListener : MonoBehaviour {
	void Update () {
		ListenLeftClick();
	}

	private void ListenLeftClick() {
		if(!Input.GetMouseButtonDown(0)) return;
		Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		Game.Instance.Shot(mousePos);
	}
}
