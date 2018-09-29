using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class Menu : MonoBehaviour {

	private Slider volumeSlider;
	private VideoPlayer video;
	private AudioSource hitSound;


	// Use this for initialization
	void Start () {
		volumeSlider = GameObject.Find("Slider").GetComponent<Slider>();
		video = GameObject.Find("Video Player").GetComponent<VideoPlayer>();
		hitSound = GameObject.Find("Main Camera").GetComponent<AudioSource>();
		volumeSlider.value = 1;
		changeVolume();
	}


	
	// Update is called once per frame
	void Update () {

	}

	public void changeVolume() {
		Debug.Log(video.GetDirectAudioVolume(0));
		video.SetDirectAudioVolume(0, volumeSlider.value / 100);
		hitSound.volume = volumeSlider.value / 100;
		Debug.Log(video.GetDirectAudioVolume(0));
	}
}
