using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;

public class TimingCreater : MonoBehaviour {
	// タイマー
	private float timer = 0;
	// 記録したタイミングのリスト
	private List<float> timingList = new List<float>();
	// タイミングを記録した際のサウンド
	private AudioSource createSound;

	void Start () {
		createSound = GameObject.Find("Main Camera").GetComponent<AudioSource>();
	}
	
	void Update () {
		if(!Game.gameInstance.isPlaying() || Game.gameInstance.isPausing()) {
			timer = 0;
			return;
		}
		timer += Time.deltaTime;
		if(Input.GetKeyDown(KeyCode.T)) {
			timingList.Add(timer);
			createSound.Play();
		}
		if(Input.GetKeyDown(KeyCode.Q)) {
			Save();
		}
	}

	/// 
	/// 記録したタイミングをファイルに書き出す。
	///
	private void Save() {
		StreamWriter sw;
		FileInfo fi;
		fi = new FileInfo(PlayerPrefs.GetString(Consts.SONG_KEY) + "/chart.txt");
		sw = fi.AppendText();
		foreach(float timing in timingList) {
			sw.WriteLine(timing);
			sw.Flush();
		}
		sw.Close();

	}
}
