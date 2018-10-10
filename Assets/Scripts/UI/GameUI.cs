using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : SingletonMonoBehaviour<GameUI> {
	private Game game;
	private Text timerText;
	private Text healthText;

	// Use this for initialization
	void Start () {
		game = Game.Instance;
		timerText = GameObject.Find("TimerText").gameObject.GetComponent<Text>();
		healthText = GameObject.Find("HealthText").GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
		UpdateTimerText();
	}

	private void UpdateTimerText() {
		if(!Game.Instance.isPlaying()) return;
		timerText.text = Timer.Instance.GetTime().ToString("F2");
	}

	///
	/// 残りHealthの表示テキストを生成
	///
	private string generateHealthText() {
		return "Health: " + game.GetHealth().ToString();
	}

	public void Hide() {
		timerText.enabled = false;
		healthText.enabled = false;
	}

	public void Display() {
		timerText.enabled = true;
		healthText.enabled = true;	
	}

	//残りHealthの表示内容を更新する
	public void UpdateHealth() {
		healthText.text = generateHealthText();
	}
}
