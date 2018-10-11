using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : SingletonMonoBehaviour<MainMenuUI> {

	private Button startButton;
	private Button optionsButton;

	// Use this for initialization
	void Start () {
		startButton = GameObject.Find("StartButton").GetComponent<Button>();
		optionsButton = GameObject.Find("OptionsButton").GetComponent<Button>();
		Display();
	}

	///
	/// Startボタンクリック処理
	/// 
	public void OnClickStartButton() {
		GameManager.Instance.GameStart();
		Hide();
	}

	/// 
	/// Optionsボタンクリック処理
	///
	public void OnClickOptionsButton() {
		OptionsMenuUI.Instance.Display();
	}

	public void Hide() {
		startButton.gameObject.SetActive(false);
		optionsButton.gameObject.SetActive(false);
	}

	public void Display() {
		startButton.gameObject.SetActive(true);
		optionsButton.gameObject.SetActive(true);
	}
}
