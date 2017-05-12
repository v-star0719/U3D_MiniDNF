using UnityEngine;
using System.Collections;

//编辑器扩展，按钮刷新特效剪辑列表
//测试播放按钮
[ExecuteInEditMode]
public class X2DEffect : MonoBehaviour
{
	public X2DEffectClip[] clipArray;
	public float duration;
	public bool loop = false;
	public bool playOnAwake = false;
	public bool autoDuration = false;//自动将动画的时间作为特效的时间

	private float timer;
	private int stoppedClipCount = 0;

	[HideInInspector]
	public bool isPlaying;
	[HideInInspector]
	public bool hasPlayed;


	public float playTimer
	{
		get{return timer;}
	}

	private bool _isFacingRight = true;//默认物体是朝向右的
	public bool isFacingRight
	{
		get { return _isFacingRight; }
		set
		{
			if(_isFacingRight == value) return;
			_isFacingRight = value;
			for(int i=0; i<clipArray.Length; i++)
				clipArray[i].isFaceRight = _isFacingRight;
		}
	}

	void Start()
	{
#if UNITY_EDITOR
		if(!Application.isPlaying) return;
#endif

		if(playOnAwake)
			Play();
	}

	// Update is called once per frame
	void Update ()
	{
		X2DTools.DrawPosMark(gameObject, transform.position, gameObject.layer);

#if UNITY_EDITOR
		if(!Application.isPlaying)
		{
			EditorUpdateClipArray();
			for(int i=0; i<clipArray.Length; i++ )
			{
				clipArray[i].transform.localPosition = Vector3.zero;//保持为（0， 0， 0）,这样改变朝向时不会出错，见X2DAniamtion.isFacingRight
			}
			return;
		}
#endif

		if(!isPlaying) return;

		timer += Time.deltaTime;
		for(int i=0; i<clipArray.Length; i++)
		{ 
			X2DEffectClip clip = clipArray[i];
			if(!clip.isPlaying)
			{
				if(timer > clip.startTime && timer < clip.endTime)
				{
					clip.gameObject.SetActive(true);
					clip.Play();
				}
			}
			else if(clip.endTime > 0 && timer >= clip.endTime)
			{
				clip.Stop();
				stoppedClipCount++;

				//循环播
				if(loop && stoppedClipCount == clipArray.Length)
				{
					isPlaying = false;
					Play();
				}
			}
		}

		if(stoppedClipCount == clipArray.Length && timer >= duration)
		{
			isPlaying = false;
			return;
		}
	}

	public void Play()
	{
		if(isPlaying) return;
		for(int i=0; i<clipArray.Length; i++)
			clipArray[i].gameObject.SetActive(false);
		hasPlayed = true;
		isPlaying = true;
		timer = 0f;
		stoppedClipCount= 0;
	}

	public void Stop()
	{
		isPlaying = false;
		for(int i = 0; i < clipArray.Length; i++)
		{
			clipArray[i].Stop();
		}
	}

	public void EditorUpdateClipArray()
	{
		clipArray = GetComponentsInChildren<X2DEffectClip>(true);
		//剪辑名字排列
		for(int i=0; i<clipArray.Length-1; i++)
		{
			for(int j=0; j<clipArray.Length-1-i; j++)
			{
				if(clipArray[j].name.CompareTo(clipArray[j+1].name) > 0)
				{
					X2DEffectClip t = clipArray[j];
					clipArray[j] = clipArray[j+1];
					clipArray[j+1] = t;
				}
			}
		}
	}
}
