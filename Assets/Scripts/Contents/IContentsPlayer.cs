using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IContentsPlayer{
	//コンテンツ再生
	void Play();
	//コンテンツ一時停止
	void Pause();
	//コンテンツ停止
	void Stop();
	//コンテンツ読み込み
	void Load(string path);
	//コンテンツの長さ
	float GetPlayTime();
	//プレイ中フラグ
	bool IsPlaying();
}
