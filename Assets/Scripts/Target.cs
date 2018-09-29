using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

///
/// TargetPrefabにアタッチするスクリプト。
/// 一つ一つの的が行う処理。
///
public class Target : MonoBehaviour {
	///　的の中心の赤丸
	private GameObject high;
	/// 的の真ん中のオレンジ丸
	private GameObject midium;
	/// 的の外側の灰丸
	private GameObject low;
	/// 最大サイズに達して縮むべきか否かのフラグ
	private bool isMaxSized = false;
	//リザルト画面用のターゲットか否か
	private bool isResultTarget = false;
	public GameObject hitPrefab;
	private List<Vector3> hitPositions;
	private bool autoMode = false;
	/// 的の大きさが変化する速度
	private float speed = 0.0175f;

	//private float speed = 0.03f;
	
	///的生成時に一度のみ呼ばれる処理。
	///GameObjectを初期化する。
	void Start () {
		transform.localScale = new Vector3();
		foreach(Transform child in transform) {
			if(child.name == "High") high = child.gameObject;
			else if(child.name == "Midium") midium = child.gameObject;
			else if(child.name == "Low") low = child.gameObject;
		}

		Vector2 highSize = new Vector2();
		Vector2 midiumSize = new Vector2();
		Vector2 lowSize = new Vector2();

		RectTransform rt = GetComponent<RectTransform>();

		float scale = 100;

		highSize.x = rt.sizeDelta.x / 10 * 2 / scale;
		highSize.y = rt.sizeDelta.y / 10 * 2 / scale;

		midiumSize = highSize * 2;
		lowSize = highSize * 3;

		high.transform.localScale = highSize;
		midium.transform.localScale = midiumSize;
		low.transform.localScale = lowSize;
		if(isResultTarget) changeModeToResultTarget(hitPositions);
	}

	/// 毎フレーム呼ばれる的への処理。
	/// おおきさが変わったりする
	void Update () {
		if(isMaxSized && isResultTarget) return;

		Vector3 ls = transform.localScale;
		if(isMaxSized && !isResultTarget) {
			ls.x -= speed;
			ls.y -= speed;
		} else {
			ls.x += speed;
			ls.y += speed;
		}
		
		transform.localScale = ls;
		if(ls.x >= 3f) {
			if(autoMode && !isResultTarget) Game.gameInstance.autoTargetHit(this.transform.gameObject);
			isMaxSized = true;
		}
		else if(ls.x < 0.0f) {
			Destroy(high.transform.parent.gameObject);
			Game.gameInstance.miss();
		}
	}

	public void changeModeToResultTarget(List<Vector3> hitPositions) {
		isResultTarget = true;
		this.hitPositions = hitPositions;
		Vector3 ls = GetComponent<RectTransform>().localScale;
		ls.x += 4f;
		ls.y += 4f;
		GetComponent<RectTransform>().localScale = ls;
		Vector3 hitWorldPosition;
		GameObject hit;
		int orderInLayer = 4;
		foreach(Vector3 hitPosition in hitPositions) {
			hitWorldPosition = transform.TransformPoint(hitPosition);
			hit = Instantiate(hitPrefab, hitWorldPosition, Quaternion.identity);
			hit.GetComponent<SpriteRenderer>().sortingOrder = orderInLayer;
			hit.transform.parent = transform;
		}
	}

	public void changeAutoMode() {
		autoMode = true;
	}
}
