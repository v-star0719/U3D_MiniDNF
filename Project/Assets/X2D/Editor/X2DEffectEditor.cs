using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(X2DEffect))]
public class X2DEffectEditor : Editor
{
	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI ();
		X2DEffect effect = target as X2DEffect;

		//if(GUILayout.Button("UpdateDuration", GUILayout.Width(120)))
		if(effect.autoDuration)
		{
			UpdateEffectDurationInEditor(effect);
		}
	}

	private void UpdateEffectDurationInEditor(X2DEffect effect)
	{
		//把最后结束的那个结束时间作为持续时间
		X2DEffectClip[] clipArray = effect.GetComponentsInChildren<X2DEffectClip>(true);
		effect.clipArray = clipArray;
		//剪辑名字排列
		float t = 0;
		for(int i=0; i<clipArray.Length; i++)
		{
			if(t < clipArray[i].startTime + clipArray[i].duration)
				t = clipArray[i].startTime + clipArray[i].duration;
		}
		effect.duration = t;
	}
}
