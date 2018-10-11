using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractGameMode : MonoBehaviour, IGameMode {
	protected IChart chart;

	public abstract void Play();
	public abstract void Pause();
	public abstract void GameProcess();
	public abstract void SongChange();
	public abstract void Interrupt();


	public void SetAutoMode(bool auto) {
		chart.SetAutoMode(auto);
	}

	

	public void Initialize() {
		chart.Initialize();
	}

	public void GameOver() {
		chart.DeleteAllTargets();
	}
}
