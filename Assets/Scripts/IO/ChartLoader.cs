using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Text;
public class ChartLoader{
	public static List<float> Load(string fullPath) {
		List<float> timingList = new List<float>();
		FileInfo fi = new FileInfo(fullPath);
		string line = "";
		try {
			bool osu = false, timingPoint = false;
			using(StreamReader sr = new StreamReader(fi.OpenRead(), Encoding.UTF8)) {
				while((line = sr.ReadLine()) != null) {
					if(line.StartsWith("osu")) {
						osu = true;
					}
					if(!osu) {
						timingList.Add(float.Parse(line) - 0.3f);
					} else {
						if(timingPoint) {
							string[] datas = line.Split(',');
							float timing = float.Parse(datas[2]) / 1000;
							timingList.Add(timing);
						} else {
							if(line.StartsWith("[HitObjects]")) timingPoint = true;
						}
					}
					
				}
			}
			//if(hasInterval) playInterval = 3.0f;
		}catch (Exception e) {
			//Debug.Log(e);
			//TODO: 例外処理
		}
		return timingList;
	}
}
