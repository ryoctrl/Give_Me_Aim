using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractChart : MonoBehaviour, IChart {
	protected GameObject newTarget;
	protected List<GameObject> targets = new List<GameObject>();


	public abstract void ChartProcess();
	public abstract void Initialize();
	public abstract void SetAutoMode(bool auto);
	protected abstract Vector3 GetRandomPosition();

	protected GameObject GenerateTarget() {
		return Instantiate(GameManager.Instance.targetPrefab, GetRandomPosition(), Quaternion.identity);
	}

	public void DeleteAllTargets() {
		foreach(GameObject obj in targets) Destroy(obj);
	}
}
