using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoContentsPlayer : SingletonMonoBehaviour<VideoContentsPlayer>, IContentsPlayer{

	private VideoPlayer videoPlayer;
	private float playTime;
	
	void Start () {
		videoPlayer = GameObject.Find("Video Player").GetComponent<VideoPlayer>();
	}

	public void Play() {
		videoPlayer.Play();
	}

	public void Pause() {
		videoPlayer.Pause();
	} 

	public void Stop() {
		videoPlayer.Stop();
	}

	public void Load(string path) {
		string moviePath = path + "\\" + ExtendSongs.MOVIE_FILE;
		videoPlayer.url = moviePath;
		videoPlayer.prepareCompleted += (val) => {
			playTime = videoPlayer.frameCount/videoPlayer.frameRate;
		};
		videoPlayer.Prepare();
	}

	//TODO: implements
	public float GetPlayTime() {
		return playTime;
	}

	public bool IsPlaying() {
		return videoPlayer.isPlaying;
	}
}
