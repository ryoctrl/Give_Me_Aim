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
	public abstract void Initialize();
	public abstract void SetAutoMode(bool auto);
	public abstract void GameOver();
	
	void OnDestroy() {
		Destroy(GetComponent<AbstractChart>());
	}
}
