using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(X2DEffectClip))]
public class X2DEffectClipEditor : Editor
{
	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI();
		X2DEffectClip clip = target as X2DEffectClip;

		clip.x2dAnima.isFaceRight = EditorGUILayout.Toggle("isFaceRight", clip.x2dAnima.isFaceRight);

		//if(GUILayout.Button("AnimationTimeAsDuration", GUILayout.Width(180)))
		if(clip.autoDuration)
		{
			if(clip.x2dAnima == null)
				Debug.Log("x2dAnima is null");
			else if(clip.x2dAnima.defaultClip == null)
				Debug.Log("x2dAnima.defaultClip is null");
			else
				clip.duration = clip.x2dAnima.defaultClip.duration;
		}

	}
}
