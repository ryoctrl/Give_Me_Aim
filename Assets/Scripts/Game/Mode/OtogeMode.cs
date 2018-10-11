using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using UnityEngine.UI;

public class OtogeMode : AbstractGameMode {
	//動画または音声
	private IContentsPlayer contentsPlayer;
	private float playInterval;
	//背景
	private SpriteRenderer background;
	private bool ready = false;

	// Use this for initialization
	void Start () {
		chart = gameObject.AddComponent(typeof(OtogeChart)) as IChart;
		background = GameObject.Find("BackgroundImage").GetComponent<SpriteRenderer>();
		Debug.Log("start");
		LoadSong();
		ready = true;
	}

	public override void GameProcess() {
		if(Timer.Instance.GetTime() >= contentsPlayer.GetPlayTime() + playInterval) GameOver();
		if(!contentsPlayer.IsPlaying()) PlayContents();

		chart.ChartProcess();
	}

	private void PlayContents() {
		if(!GameManager.Instance.isPlaying() || Timer.Instance.GetTime() <= playInterval) return;
		contentsPlayer.Play();
	}

	public override void Play() {
		PlayContents();
	}

	public override void Pause() {
		contentsPlayer.Pause();
	}

	public override void Interrupt() {
		chart.DeleteAllTargets();
		contentsPlayer.Stop();
	}

	public override void SongChange(){
		if(!ready) return;
		LoadSong();
	}

		//音ゲーモードの譜面を読み込む
	private void LoadSong() {
		//色々初期化
		playInterval = 0;
		background.sprite = null;

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
		List<float> timingList = ChartLoader.Load(path + "\\chart.txt");

		if(timingList.Count > 0 && timingList[0] < 2.0f) {
			playInterval = 2.0f;
			for(int i = 0; i < timingList.Count; i++) {
				timingList[i] += 2.0f;
			}
		}

		(chart as OtogeChart).SetTimingList(timingList);
		
		//背景設定
		if(!File.Exists(moviePath) && File.Exists(path + "\\" + ExtendSongs.BACK_FILE)) {
			background.sprite = ExtendSongs.SpriteFromFile(path + "\\" + ExtendSongs.BACK_FILE);
		}
	}
}
