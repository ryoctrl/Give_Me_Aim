using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioContentsPlayer : SingletonMonoBehaviour<AudioContentsPlayer>, IContentsPlayer {
	private AudioSource audioSource;
	private float playTime;

	void Start () {
		audioSource = GameObject.Find("Audio Source").GetComponent<AudioSource>();
	}

	public void Play() {
		audioSource.Play();
	}

	public void Pause() {
		audioSource.Pause();
	} 

	public void Stop() {
		audioSource.Stop();
	}

	public void Load(string path) {
		StartCoroutine(ExtendSongs.SetAudioSource(path, () => {
			playTime = audioSource.clip.length;
		}));
	}

	public float GetPlayTime() {
		return playTime;
	}

	public bool IsPlaying() {
		return audioSource.isPlaying;
	}
}
