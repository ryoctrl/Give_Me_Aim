using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEditor;

public class Menu : MonoBehaviour {
	public const string VOLUME_KEY = "volume";
	public const string OTOGE_KEY = "otoge";
	public const string AUTO_KEY = "auto";
	public const string CLICK_KEYS = "click";
	public const string SONG_KEY = "song";

	private Slider volumeSlider;
	private VideoPlayer video;
	private AudioSource hitSound;
	private Toggle otogeToggle;
	private Toggle autoToggle;
	private InputField clickInput;
	private Dropdown songDropdown;
	private List<string> songsList;
	private AudioSource audioSource;
	private Game game = Game.gameInstance;


	// Use this for initialization
	void Start () {
		volumeSlider = GameObject.Find("Slider").GetComponent<Slider>();
		video = GameObject.Find("Video Player").GetComponent<VideoPlayer>();
		audioSource = GameObject.Find("Audio Source").GetComponent<AudioSource>();
		hitSound = GameObject.Find("Main Camera").GetComponent<AudioSource>();
		otogeToggle = GameObject.Find("OtogeModeToggle").GetComponent<Toggle>();
		autoToggle = GameObject.Find("AutoModeToggle").GetComponent<Toggle>();
		clickInput = GameObject.Find("ClickKeyInput").GetComponent<InputField>();
		songDropdown = GameObject.Find("SongDropdown").GetComponent<Dropdown>();
		allInitialize();
		transform.gameObject.SetActive(false);
	}

	private void allInitialize() {
		volumeSlider.value = PlayerPrefs.GetInt(VOLUME_KEY, 50);
		otogeToggle.isOn = PlayerPrefs.GetInt(OTOGE_KEY, 0) == 1;
		autoToggle.isOn = PlayerPrefs.GetInt(AUTO_KEY, 0) == 1;
		clickInput.text = PlayerPrefs.GetString(CLICK_KEYS, "");
		songDropdown.ClearOptions();
		songsList = ExtendSongs.GetSongs();
		foreach(var dir in songsList) {
			string[] paths = dir.Split('\\');
			songDropdown.options.Add(new Dropdown.OptionData{text = paths[paths.Length - 1]});
		}
		string song = PlayerPrefs.GetString(SONG_KEY, "");
		if(songsList.Contains(song)) songDropdown.value = songsList.IndexOf(song);
		else onSongChanged(0);
		songDropdown.RefreshShownValue();
	}

	public void onSongChanged(int index) {
		index = songDropdown.value;
		if(Game.gameInstance.isPlaying()) return;
		PlayerPrefs.SetString(SONG_KEY, songsList.Count > index ? songsList[index] : "");
		Game.gameInstance.LoadSong();
	}

	public void changeVolume() {
		video.SetDirectAudioVolume(0, volumeSlider.value / 100);
		audioSource.volume = volumeSlider.value / 100;
		hitSound.volume = volumeSlider.value / 100;
		PlayerPrefs.SetInt(VOLUME_KEY, (int)volumeSlider.value);
	}

	public void changeOtogeMode() {
		if(Game.gameInstance.isPlaying()) return;
		Game.gameInstance.changeMode(otogeToggle.isOn);
		autoToggle.interactable = otogeToggle.isOn;
		PlayerPrefs.SetInt(OTOGE_KEY, otogeToggle.isOn ? 1 : 0);
	}

	public void changeAutoMode() {
		if(Game.gameInstance.isPlaying()) return;
		Game.gameInstance.changeAutoMode(autoToggle.isOn);
		PlayerPrefs.SetInt(AUTO_KEY, autoToggle.isOn ? 1 : 0);
	}

	public void exitGame() {
		#if UNITY_EDITOR
			EditorApplication.isPlaying = false;
		#elif UNITY_STANDALONE
			Application.Quit();
		#endif
	}

	public void PushBackButton() {
		transform.gameObject.SetActive(false);
		if(Game.gameInstance.isPausing()) Game.gameInstance.Pause();
	}

	public void PushTopButton() {
		Game.gameInstance.backToTop();
	}
}
