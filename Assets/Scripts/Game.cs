using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using UnityEngine.Video;

///
/// ゲーム全体のスクリプト。Canvasにアタッチ。
///
public class Game : SingletonMonoBehaviour<Game> {
	public static Game gameInstance = null;
	///TargetをHitした回数
	private int score;
	///HP。Targetを逃すと1減る。
	private int health;
	//クリックした回数
	private int shots;
	///的のプレハブ用変数
	public GameObject targetPrefab;
	///前回の的出現からの経過時間
	private float seconds;
	///的が出現する間隔
	public float interval = 1f;
	///スタートボタンクリックフラグ（Sceneを分けたくなかったため用意)
	private bool playing = false;
	///インスタンス化した的のリスト。
	private List<GameObject> targets = new List<GameObject>();
	/// インスタンス化した的オブジェクトの一時用変数(Update内で新規変数宣言をしたくなかったため用意)
	private GameObject newTarget;
	//射撃時の音
	private AudioSource hitSound;
	//ビデオプレーヤー
	private VideoPlayer videoPlayer;
	//タイミングクリエーター
	private TimingCreater tc;
	// ゲーム中に射撃したターゲットの相対位置リスト
	private List<Vector3> hitPositions = new List<Vector3>();
	//リザルトが麺に表示するターゲット
	private GameObject resultTarget;
	//メニュー
	private GameObject menuCanvas;
	// ガイドラインの丸のプレハブ(UnityEditorからのセット用)
	public GameObject guideLinePrefab;
	// ガイドラインの丸のプレハブ(外部スクリプトから読み込むため上記のPrefabをこちらにセットしなおしている)
	public static GameObject guideLineNodePrefab;
	// 音ゲー用の動画を再生したかどうか
	private bool movieStarted = false;
	//本来推すべき時間からこの秒数を引いた時間にターゲットを生成する。
	private float otogeInterval = 1.6f;
	//前に生成したターゲット
	private GameObject previousTarget = null;
	// 前に生成したターゲットのワールド位置
	private Vector3 previousPos;
	// 最初のターゲットを生成したか
	private bool firstPosSetted = false;
	// 的作成のタイミング
	private List<float> timingList = new List<float>();
	// 音ゲーモードか否か
	private bool otogeMode = false;
	// タイミングリストの先頭からいくつ呼んだか
	private int otogeCount = 0;
	// オートモードか否か
	private bool autoMode = false;
	// ポーズ中か否か
	private bool pausing = false;
	private AudioSource audioSource;
	
	private SpriteRenderer background;

	

	///
	/// ゲーム起動時に一度だけ呼ばれる処理.
	/// GameObject変数を初期化しておく
	///
	void Start () {
		hitSound = GameObject.Find("Main Camera").GetComponent<AudioSource>();
		videoPlayer = GameObject.Find("Video Player").GetComponent<VideoPlayer>();
		tc = GameObject.Find("Video Player").GetComponent<TimingCreater>();
		audioSource = GameObject.Find("Audio Source").GetComponent<AudioSource>();
		Game.gameInstance = this;
		menuCanvas = GameObject.Find("OptionsMenuCanvas");
		background = GameObject.Find("BackgroundImage").GetComponent<SpriteRenderer>();
	}

	private string currentMoviePath = "";

	int soundCount = 0;

	///
	/// 毎フレーム行う処理
	///
	void Update () {
		if(!playing || pausing) return;
		if(movieStarted && !(videoPlayer.isPlaying || audioSource.isPlaying) && Timer.Instance.GetTime() > 100f) {
			miss();
		} else if(!(videoPlayer.isPlaying || audioSource.isPlaying)&& otogeMode) {
			PlayVideoOrAudio();
		}
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
	private float playInterval = 0;
	private void PlayVideoOrAudio() {
		if(Timer.Instance.GetTime() <= playInterval) return;
		if(File.Exists(currentMoviePath)) videoPlayer.Play();
		else audioSource.Play();
		movieStarted = true;
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
		soundCount = 0;
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
		//Debug.Log(newTarget.gameObject.order)
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

	///
	/// ガイドラインを生成する
	///
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

	///
	/// 的を出現させるランダム位置を作成
	///
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

	

	///
	/// 的をクリックした時の処理
	///
	private void hitTarget(RaycastHit2D hit) {
		GameObject target = hit.collider.transform.parent.gameObject;
		Vector3 hitPosition = target.transform.InverseTransformPoint(hit.point);
		hitPositions.Add(hitPosition);
		Destroy(target);
		hitSound.Play();
		score++;
	}


	///
	/// Public Methods
	///
	
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
		if(health == 0) {
			playing = false;
			foreach(GameObject target in targets) {
				Destroy(target);
			}
			GameUI.Instance.Hide();
			ResultUI.Instance.DisplayResult();
			MainMenuUI.Instance.Display();
		}
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
		if(File.Exists(currentMoviePath)) videoPlayer.Stop();
		else audioSource.Stop();
	}
	
	///
	/// ポーズする。
	///
	public void Pause() {
		if(pausing) {
			pausing = false;
			Timer.Instance.Begin();
			if(File.Exists(currentMoviePath) && playing && !videoPlayer.isPlaying) videoPlayer.Play();
			else if(playing && !audioSource.isPlaying) audioSource.Play();
		} else {
			pausing = true;
			Timer.Instance.Pause();
			if(videoPlayer.isPlaying) videoPlayer.Pause();
			if(audioSource.isPlaying) audioSource.Pause();
		}
	}

	///
	/// ポーズ中か否か
	///
	public bool isPausing() {
		return pausing;
	}

	private float contentTime = 0;

	///
	///音ゲーモードの譜面を読み込む
	///
	public void LoadSong() {
		//色々初期化
		audioSource.clip = null;
		background.sprite = null;
		timingList = new List<float>();

		//楽曲のパス取得
		string path = PlayerPrefs.GetString(Consts.SONG_KEY, "");
		if(path == "") return;

		//動画を優先してセット
		//動画が無ければ曲読み込み
		string moviePath = path + "\\" + ExtendSongs.MOVIE_FILE;
		if(File.Exists(moviePath)) {
			videoPlayer.url = moviePath;
			Debug.Log("VideoTime:" +  videoPlayer.timeReference);
		} else {
			StartCoroutine(ExtendSongs.SetAudioSource(path));
			Debug.Log("Audio time: " + audioSource.time);
		} 
		
		//譜面読み込み
		FileInfo fi = new FileInfo(path + "\\chart.txt");
		string line = "";
		bool hasInterval = false;
		try {
			bool osu = false, timingPoint = false;
			using(StreamReader sr = new StreamReader(fi.OpenRead(), Encoding.UTF8)) {
				while((line = sr.ReadLine()) != null) {
					if(line.StartsWith("osu")) {
						osu = true;
					}
					if(!osu) {
						timingList.Add(float.Parse(line));
					} else {
						if(timingPoint) {
							string[] datas = line.Split(',');
							float timing = float.Parse(datas[2]) / 1000;
							if(!hasInterval && timing <= 3.0f) hasInterval = true;
							if(hasInterval) timing += 3.0f;
							timingList.Add(timing);
						} else {
							if(line.StartsWith("[HitObjects]")) timingPoint = true;
						}
					}
					
				}
			}
			if(hasInterval) playInterval = 3.0f;
			currentMoviePath = moviePath;
		}catch (Exception e) {
			Debug.Log(e);
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
