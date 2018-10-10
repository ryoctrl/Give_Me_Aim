using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : SingletonMonoBehaviour<Timer> {

	private float timer;

	private bool beginning = false;

	// Use this for initialization
	void Start () {
		timer = 0f;
	}
	
	// Update is called once per frame
	void Update () {
		if(!beginning) return;
		timer += Time.deltaTime;
	}

	public void Stop() {
		beginning = false;
		timer = 0f;
	}


	public void Pause() {
		beginning = false;
	}

	public void Begin() {
		beginning = true;
	}

	public float GetTime() {
		return timer;
	}
}
