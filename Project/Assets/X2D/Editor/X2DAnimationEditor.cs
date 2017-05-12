using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(X2DAnimation))]
public class X2DAnimationEditor : Editor
{
	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI ();
		X2DAnimation anima = target as X2DAnimation;

		anima.flipX = EditorGUILayout.Toggle("flipX", anima.flipX);
		anima.flipY = EditorGUILayout.Toggle("flipY", anima.flipY);
		anima.isFaceRight = EditorGUILayout.Toggle("isFaceRight", anima.isFaceRight);

		if(GUILayout.Button("UpdateClipArray", GUILayout.Width(120)))
		{
			UpdateClipArrayInEditor(anima);
		}

		GUI.enabled = Application.isPlaying;
		if(!anima.isPreviewing)
		{
			if(GUILayout.Button("StartPreviewAll", GUILayout.Width(150)))
			{
				X2DAnimationPreviewer previewer = anima.GetComponent<X2DAnimationPreviewer>();
				if(previewer == null)
					previewer = anima.gameObject.AddComponent<X2DAnimationPreviewer>();
				previewer.PreviewAll();
			}
		}
		else
		{
			if(GUILayout.Button("StopPreviewAll", GUILayout.Width(150)))
			{
				X2DAnimationPreviewer previewer = anima.GetComponent<X2DAnimationPreviewer>();
				if(previewer != null)
					previewer.StopPreview();
			}
		}
		GUI.enabled = true;
	}

	//编辑器中刷新clip数组
	public void UpdateClipArrayInEditor(X2DAnimation anima)
	{
		X2DAnimationClip[] clipArray = anima.GetComponentsInChildren<X2DAnimationClip>(true);
		anima.clipArray = clipArray;
		//剪辑名字排列
		for(int i=0; i<clipArray.Length-1; i++)
		{
			for(int j=0; j<clipArray.Length-1-i; j++)
			{
				if(clipArray[j].clipName.CompareTo(clipArray[j+1].clipName) > 0)
				{
					X2DAnimationClip t = clipArray[j];
					clipArray[j] = clipArray[j+1];
					clipArray[j+1] = t;
				}
			}
		}
	}
}
