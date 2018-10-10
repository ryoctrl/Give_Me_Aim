using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEditor;

public class OptionsMenuUI : SingletonMonoBehaviour<OptionsMenuUI> {
	private Slider volumeSlider;
	private Toggle otogeToggle;
	private Toggle autoToggle;
	private Dropdown songDropdown;
	private List<string> songsList;
	private AudioSource audioSource;
	private VideoPlayer video;
	private AudioSource hitSound;


	// Use this for initialization
	void Start () {
		volumeSlider = GameObject.Find("VolumeSlider").GetComponent<Slider>();
		video = GameObject.Find("Video Player").GetComponent<VideoPlayer>();
		audioSource = GameObject.Find("Audio Source").GetComponent<AudioSource>();
		hitSound = GameObject.Find("Main Camera").GetComponent<AudioSource>();
		otogeToggle = GameObject.Find("OtogeModeToggle").GetComponent<Toggle>();
		autoToggle = GameObject.Find("AutoModeToggle").GetComponent<Toggle>();
		songDropdown = GameObject.Find("SongDropdown").GetComponent<Dropdown>();
		allInitialize();
		transform.gameObject.SetActive(false);
	}

	private void allInitialize() {
		volumeSlider.value = PlayerPrefs.GetInt(Consts.VOLUME_KEY, 50);
		otogeToggle.isOn = PlayerPrefs.GetInt(Consts.OTOGE_KEY, 0) == 1;
		autoToggle.isOn = PlayerPrefs.GetInt(Consts.AUTO_KEY, 0) == 1;
		songDropdown.ClearOptions();
		songsList = ExtendSongs.GetSongs();
		foreach(var dir in songsList) {
			string[] paths = dir.Split('\\');
			songDropdown.options.Add(new Dropdown.OptionData{text = paths[paths.Length - 1]});
		}
		string song = PlayerPrefs.GetString(Consts.SONG_KEY, "");
		if(songsList.Contains(song)) songDropdown.value = songsList.IndexOf(song);
		else OnValueChangedSongDropdown(0);
		songDropdown.RefreshShownValue();
	}


	// 音量調節スライダの変更処理
	// TODO: AudioMixerを調査する
	public void OnValueChangedVolumeSlider() {
		video.SetDirectAudioVolume(0, volumeSlider.value / 100);
		audioSource.volume = volumeSlider.value / 100;
		hitSound.volume = volumeSlider.value / 100;
		PlayerPrefs.SetInt(Consts.VOLUME_KEY, (int)volumeSlider.value);
	}

	//音ゲーモードトグルの変更処理
	public void OnValueChangedOtogeModeToggle() {
		if(Game.gameInstance.isPlaying()) return;
		Game.gameInstance.changeMode(otogeToggle.isOn);
		autoToggle.interactable = otogeToggle.isOn;
		PlayerPrefs.SetInt(Consts.OTOGE_KEY, otogeToggle.isOn ? 1 : 0);
	}

	//オートモードトグルの変更処理
	public void OnValueChangedAutoModeToggle() {
		if(Game.gameInstance.isPlaying()) return;
		Game.gameInstance.changeAutoMode(autoToggle.isOn);
		PlayerPrefs.SetInt(Consts.AUTO_KEY, autoToggle.isOn ? 1 : 0);
	}

	// 曲目ドロップダウンの変更処理
	public void OnValueChangedSongDropdown(int index) {
		index = songDropdown.value;
		if(Game.gameInstance.isPlaying()) return;
		PlayerPrefs.SetString(Consts.SONG_KEY, songsList.Count > index ? songsList[index] : "");
		Game.gameInstance.LoadSong();
	}

	// Backボタンのクリック処理
	public void OnClickBackButton() {
		Hide();
		if(Game.gameInstance.isPausing()) Game.gameInstance.Pause();
	}

	// Topボタンのクリック処理
	public void OnClickTopButton() {
		Hide();
		Game.Instance.Interrupt();
	}

	// Exitボタンのクリック処理
	public void OnClickExitButton() {
		#if UNITY_EDITOR
			EditorApplication.isPlaying = false;
		#elif UNITY_STANDALONE
			Application.Quit();
		#endif
	}

	//オプションメニューを非表示にする
	public void Hide() {
		gameObject.SetActive(false);
	}

	// オプションメニューを表示する
	public void Display() {
		if(!gameObject.activeSelf) gameObject.SetActive(true);
	}
}
