using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalMode : AbstractGameMode {

	// Use this for initialization
	void Start () {
		chart = gameObject.AddComponent(typeof(NormalChart)) as IChart;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public override void SongChange(){}
	public override void Pause() {

	}
	public override void Play() {

	}

	public override void GameProcess() {
		chart.ChartProcess();
	}

	public override void Interrupt() {
		chart.DeleteAllTargets();
	}
}

