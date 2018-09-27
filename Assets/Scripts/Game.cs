using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

///
/// ゲーム全体のスクリプト。Canvasにアタッチ。
///
public class Game : MonoBehaviour {
	/// TargetやSpriteから処理を以上するためのStatic変数。
	public static Game gameInstance = null;
	private const int width = 8;
	private const int height = 4;
	///TargetをHitした回数
	private int score;
	///HP。Targetを逃すと1減る。
	private int health;
	//クリックした回数
	private int shots;
	///タイマー。
	private float timer;
	/// タイマー表示用のTextField
	private Text timerText;
	/// health表示用のTextField
	private Text healthText;
	///的のプレハブ用変数
	public GameObject targetPrefab;
	///前回の的出現からの経過時間
	private float seconds;
	///的が出現する間隔
	public float interval = 1f;
	///スタートボタンクリックフラグ（Sceneを分けたくなかったため用意)
	private bool started = false;
	///インスタンス化した的のリスト。
	private List<GameObject> targets = new List<GameObject>();
	///スタートボタンobject
	private Button startButton;
	///リザルト表示用TextObjects（Sceneを分けたくなかったため用意)
	private Text totalTimeText;
	private Text scoreText;
	private Text accuracyText;
	/// インスタンス化した的オブジェクトの一時用変数(Update内で新規変数宣言をしたくなかったため用意)
	private GameObject newTarget;

	///
	/// ゲーム起動時に一度だけ呼ばれる処理.
	/// GameObject変数を初期化しておく
	///
	void Start () {
		timerText = GameObject.Find("TimerText").gameObject.GetComponent<Text>();
		startButton = GameObject.Find("StartButton").GetComponent<Button>();
		totalTimeText = GameObject.Find("TotalTime").GetComponent<Text>();
		scoreText = GameObject.Find("ScoreText").GetComponent<Text>();
		healthText = GameObject.Find("HealthText").GetComponent<Text>();
		accuracyText = GameObject.Find("AccuracyText").GetComponent<Text>();
		Game.gameInstance = this;
	}

	///
	/// 毎フレーム行う処理
	///
	void Update () {
		if(Input.GetKeyDown(KeyCode.Space)) Debug.Break();
		mouseProcess();
		if(!started) return;
		generateTarget();
		updateTimer();
		
	}

	private void mouseProcess() {
		if(Input.GetMouseButtonDown(0)){
			shots++;
			Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			RaycastHit2D hit = Physics2D.Raycast(mousePos, new Vector3(0, 0, 1), 100);
			if(hit.collider == null) return;
			GameObject target = hit.collider.transform.parent.gameObject;
			hitTarget(target);
		}
	}

	///
	/// ゲーム中に使う変数群を初期化
	///
	private void gameInitialize() {
		score = 0;
		health = 3;
		timer = 0;
		interval = 1;
		shots = 0;
		timerText.enabled = true;
		healthText.enabled = true;
		totalTimeText.text = "";
		scoreText.text = "";
		accuracyText.text = "";
		healthText.text = generateHealthText();
	}

	private string generateHealthText() {
		string healthText = "";
		for(int i = 0; i < health; i++) healthText += "☆";
		return "Health " + healthText;
		
	}

	///
	/// 的を生成する処理
	///
	private void generateTarget() {
		if(!started) return;
		seconds += Time.deltaTime;
		if(seconds < interval) return;

		newTarget = Instantiate(targetPrefab, getRandomPos(), Quaternion.identity);
		targets.Add(newTarget);
		if(score % 4 == 0 && interval >= 0.4f)  {
			interval -= 0.1f;
		}
		seconds = 0;
	}

	///
	/// タイマーの表示を更新
	///
	private void updateTimer() {
		if(!started) return;
		
		timer += Time.deltaTime;
		timerText.text = timer.ToString("F2");
	}

	///
	/// 的を出現させるランダム位置を作成
	///
	private Vector3 getRandomPos() {
		return new Vector3(Random.Range(-width, width), Random.Range(-height, height), 0);
	}

	///
	/// 的をクリックし損ねた時の処理(Targetから呼び出し)
	///
	public void miss() {
		health--;
		healthText.text = generateHealthText();
		if(health == 0) {
			started = false;
			foreach(GameObject target in targets) {
				Destroy(target);
			}
			timerText.enabled = false;
			healthText.enabled = false;
			accuracyText.text = "Accuracy: " + ((double)score / shots * 100).ToString("F1") + "%";
			totalTimeText.text = "Tortal time: " + timer.ToString("F2");
			scoreText.text = "Targets hit: " + score.ToString();
			startButton.gameObject.SetActive(true);
		}
	}

	///
	/// 的をクリックした時の処理
	///
	private void hitTarget(GameObject target) {
		Destroy(target);
		score++;
	}

	///
	/// Startボタンを押したときの処理
	///
	public void pushButton() {
		gameInitialize();
		started = true;
		startButton.gameObject.SetActive(false);
	}
}
