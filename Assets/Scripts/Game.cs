using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using UnityEngine.UI;

///
/// ゲーム全体のスクリプト。Canvasにアタッチ。
///
public class Game : SingletonMonoBehaviour<Game> {
	public static Game gameInstance = null;

	//prefabs
	///的のプレハブ用変数
	public GameObject targetPrefab;
	// ガイドラインの丸のプレハブ(UnityEditorからのセット用)
	public GameObject guideLinePrefab;
	// ガイドラインの丸のプレハブ(外部スクリプトから読み込むため上記のPrefabをこちらにセットしなおしている)
	public static GameObject guideLineNodePrefab;
	//game statuses
	///TargetをHitした回数
	private int score;
	///HP。Targetを逃すと1減る。
	private int health;
	//クリックした回数
	private int shots;
	// 的作成のタイミング
	private List<float> timingList = new List<float>();
	// タイミングリストの先頭からいくつ呼んだか
	private int otogeCount = 0;
	///前回の的出現からの経過時間
	private float seconds;
	//game options
	///的が出現する間隔
	public float interval = 1f;
	//本来推すべき時間からこの秒数を引いた時間にターゲットを生成する。
	private float otogeInterval = 1.6f;
	///インスタンス化した的のリスト。
	private List<GameObject> targets = new List<GameObject>();
	/// インスタンス化した的オブジェクトの一時用変数(Update内で新規変数宣言をしたくなかったため用意)
	private GameObject newTarget;
	//射撃時の音
	private AudioSource hitSound;
	//タイミングクリエーター
	private TimingCreater tc;
	// ゲーム中に射撃したターゲットの相対位置リスト
	private List<Vector3> hitPositions = new List<Vector3>();
	//リザルトが麺に表示するターゲット
	private GameObject resultTarget;
	//前に生成したターゲット
	private GameObject previousTarget = null;
	// 前に生成したターゲットのワールド位置
	private Vector3 previousPos;
	private SpriteRenderer background;
	private IContentsPlayer contentsPlayer;
	///スタートボタンクリックフラグ（Sceneを分けたくなかったため用意)
	private bool playing = false;
	// 最初のターゲットを生成したか
	private bool firstPosSetted = false;
	// 音ゲーモードか否か
	private bool otogeMode = false;
	// オートモードか否か
	private bool autoMode = false;
	// ポーズ中か否か
	private bool pausing = false;
	private float playInterval = 0;

	void Start () {
		hitSound = GameObject.Find("Main Camera").GetComponent<AudioSource>();
		tc = GameObject.Find("Video Player").GetComponent<TimingCreater>();
		Game.gameInstance = this;
		background = GameObject.Find("BackgroundImage").GetComponent<SpriteRenderer>();
	}

	void Update () {
		if(!playing || pausing) return;
		if(Timer.Instance.GetTime() >= contentsPlayer.GetPlayTime() + playInterval) GameOver();
		if(!contentsPlayer.IsPlaying() && otogeMode) PlayVideoOrAudio(); 

		if(!otogeMode) {
			generateTarget();
		} else if(otogeCount < timingList.Count){
			if(Timer.Instance.GetTime() >= timingList[otogeCount] - otogeInterval) {
				Target target = generateTarget();
				if(autoMode && target != null) target.changeAutoMode();
				otogeCount++;
			} 
		}
	}

	private void PlayVideoOrAudio() {
		if(!playing || Timer.Instance.GetTime() <= playInterval) return;
		contentsPlayer.Play();
	}

	// クリックまたはD/Fキー入力処理
	public void Shot(Vector3 shotPosition) {
		if(!playing) return;
		shots++;
		RaycastHit2D hit = Physics2D.Raycast(shotPosition, new Vector3(0, 0, 1), 100);
		if(hit.collider == null) return;
		hitTarget(hit);
	}

	///
	/// ゲーム中に使う変数群を初期化
	///
	private void gameInitialize() {
		Timer.Instance.Stop();
		ResultUI.Instance.Initialize();
		if(resultTarget != null) {
			Destroy(resultTarget);
			resultTarget = null;
		}
		score = 0;
		if(otogeMode) health = 300;
		else health = 3;
		interval = 1;
		shots = 0;
		otogeCount = 0;
		GameUI.Instance.Display();
	}

	///
	/// 的を生成する処理
	///
	private Target generateTarget() {
		if(!playing) return null;
		if(!otogeMode) {
			seconds += Time.deltaTime;
			if(seconds < interval) return null;
		}

		newTarget = Instantiate(targetPrefab, getRandomPos(), Quaternion.identity);
		if(otogeMode && previousTarget != null) {
			generateGuideLine(previousTarget, newTarget);
		}
		previousTarget = newTarget;
		targets.Add(newTarget);
		if(score % 4 == 0 && interval >= 0.4f)  {
			interval -= 0.1f;
		}
		seconds = 0;
		return newTarget.GetComponent<Target>();
	}

	//的間で光るガイドラインを生成
	//TODO: まともに見えるようにする
	private void generateGuideLine(GameObject previousTarget, GameObject newTarget) {
		if(Game.guideLineNodePrefab == null) Game.guideLineNodePrefab = guideLinePrefab;
		Vector3 prePos = previousTarget.transform.position;
		Vector3 newPos = newTarget.transform.position;
		float dis = Vector3.Distance(prePos, newPos);
		int numGuide = (int)(dis / 0.5f);
		float currentTiming = timingList[otogeCount - 1];
		float nextTiming = timingList[otogeCount];
		float interval = nextTiming - currentTiming;
		interval /= numGuide;
		float t = (float)1 / numGuide;
		Vector3 guidePos = Vector3.Lerp(prePos, newPos, t);
		Instantiate(guideLineNodePrefab, guidePos, Quaternion.identity).GetComponent<GuidelineNode>().setNodeInfo(interval, prePos, newPos, numGuide, 1, true, currentTiming, Timer.Instance.GetTime());
	}

