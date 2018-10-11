using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalMode : AbstractGameMode {

	void Start () {
		chart = gameObject.AddComponent(typeof(NormalChart)) as IChart;
	}

	public override void SongChange(){}
	public override void SetAutoMode(bool auto){}
	public override void Pause() {}
	public override void Play() {}

	public override void Initialize() {
		chart.Initialize();
		GameManager.Instance.SetHealth(3);
	}

	public override void GameProcess() {
		chart.ChartProcess();
	}

	public override void Interrupt() {
		chart.DeleteAllTargets();
	}

	public override void GameOver() {
		chart.DeleteAllTargets();
	}
}

