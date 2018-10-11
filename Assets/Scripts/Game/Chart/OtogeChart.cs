using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtogeChart : AbstractChart {
	private Vector3 previousPos;
	private List<float> timingList;
	private GameObject previousTarget;
	private int generated = 0;
	private float generateInterval = 1.6f;
	private bool autoMode = false;

	public override void SetAutoMode(bool auto){
		autoMode = auto;
	}

	protected override Vector3 GetRandomPosition() {
		Vector3 newPos;
		int width = Consts.WIDTH, height = Consts.HEIGHT;

		if(previousTarget == null) {
			newPos = new Vector3(UnityEngine.Random.Range(-width, width), UnityEngine.Random.Range(-height, height), 0);
		} else {
			float interval = width / 2;
			float widthMinus = previousPos.x - interval > -width ? previousPos.x - interval : -width;
			float widthPlus = previousPos.x + interval  < width ? previousPos.x + interval : width;
			float randX = UnityEngine.Random.Range(widthMinus, widthPlus);
			if(randX > previousPos.x - 1 && randX < previousPos.x + 1) {
				randX = randX - 1.5f < -interval ? randX + 3 : randX - 1.5f;
			} 

			interval = height / 2;
			float heightMinus = previousPos.y - interval  > -height ? previousPos.y - interval : -height;
			float heightPlus = previousPos.y + interval < height ? previousPos.y + interval : height;
			float randY = UnityEngine.Random.Range(heightMinus, heightPlus);
			if(randY > previousPos.y - 1 && randY < previousPos.y + 1) {
				randY = randY - 1.5f < -interval ? randY + 3 : randY - 1.5f;
			}
			newPos = new Vector3(randX, randY, 0);
		}
		previousPos = newPos;
		return newPos;
	}

	public override void ChartProcess() {
		if(!GameManager.Instance.isPlaying()) return;
		if(timingList == null) return;

		if(Timer.Instance.GetTime() >= timingList[generated] - generateInterval) {
			newTarget = GenerateTarget();
			if(previousTarget != null) {
				GenerateGuideLine(previousTarget, newTarget);
			}
			previousTarget = newTarget;
			targets.Add(newTarget);

			Target target = newTarget.GetComponent<Target>();
			if(autoMode && target != null) target.changeAutoMode();
			generated++;
		}
	}

	public void SetTimingList(List<float> timingList) {
		this.timingList = timingList;
	}

	public override void Initialize() {
		generated = 0;
	}

	private void GenerateGuideLine(GameObject previousTarget, GameObject newTarget) {
		//if(GameManager.guideLineNodePrefab == null) GameManager.guideLineNodePrefab = GameManager.Instance.guideLinePrefab;
		Vector3 prePos = previousTarget.transform.position;
		Vector3 newPos = newTarget.transform.position;
		float dis = Vector3.Distance(prePos, newPos);
		int numGuide = (int)(dis / 0.5f);
		float currentTiming = timingList[generated - 1];
		float nextTiming = timingList[generated];
		float interval = nextTiming - currentTiming;
		interval /= numGuide;
		float t = (float)1 / numGuide;
		Vector3 guidePos = Vector3.Lerp(prePos, newPos, t);
		Instantiate(GameManager.Instance.guideLinePrefab, guidePos, Quaternion.identity).GetComponent<GuidelineNode>().setNodeInfo(interval, prePos, newPos, numGuide, 1, true, currentTiming, Timer.Instance.GetTime());
	}
}
