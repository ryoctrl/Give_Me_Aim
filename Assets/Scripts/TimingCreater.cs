using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;

public class TimingCreater : MonoBehaviour {
	private float timer = 0;
	private List<float> timingList = new List<float>();
	private bool start = false;

	private AudioSource createSound;

	// Use this for initialization
	void Start () {
		createSound = GameObject.Find("Main Camera").GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
		if(!start || Game.gameInstance.isPausing()) return;
		timer += Time.deltaTime;
		if(Input.GetKeyDown(KeyCode.T)) {
			timingList.Add(timer);
			createSound.Play();
		}
		if(Input.GetKeyDown(KeyCode.Q)) {
			Save();
		}
	}

	public void startTiming() {
		this.start = true;
	}

	private void Save() {
		StreamWriter sw;
		FileInfo fi;
		fi = new FileInfo(PlayerPrefs.GetString(Menu.SONG_KEY) + "/chart.txt");
		sw = fi.AppendText();
		foreach(float timing in timingList) {
			sw.WriteLine(timing);
			sw.Flush();
		}
		sw.Close();

	}
}
