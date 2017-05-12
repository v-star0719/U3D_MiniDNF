using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class AnimationCurveData
{
	public string curveName;
	public AnimationCurve curve;
}

public class AnimationCurveCollection : MonoBehaviour
{
	private static AnimationCurveCollection instance;
	public List<AnimationCurveData> curveDataArray;

	void Awake()
	{
		instance = this;
	}
	void OnDestroy()
	{
		instance = null;
	}

	//public 
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public static AnimationCurve GetAnimationCurve(string curveName){
		return instance.GetAnimationCurveX(curveName);
	}
	private AnimationCurve GetAnimationCurveX(string curveName)
	{
		for(int i=0; i<curveDataArray.Count; i++)
		{
			AnimationCurveData data = curveDataArray[i];
			if(data.curveName == curveName)
				return data.curve;
		}
		Debug.LogError("AnimationCurve is not exist: " + curveName);
		return null;
	}
}
