using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ゲームプログラム全体にあたるメインのScript
public class GameManager : SingletonMonoBehaviour<GameManager> {
	public GameObject targetPrefab;
	public GameObject guideLinePrefab;
	public static GameObject guideLineNodePrefab;

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


	// クリックまたはD/Fキー入力処理
	public void Shot(Vector3 shotPosition) {
		if(!playing) return;
		shots++;
		RaycastHit2D hit = Physics2D.Raycast(shotPosition, new Vector3(0, 0, 1), 100);
		if(hit.collider == null) return;
		hitTarget(hit);
	}

	

	public void SetHealth(int health) {
		this.health = health;
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

		if(otogeMode) gameMode = gameObject.AddComponent(typeof(OtogeMode)) as IGameMode;
		else gameMode = gameObject.AddComponent(typeof(NormalMode)) as IGameMode;
	}

	//オートモードを設定
	public void changeAutoMode(bool autoMode) {
		gameMode.SetAutoMode(autoMode);
	}

	///
	/// ゲーム中か否か
	///
	public bool isPlaying() {
		return playing;
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

	public void GameStart() {
		gameInitialize();
		Timer.Instance.Begin();
		playing = true;
	}

	public int GetHealth() {
		return health;
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

	// ゲーム終了処理
	private void GameOver() {
		playing = false;
		gameMode.GameOver();

		GameUI.Instance.Hide();
		ResultUI.Instance.DisplayResult();
		MainMenuUI.Instance.Display();
	}

	// 的をクリックしたときの処理
	private void hitTarget(RaycastHit2D hit) {
		//当たった的
		GameObject target = hit.collider.transform.parent.gameObject;
		//当たった場所
		Vector3 hitPosition = target.transform.InverseTransformPoint(hit.point);
		hitPositions.Add(hitPosition);
		//Hit処理
		Destroy(target);
		Hit();
	}
}