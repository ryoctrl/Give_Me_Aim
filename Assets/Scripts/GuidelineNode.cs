using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///
/// ガイドラインの点一つ一つの処理
///
public class GuidelineNode : MonoBehaviour {

	//生成されてからの時間
	private float timer;
	//生成されてから削除されるまでの時間
	private float destroyTime;
	private Vector3 prePos;
	private Vector3 newPos;
	private int numGuide;
	private int currentGuide;
	private bool isParent;
	private float startTime;
	private float now;
	private SpriteRenderer sr;

	// Use this for initialization
	void Start () {
		timer = 0;
		sr = GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
		if(isParent) now += Time.deltaTime;
		if(isParent && startTime > now) return;
		var c = sr.color;
		c.a = 255;
		sr.color = c;

		if(Game.gameInstance.isPausing()) return;
		timer += Time.deltaTime;
		if(timer > destroyTime || float.IsInfinity(destroyTime)) {
			InstansitateNewGuideAndDestroy();
		}
	}

	private void InstansitateNewGuideAndDestroy() {
		if(currentGuide < numGuide - 1) {
			int nextGuide = currentGuide + 1;
			float t = (float)1 / numGuide * nextGuide;
			Vector3 guidePos = Vector3.Lerp(prePos, newPos, t);
			Instantiate(Game.guideLineNodePrefab, guidePos, Quaternion.identity).GetComponent<GuidelineNode>().setNodeInfo(destroyTime, prePos, newPos, numGuide, nextGuide, false, 0, 0);
		}
		Destroy(transform.gameObject);
	}

	public void setNodeInfo(float destroyTime, Vector3 prePos, Vector3 newPos, int numGuide, int currentGuide, bool isParent, float startTime, float now) {
		this.destroyTime = destroyTime;
		this.prePos = prePos;
		this.newPos = newPos;
		this.numGuide = numGuide;
		this.currentGuide = currentGuide;
		this.isParent = isParent;
		this.startTime = startTime;
		this.now = now;
	}
}
