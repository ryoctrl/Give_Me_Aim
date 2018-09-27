using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

///
/// TargetのSpriteLayerを持つObjectにアタッチするスクリプト。
/// rayとの衝突を受け取りHit判定を行う。
///
public class SpriteClick : MonoBehaviour, IPointerClickHandler {

	///的をクリックした際に呼ばれる。
	public void OnPointerClick(PointerEventData eventData) {
		Destroy(transform.parent.gameObject);
		Game.gameInstance.hit();
		
	}
}
