using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;

public class TimingCreater : MonoBehaviour {
	private float timer = 0;
	private List<float> timingList = new List<float>();
	private bool start = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(!start) return;
		timer += Time.deltaTime;
		if(Input.GetKeyDown(KeyCode.T)) {
			timingList.Add(timer);
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
		fi = new FileInfo(Application.dataPath + "/givemeegg.csv");
		sw = fi.AppendText();
		foreach(float timing in timingList) {
			sw.WriteLine(timing);
			sw.Flush();
		}
		sw.Close();

	}
}
