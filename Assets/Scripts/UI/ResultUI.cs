using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultUI : SingletonMonoBehaviour<ResultUI> {
	private Game game;
	///リザルト表示用TextObjects（Sceneを分けたくなかったため用意)
	private Text totalTimeText;
	private Text scoreText;
	private Text accuracyText;


	// Use this for initialization
	void Start () {
		totalTimeText = GameObject.Find("TotalTimeText").GetComponent<Text>();
		scoreText = GameObject.Find("ScoreText").GetComponent<Text>();
		accuracyText = GameObject.Find("AccuracyText").GetComponent<Text>();
		game = Game.Instance;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Initialize() {
		totalTimeText.text = "";
		scoreText.text = "";
		accuracyText.text = "";
	}

	public void DisplayResult() {
		GameObject resultTarget = Instantiate(game.GetTargetPrefab(), new Vector3(0, -0.8f, 0), Quaternion.identity);
		resultTarget.GetComponent<Target>().changeModeToResultTarget(game.GetHitPositions());
		accuracyText.text = "Accuracy: " + ((double)game.GetScore() / game.GetShots() * 100).ToString("F1") + "%";
		totalTimeText.text = "Tortal time: " + Timer.Instance.GetTime().ToString("F2");
		scoreText.text = "Targets hit: " + game.GetScore().ToString();
	}
}
