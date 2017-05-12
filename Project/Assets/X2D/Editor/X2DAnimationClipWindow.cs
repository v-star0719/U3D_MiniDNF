using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class FrameTextureInfo
{
	public int i;
	public int x;
	public int y;
	public int width;
	public int height;
	public int frameWidth;
	public int frameHeight;

	public override string ToString()
	{
		return string.Format("{0} pos({1}, {2}), size({3}, {4}), frameRegion({5}, {6})", i, x, y, width, height, frameWidth, frameHeight);
	}
}

public class X2DAnimationClipWindow : EditorWindow
{
	static int frameListSize = 0;
	static bool useTexutreSize = false;

	float oneKeyFrameTime;
	Vector2 oneKeyFrameSize;
	Vector2 oneKeyFrameRegion;

	[MenuItem("Window/X2DAnimation")]
	public static void OpenX2DAnimationEditorWindow()
	{
		X2DAnimationClipWindow window = (X2DAnimationClipWindow)EditorWindow.GetWindow(typeof(X2DAnimationClipWindow), false, "X2DAnimation");	
		window.Show();
	}

	private Vector2 mScroll = Vector2.zero;
	private X2DAnimationClip curSelectedClip;

	void OnGUI ()
	{
		X2DAnimationClip clip = null;
		if(Selection.activeGameObject != null)
			clip = Selection.activeGameObject.GetComponent<X2DAnimationClip>();
		if(clip != null)
			curSelectedClip = clip;
		else
			clip = curSelectedClip;//如果当前没选中动画剪辑，则继续显示当前的动画剪辑
		if(clip == null) return;

		GUI.color = new Color(255/255f, 204/255f, 255/255f);
		GUILayout.Label("[" + clip.name + "]");
		GUI.color = Color.white;

		if(clip.frameList == null){
			GUI.color = Color.red;
			GUILayout.Label("剪辑列未创建");
			GUI.color = Color.white;
			return;
		}

		//分左右两组
		GUILayout.BeginHorizontal();
		//第一组
		GUILayout.BeginVertical(GUILayout.Width(300));
		//OneKeyFrameTime
		GUILayout.BeginHorizontal();
		if(GUILayout.Button("OneKeyFrameTime", GUILayout.Width(132)))
		{
			for(int i=0; i<clip.frameList.Count; i++)
				clip.frameList[i].duration = oneKeyFrameTime;
		}
		oneKeyFrameTime = EditorGUILayout.FloatField(oneKeyFrameTime, GUILayout.Width(150));
		GUILayout.EndHorizontal();

		//OneKeyFrameSize
		GUILayout.BeginHorizontal();
		if(GUILayout.Button("OneKeyFrameSize", GUILayout.Width(132)))
		{
			for(int i=0; i<clip.frameList.Count; i++)
				clip.frameList[i].size = oneKeyFrameSize;
		}
		oneKeyFrameSize = TqmGUILayoutUtility.SimpleVector2(oneKeyFrameSize, 60);
		GUILayout.EndHorizontal();

		//OneKeyFrameRegion
		GUILayout.BeginHorizontal();
		if(GUILayout.Button("OneKeyFrameRegion", GUILayout.Width(132)))
		{
			for(int i = 0; i < clip.frameList.Count; i++)
				clip.frameList[i].region = oneKeyFrameRegion;
		}
		oneKeyFrameRegion = TqmGUILayoutUtility.SimpleVector2(oneKeyFrameRegion, 60);
		GUILayout.EndHorizontal();
		GUILayout.EndVertical();
		//第二组
		GUILayout.BeginVertical();
		//clip name
		GUILayout.BeginHorizontal();
		GUILayout.Label("ClipName", GUILayout.Width(100));
		clip.clipName = GUILayout.TextField(clip.clipName, GUILayout.Width(100));
		GUILayout.EndHorizontal();
		
		//duration
		clip.duration = 0;
		for(int i=0; i<clip.frameList.Count; i++)
			clip.duration += clip.frameList[i].duration;
		GUILayout.BeginHorizontal();
		GUILayout.Label("Duration", GUILayout.Width(100));
		clip.duration = EditorGUILayout.FloatField(clip.duration, GUILayout.Width(100));
		GUILayout.EndHorizontal();
		
		//play mode
		GUILayout.BeginHorizontal();
		GUILayout.Label("PlayMode",GUILayout.Width(100));
		clip.playMode = (EmX2DAnimationPlayMode)EditorGUILayout.EnumPopup(clip.playMode, GUILayout.Width(100));
		GUILayout.EndHorizontal();

		GUILayout.EndVertical();
		GUILayout.EndHorizontal();

		//pos correct
		GUILayout.BeginHorizontal();
		if(clip.posCorrecting)
		{
			GUI.color = Color.red;
			if(GUILayout.Button("StopCorrecting", GUILayout.Width(100)))
			{
				GameObject animaGO = clip.transform.parent.gameObject;
				X2DAnimation anima = animaGO.GetComponent<X2DAnimation>();
				if(anima != null)
					anima.EditorShowRenderer();
				clip.posCorrecting = false;
			}
			GUI.color = Color.white;
		}
		else
		{
			if(GUILayout.Button("StartCorrecting", GUILayout.Width(100)))
			{
				GameObject animaGO = clip.transform.parent.gameObject;
				X2DAnimation anima = animaGO.GetComponent<X2DAnimation>();
				if(anima != null)
					anima.EditorHideRenderer();
				clip.posCorrecting = true;
			}
		}
		clip.Update_PosCorrecting();

		//fill with seleced textures
		if(GUILayout.Button("fill with selected textures", GUILayout.Width(160)))
		{
			if(Selection.objects.Length == 0)
				Debug.LogError("no object selected");
			else
			{
				Object[] array = Selection.objects;
				SortByName(array);
				for(int i=0; i<array.Length && i < clip.frameList.Count; i++)
				{
					clip.frameList[i].tex = array[i] as Texture2D;
				}
			}
		}

		//load frame info
		if(GUILayout.Button("load frame info", GUILayout.Width(100)))
			LoadTextureInfo(clip);

		//load org tex size
		useTexutreSize = GUILayout.Button("load org tex size", GUILayout.Width(120));

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
		int insertAt = -1;
		int removeAt = -1;
		mScroll = GUILayout.BeginScrollView(mScroll);
		float frameStartTime = 0f;
		for(int i=0; i<clip.frameList.Count; i++)
		{
			X2DAnimationFrame frame = clip.frameList[i];
			GUILayout.BeginHorizontal();
			GUILayout.Space(20);
			//order
			GUILayout.Label(i.ToString(), GUILayout.Width(10));
			//Duration
			TqmGUILayoutUtility.ColorLable(Color.blue, "duration", GUILayout.Width(60));
			clip.frameList[i].duration = EditorGUILayout.FloatField(frame.duration, GUILayout.Width(40));
			//startTime
			frame.startTime = frameStartTime;
			frameStartTime += frame.duration;
			GUILayout.Label(frame.startTime.ToString("f2"), GUILayout.Width(30));
			//size
			TqmGUILayoutUtility.ColorLable(Color.blue, " size", GUILayout.Width(30));
			frame.size.x = EditorGUILayout.FloatField(frame.size.x,GUILayout.Width(40));
			frame.size.y = EditorGUILayout.FloatField(frame.size.y, GUILayout.Width(40));
			if(useTexutreSize) frame.size = new Vector2(frame.tex.width, frame.tex.height);
			//pos
			TqmGUILayoutUtility.ColorLable(Color.blue, " pos", GUILayout.Width(28));
			frame.pos.x = EditorGUILayout.FloatField(frame.pos.x,GUILayout.Width(40));
			frame.pos.y = EditorGUILayout.FloatField(frame.pos.y, GUILayout.Width(40));
			//frameRegion
			TqmGUILayoutUtility.ColorLable(Color.blue, " region", GUILayout.Width(44));
			frame.region.x = EditorGUILayout.FloatField(frame.region.x, GUILayout.Width(40));
			frame.region.y = EditorGUILayout.FloatField(frame.region.y, GUILayout.Width(40));
			//texture
			TqmGUILayoutUtility.ColorLable(Color.blue, " tex", GUILayout.Width(26));
			frame.tex = EditorGUILayout.ObjectField(frame.tex, typeof(Object), true) as Texture2D;
			//insert remove
			if(GUILayout.Button("-")) removeAt = i;
			if(GUILayout.Button("+")) insertAt = i;

			GUILayout.EndHorizontal();
		}
		if(GUILayout.Button("+")) insertAt = clip.frameList.Count;

		//进行插入删除
		if(removeAt > -1)
			clip.frameList.RemoveAt(removeAt);
		if(insertAt > -1)
			clip.frameList.Insert(insertAt, new X2DAnimationFrame());

		GUILayout.EndScrollView();
		useTexutreSize = false;
	}