	// 的を生成する場所を生成
	private Vector3 getRandomPos() {
		Vector3 newPos;
		int width = Consts.WIDTH, height = Consts.HEIGHT;
		if(!otogeMode || !firstPosSetted) {
			firstPosSetted = true;
			newPos = new Vector3(UnityEngine.Random.Range(-width, width), UnityEngine.Random.Range(-height, height), 0);
		} else {
			float interval = width / 2;
			float widthMinus = previousPos.x - interval > -width ? previousPos.x - interval : -width;
			float widthPlus = previousPos.x + interval  < width ? previousPos.x + interval : width;
			float randX = UnityEngine.Random.Range(widthMinus, widthPlus);
			if(randX > previousPos.x - 1 && randX < previousPos.x + 1) {
				randX = randX - 1.5f < -interval ? randX + 3 : randX - 1.5f;
			} 

			interval = height / 2;
			float heightMinus = previousPos.y - interval  > -height ? previousPos.y - interval : -height;
			float heightPlus = previousPos.y + interval < height ? previousPos.y + interval : height;
			float randY = UnityEngine.Random.Range(heightMinus, heightPlus);
			if(randY > previousPos.y - 1 && randY < previousPos.y + 1) {
				randY = randY - 1.5f < -interval ? randY + 3 : randY - 1.5f;
			}
			newPos = new Vector3(randX, randY, 0);
		}
		previousPos = newPos;
		return newPos;
	}

	// 的をクリックしたときの処理
	private void hitTarget(RaycastHit2D hit) {
		GameObject target = hit.collider.transform.parent.gameObject;
		Vector3 hitPosition = target.transform.InverseTransformPoint(hit.point);
		hitPositions.Add(hitPosition);
		Destroy(target);
		hitSound.Play();
		score++;
	}

	// ゲーム終了処理
	private void GameOver() {
		playing = false;
		foreach(GameObject target in targets) {
			Destroy(target);
		}
		GameUI.Instance.Hide();
		ResultUI.Instance.DisplayResult();
		MainMenuUI.Instance.Display();
	}
	
	///
	/// AudoMode時のターゲット自動クリック処理
	///
	public void autoTargetHit(GameObject target) {
		Destroy(target);
		hitSound.Play();
		score++;
	}

	///
	/// 的をクリックし損ねた時の処理(Targetから呼び出し)
	///
	public void miss() {
		health--;
		GameUI.Instance.UpdateHealth();
		if(health == 0) GameOver();
	}

	
	///
	/// 音ゲーモード切替
	///
	public void changeMode(bool otogeMode) {
		if(playing) return;
		if(otogeMode) LoadSong();
		this.otogeMode = otogeMode;
	}

	///
	/// オートモード切替
	///
	public void changeAutoMode(bool autoMode) {
		if(!otogeMode || playing) return;
		this.autoMode = autoMode;
	}

	///
	/// ゲーム中か否か
	///
	public bool isPlaying() {
		return playing;
	}

	//ゲーム中断
	public void Interrupt() {
		StopSounds();
		pausing = false;
		playing = false;
		foreach(GameObject target in targets) {
			Destroy(target);
		}
		GameUI.Instance.Hide();
		MainMenuUI.Instance.Display();
	}
	
	private void StopSounds() {
		contentsPlayer.Stop();
	}
	
	///
	/// ポーズする。
	///
	public void Pause() {
		if(pausing) {
			pausing = false;
			Timer.Instance.Begin();
			contentsPlayer.Play();
		} else {
			pausing = true;
			Timer.Instance.Pause();
			contentsPlayer.Pause();
		}
	}

	///
	/// ポーズ中か否か
	///
	public bool isPausing() {
		return pausing;
	}

	//音ゲーモードの譜面を読み込む
	public void LoadSong() {
		//色々初期化
		playInterval = 0;
		background.sprite = null;
		timingList = new List<float>();

		//楽曲のパス取得
		string path = PlayerPrefs.GetString(Consts.SONG_KEY, "");
		if(path == "") return;

		//動画を優先してセット
		//動画が無ければ曲読み込み
		string moviePath = path + "\\" + ExtendSongs.MOVIE_FILE;
		if(File.Exists(moviePath)) contentsPlayer = VideoContentsPlayer.Instance;
		else contentsPlayer = AudioContentsPlayer.Instance;

		contentsPlayer.Load(path);

		//譜面読み込み
		timingList = ChartLoader.Load(path + "\\chart.txt");

		if(timingList.Count > 0 && timingList[0] < 2.0f) {
			playInterval = 2.0f;
			for(int i = 0; i < timingList.Count; i++) {
				timingList[i] += 2.0f;
			}
		}
		
		//背景設定
		if(!File.Exists(moviePath) && File.Exists(path + "\\" + ExtendSongs.BACK_FILE)) {
			background.sprite = ExtendSongs.SpriteFromFile(path + "\\" + ExtendSongs.BACK_FILE);
		}
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
}
