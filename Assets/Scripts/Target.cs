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
	/// 的の大きさが変化する速度
	private float speed = 0.03f;

	///的生成時に一度のみ呼ばれる処理。
	///GameObjectを初期化する。
	void Start () {
		Vector3 initialScale = new Vector3();
		initialScale.x = 0;
		initialScale.y = 0;
		transform.localScale = initialScale;
		foreach(Transform child in transform) {
			if(child.name == "High") high = child.gameObject;
			else if(child.name == "Midium") midium = child.gameObject;
			else if(child.name == "Low") low = child.gameObject;
		}

		Vector2 highSize = new Vector2();
		Vector2 midiumSize = new Vector2();
		Vector2 lowSize = new Vector2();

		float scale = 150;

		highSize.x = GetComponent<RectTransform>().sizeDelta.x / 10 * 2 / scale;
		highSize.y = GetComponent<RectTransform>().sizeDelta.y / 10 * 2 / scale;

		midiumSize.x = GetComponent<RectTransform>().sizeDelta.x / 10 * 6 / scale;
		midiumSize.y = GetComponent<RectTransform>().sizeDelta.y / 10 * 6 / scale;

		lowSize.x = GetComponent<RectTransform>().sizeDelta.x / 10 * 10 / scale;
		lowSize.y = GetComponent<RectTransform>().sizeDelta.y / 10 * 10 / scale;

		high.transform.localScale = highSize;
		midium.transform.localScale = midiumSize;
		low.transform.localScale = lowSize;
	}

	/// 毎フレーム呼ばれる的への処理。
	/// おおきさが変わったりする
	void Update () {
		Vector3 ls = transform.localScale;
		if(isMaxSized) {
			ls.x -= speed;
			ls.y -= speed;
		} else {
			ls.x += speed;
			ls.y += speed;
		}
		
		transform.localScale = ls;
		if(ls.x >= 2f) isMaxSized = true;
		else if(ls.x < 0.0f) {
			Destroy(high.transform.parent.gameObject);
			Game.gameInstance.miss();
		}
	}
}