	void OnInspectorUpdate()
	{
		this.Repaint();
	}

	public static void SortByName(Object[] array)
	{
		for(int i=0; i<array.Length-1; i++)
		{
			for(int j=0; j<array.Length-i-1; j++)
			{
				if(GetIntInString(array[j].name) > GetIntInString(array[j+1].name))
				{
					Object t = array[j];
					array[j] = array[j+1];
					array[j+1] = t;
				}
			}
		}
	}

	public static int GetIntInString(string str)
	{
		int n = 0;
		for(int i=0; i<str.Length; i++)
		{
			char c = str[i];
			while(0 <= c && c <= '9')
			{
				n = n*10 + (c - '0');
				i++;
				if(i < str.Length) c = str[i];
				else c = 'Z';
			}
			if(n != 0)
				break;
		}
		return n;
	}

	//待优化：使用目录名作为信息文件名
	//待优化：数值读取方式
	//待完善：异常处理
	public static void LoadTextureInfo(X2DAnimationClip clip)
	{
		const string fileName = "info.txt";
		string texturePath = AssetDatabase.GetAssetPath(clip.frameList[0].tex);

		string textureDir = Application.dataPath.Replace("/Assets", "") + "/" + Path.GetDirectoryName(texturePath) + "/";
		Debug.Log(textureDir + fileName);
		
		//FileInfo fi = new FileInfo(fileName + fileName);
		StreamReader sr = new StreamReader(textureDir + fileName);
		if(sr == null)
			return;

		List<FrameTextureInfo> infoList = new List<FrameTextureInfo>();
		while(!sr.EndOfStream)
		{
			string line = sr.ReadLine();
			FrameTextureInfo fti = new FrameTextureInfo();
			ReadInt(line, "[", out fti.i);
			ReadInt(line, "  x=", out fti.x);
			ReadInt(line, "  y=", out fti.y);
			ReadInt(line, "  width=", out fti.width);
			ReadInt(line, "  height=", out fti.height);
			ReadInt(line, "  max_width=", out fti.frameWidth);
			ReadInt(line, "  max_height=", out fti.frameHeight);
			infoList.Add(fti);
		}
		sr.Close();

		for(int i=0; i<clip.frameList.Count; i++ )
		{
			X2DAnimationFrame frame = clip.frameList[i];
			for(int j=0; j<infoList.Count; j++)
			{
				FrameTextureInfo fti = infoList[j];
				if(GetIntInString(frame.tex.name) == fti.i)
				{
					frame.pos.x = (fti.width - fti.frameWidth)*0.5f + fti.x;//pos是相对于frameRegion左上角的偏移量
					frame.pos.y = (fti.frameHeight - fti.height)*0.5f - fti.y;//pos是相对于frameRegion左上角的偏移量（y轴竖直向下）
					frame.size.x = fti.width;
					frame.size.y = fti.height;
					frame.region.x = fti.frameWidth;
					frame.region.y = fti.frameHeight;
					break;
				}
			}
		}
	}

	private static void ReadInt(string line, string name, out int n)
	{
		int i1 = line.IndexOf(name);
		i1 += name.Length;
		int i2 = i1;
		for(; i2<line.Length; i2++ )
		{
			if(line[i2]<'0' || line[i2]>'9')
				break;
		}

		if(!int.TryParse(line.Substring(i1, i2-i1), out n))
			Debug.Log("int.TryParse error");
	}
}
