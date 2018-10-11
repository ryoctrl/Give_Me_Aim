using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalChart : AbstractChart {
	private float interval = 1f;
	private float elapsed = 0f;

	protected override Vector3 GetRandomPosition() {
		int width = Consts.WIDTH, height = Consts.HEIGHT;
		return new Vector3(Random.Range(-width, width), Random.Range(-height, height), 0);
	}

	public override void SetAutoMode(bool auto){}

	public override void ChartProcess() {
		if(!GameManager.Instance.isPlaying()) return;

		elapsed += Time.deltaTime;
		if(elapsed < interval) return;

		newTarget = GenerateTarget();

		targets.Add(newTarget);

		if(GameManager.Instance.GetScore() % 4 == 0 && interval >= 0.4f) interval -= 0.1f;
		elapsed = 0;
		
		return;
	}

	public override void Initialize() {
		GameManager.Instance.SetHealth(3);
		interval = 1f;
		elapsed = 0f;
	}
}
