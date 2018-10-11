using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class ExtendSongs : MonoBehaviour {
	public const string SONGS_DIR = "songs";
	public const string CHART_FILE = "chart.txt";
	public const string MOVIE_FILE = "movie.mp4";
	public const string MUSIC_FILE = "music.wav";
	public const string BACK_FILE = "background.png";

	///
	/// chart.txtと(movie.mp4 または music.mp3)を含むsongs下のディレクトリリストを返す
	///
	public static List<string> GetSongs() {
		string path = Application.dataPath + "/" + SONGS_DIR;
		if(!Directory.Exists(path)) {
			Directory.CreateDirectory(path);
		}

		string[] pathes = Directory.GetDirectories(path, "*", System.IO.SearchOption.TopDirectoryOnly);
		List<string> correctPaths = new List<string>();
		foreach(string d in pathes) {
			string dir = d.Replace("\\", "/");
			string[] files = Directory.GetFiles(dir, "*", SearchOption.TopDirectoryOnly);

			for(int i = 0; i < files.Length; i++) {
				files[i] =  files[i].Replace("\\", "/");
			}

			if(files.Contains(dir + "/" + CHART_FILE) && 
				(files.Contains(dir + "/" + MOVIE_FILE) || files.Contains(dir + "/" + MUSIC_FILE))) correctPaths.Add(dir);
		}
	
		return correctPaths;
	}

	public delegate void completedSetAudio();

	///
	/// 楽曲をAudoSourceにSet.
	///
	public static IEnumerator SetAudioSource(string songPath, completedSetAudio callback) {
		string url = "file://" + songPath + "\\" + MUSIC_FILE;

		UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.WAV);

		yield return www.SendWebRequest();

		if(!www.isNetworkError) {
			GameObject.Find("Audio Source").GetComponent<AudioSource>().clip = DownloadHandlerAudioClip.GetContent(www);
			callback();
		} else {
			Debug.Log(www.error);
		}
	}

	///
	/// FilePathから画像読み込み
	/// From https://qiita.com/consolesoup/items/5faf1b48c8db2e08f393
	///
	public static Texture2D Texture2DFromFile(string path)
	{
		Texture2D texture = null;
		if (File.Exists(path))
		{
			//byte取得
			FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
			BinaryReader bin = new BinaryReader(fileStream);
			byte[] readBinary = bin.ReadBytes((int)bin.BaseStream.Length);
			bin.Close();
			fileStream.Dispose();
			fileStream = null;
			if (readBinary != null)
			{
				//横サイズ
				int pos = 16;
				int width = 0;
				for (int i = 0; i < 4; i++)
				{
					width = width * 256 + readBinary[pos++];
				}
				//縦サイズ
				int height = 0;
				for (int i = 0; i < 4; i++)
				{
					height = height * 256 + readBinary[pos++];
				}
				//byteからTexture2D作成
				texture = new Texture2D(width, height);
				texture.LoadImage(readBinary);
			}
			readBinary = null;
		}
		return texture;
	}
	
	public static Sprite SpriteFromFile(string path)
	{
		Texture2D texture = Texture2DFromFile(path);
		Sprite sprite = null;
		if (texture)
		{
			//Texture2DからSprite作成
			sprite = Sprite.Create(texture, new UnityEngine.Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
		}
		return sprite;
	}
}

