using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class ExtendSongs : MonoBehaviour {
	///
	/// chart.txtとmovie.mp4を含むsongs下のディレクトリリストを返す
	///
	public static List<string> GetSongs() {
		string path = Application.dataPath + "/songs";
		if(!Directory.Exists(path)) {
			Directory.CreateDirectory(path);
		}

		string[] pathes = Directory.GetDirectories(path, "*", System.IO.SearchOption.TopDirectoryOnly);
		List<string> correctPaths = new List<string>();
		foreach(string dir in pathes) {
			string[] files = Directory.GetFiles(dir, "*", SearchOption.TopDirectoryOnly);
			if(files.Contains(dir + "\\chart.txt") && files.Contains(dir + "\\movie.mp4")) correctPaths.Add(dir);
		}
	
		return correctPaths;
	}
}
