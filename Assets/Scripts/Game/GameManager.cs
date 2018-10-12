﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ゲームプログラム全体にあたるメインのScript
public class GameManager : SingletonMonoBehaviour<GameManager> {
	public GameObject targetPrefab;
	public GameObject guideLinePrefab;

	///TargetをHitした回数
	private int score;
	///HP。Targetを逃すと1減る。
	private int health;
	//クリックした回数
	private int shots;
	//射撃時の音
	private AudioSource hitSound;
	// ゲーム中に射撃したターゲットの相対位置リスト
	private List<Vector3> hitPositions = new List<Vector3>();
	///スタートボタンクリックフラグ
	private bool playing = false;
	// ポーズ中か否か
	private bool pausing = false;
	//ゲームシステム
	private IGameMode gameMode;

	void Start () {
		hitSound = GameObject.Find("Main Camera").GetComponent<AudioSource>();
		SettingInitialize();
	}

	void Update () {
		if(!playing || pausing) return;
		gameMode.GameProcess();
	}

	public int GetHealth() {
		return health;
	}

	public void SetHealth(int health) {
		this.health = health;
	}

	public int GetShots() {
		return shots;
	}

	public int GetScore() {
		return score;
	}

	public GameObject GetTargetPrefab() {
		return targetPrefab;
	}

	public List<Vector3> GetHitPositions() {
		return hitPositions;
	}

	//ゲーム中か否か
	public bool isPlaying() {
		return playing;
	}
	

	// クリックまたはD/Fキー入力処理
	public void Shot(Vector3 shotPosition) {
		if(!playing) return;
		shots++;
		RaycastHit2D rayHit = Physics2D.Raycast(shotPosition, new Vector3(0, 0, 1), 100);
		if(rayHit.collider == null) return;

		GameObject hitObject = rayHit.collider.transform.parent.gameObject;

		hitObject.GetComponent<Target>().Hit();
		hitPositions.Add(hitObject.transform.InverseTransformPoint(rayHit.point));

		Hit();
	}

	//オートモード時の自動ターゲットヒット
	public void Hit() {
		hitSound.Play();
		score++;
	}

	//的をクリックし損ねた時の処理
	public void miss() {
		health--;
		GameUI.Instance.UpdateHealth();
		if(health == 0) GameOver();
	}

	//音ゲーモード切り替え
	public void changeMode(bool otogeMode) {
		if(playing) return;

		Destroy(GetComponent<AbstractGameMode>());

		if(otogeMode) gameMode = gameObject.AddComponent(typeof(OtogeMode)) as IGameMode;
		else gameMode = gameObject.AddComponent(typeof(NormalMode)) as IGameMode;
	}

	//オートモードを設定
	public void changeAutoMode(bool autoMode) {
		gameMode.SetAutoMode(autoMode);
	}

	//ゲーム中断
	public void Interrupt() {
		if(!playing) return;
		pausing = false;
		playing = false;
		gameMode.Interrupt();
		GameUI.Instance.Hide();
		MainMenuUI.Instance.Display();
	}
	
	//ポーズ
	public void Pause() {
		if(pausing) {
			Timer.Instance.Begin();
			gameMode.Play();
		} else {
			Timer.Instance.Pause();
			gameMode.Pause();
		}
		pausing = !pausing;
	}

	//ポーズ中であるか否か
	public bool isPausing() {
		return pausing;
	}

	//MenuUIからの曲変更イベント
	public void SongChanged() {
		gameMode.SongChange();
	}

	//ゲーム開始
	public void GameStart() {
		gameInitialize();
		GameUI.Instance.UpdateHealth();
		Timer.Instance.Begin();
		playing = true;
	}

	
	// ゲーム終了処理
	public void GameOver() {
		playing = false;
		gameMode.GameOver();

		GameUI.Instance.Hide();
		ResultUI.Instance.DisplayResult();
		MainMenuUI.Instance.Display();
	}

	//ゲーム起動時にユーザー設定を初期化
	private void SettingInitialize() {
		changeMode(PlayerPrefs.GetInt(Consts.OTOGE_KEY, 0) == 1);
	}

	//ゲーム毎の変数を初期化
	private void gameInitialize() {
		gameMode.Initialize();
		Timer.Instance.Stop();
		ResultUI.Instance.Initialize();
		GameUI.Instance.Display();

		score = 0;
		shots = 0;
	}

}