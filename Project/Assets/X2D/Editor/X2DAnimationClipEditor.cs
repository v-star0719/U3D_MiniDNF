using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(X2DAnimationClip))]
public class X2DAnimationClipEditor : Editor
{
	//private int frameListSize = 0;
	//private bool useTexutreSize = false;
	public override void OnInspectorGUI ()
	{
		X2DAnimationClip clip = target as X2DAnimationClip;

		//使用的shader，需要手动刷新下GameView（比如deactive,enactive），gameview就会update从而显示新的shader
		GUILayout.BeginHorizontal();
		GUILayout.Label("Shader", GUILayout.Width(100));
		Object shaderObj = EditorGUILayout.ObjectField(Shader.Find(clip.renderShaderName), typeof(Shader), true);
		Shader shader = shaderObj as Shader;
		if(shader != null)
			clip.renderShaderName = shader.name;
		GUILayout.EndHorizontal();

		//默认视图
		base.OnInspectorGUI();

		//将gameobject的名字锁定为和剪辑名一样
		if(clip.name != clip.clipName)
			clip.name = clip.clipName;

		//增加预览按钮
		GUI.enabled = Application.isPlaying;
		if(GUILayout.Button("StartPreview", GUILayout.Width(100)))
		{
			GameObject animaGO = clip.transform.parent.gameObject;
			X2DAnimationPreviewer previewer = animaGO.GetComponent<X2DAnimationPreviewer>();
			if(previewer == null)
				previewer = animaGO.AddComponent<X2DAnimationPreviewer>();
			previewer.PreviewOne(clip);
		}
		if(GUILayout.Button("StopPreview", GUILayout.Width(100)))
		{
			GameObject animaGO = clip.transform.parent.gameObject;
			X2DAnimationPreviewer previewer = animaGO.GetComponent<X2DAnimationPreviewer>();
			if(previewer != null)
				previewer.StopPreview();
		}
		GUI.enabled = true;

		return;
		/*
		X2DAnimationClip clip = target as X2DAnimationClip;
		if(clip == null) return;

		//default time
		GUILayout.BeginHorizontal();
		GUILayout.Label("DefaultTime", GUILayout.Width(100));
		clip.defaultTime= EditorGUILayout.FloatField(clip.defaultTime, GUILayout.Width(150));
		if(GUILayout.Button("SetAll", GUILayout.Width(48)))
		{
			for(int i=0; i<clip.frameList.Count; i++)
				clip.frameList[i].time = clip.defaultTime;
		}
		GUILayout.EndHorizontal();

		//default size
		GUILayout.BeginHorizontal();
		GUILayout.Label("DefaultSize", GUILayout.Width(100));
		clip.defaultSize= EditorGUILayout.Vector2Field("", clip.defaultSize, GUILayout.Width(150));
		if(GUILayout.Button("SetAll", GUILayout.Width(48)))
		{
			for(int i=0; i<clip.frameList.Count; i++)
				clip.frameList[i].size = clip.defaultSize;
		}
		GUILayout.EndHorizontal();

		//clip name
		GUILayout.BeginHorizontal();
		GUILayout.Label("ClipName", GUILayout.Width(100));
		clip.clipName = GUILayout.TextField(clip.clipName, GUILayout.Width(100));
		GUILayout.EndHorizontal();

		//duration
		clip.duration = 0;
		for(int i=0; i<clip.frameList.Count; i++)
			clip.duration += clip.frameList[i].time;
		GUILayout.BeginHorizontal();
		GUILayout.Label("Duration", GUILayout.Width(100));
		clip.duration = EditorGUILayout.FloatField(clip.duration, GUILayout.Width(100));
		GUILayout.EndHorizontal();

		//play mode
		GUILayout.BeginHorizontal();
		GUILayout.Label("PlayMode",GUILayout.Width(100));
		clip.playMode = (EmX2DAnimationPlayMode)EditorGUILayout.EnumPopup(clip.playMode, GUILayout.Width(100));
		GUILayout.EndHorizontal();

		//list length
		GUILayout.BeginHorizontal();
		GUILayout.Label("FrameList", GUILayout.Width(100));
		GUILayout.Label(clip.frameList.Count.ToString(), GUILayout.Width(10));
		frameListSize = EditorGUILayout.IntField(frameListSize, GUILayout.Width(86));
		bool resize = GUILayout.Button("Resize", GUILayout.Width(50));
		GUILayout.EndHorizontal();
		if(resize)
		{
			if(frameListSize > clip.frameList.Count)
			{
				for(int i=clip.frameList.Count; i<frameListSize; i++){
					X2DAnimationFrame f = new X2DAnimationFrame();
					clip.frameList.Add(f);
				}
			}
			else if(frameListSize < clip.frameList.Count)
			{
				for(int i=clip.frameList.Count; i>frameListSize; i--)
					clip.frameList.RemoveAt(clip.frameList.Count-1);
			}
		}

		//list成员
		for(int i=0; i<clip.frameList.Count; i++)
		{
			X2DAnimationFrame frame = clip.frameList[i];
			GUILayout.BeginHorizontal();
			GUILayout.Space(20);
			//order
			GUILayout.Label(i.ToString(), GUILayout.Width(10));
			//time
			GUILayout.Label("Time", GUILayout.Width(34));
			clip.frameList[i].time = EditorGUILayout.FloatField(frame.time, GUILayout.Width(40));
			//size
			GUILayout.Label(" size x", GUILayout.Width(40));
			frame.size.x = EditorGUILayout.FloatField(frame.size.x,GUILayout.Width(40));
			GUILayout.Label("y", GUILayout.Width(10));
			frame.size.y = EditorGUILayout.FloatField(frame.size.y, GUILayout.Width(40));
			//texture
			GUILayout.Label(" tex", GUILayout.Width(26));
			frame.tex = EditorGUILayout.ObjectField(frame.tex, typeof(Object), true) as Texture2D;

			useTexutreSize = GUILayout.Toggle(useTexutreSize, "auto");
			if(useTexutreSize) frame.size = new Vector2(frame.tex.width, frame.tex.height);

			GUILayout.EndHorizontal();
		}
		*/
	}
}
