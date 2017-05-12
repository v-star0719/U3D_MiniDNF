using UnityEngine;
using System.Collections;

[RequireComponent(typeof(X2DAnimation))]
public class X2DEffectClip : MonoBehaviour
{
	public float startTime;
	public float duration;//小于等于0就是一直播
	public bool autoDuration = false;//自动将动画的时间作为特效的时间

	private X2DAnimation _x2dAnima;
	public X2DAnimation x2dAnima
	{
		get
		{
			if(_x2dAnima == null)
				_x2dAnima = GetComponent<X2DAnimation>();
			return _x2dAnima;
		}
	}

	public float endTime
	{
		get{
			if(duration <= 0)
				return float.MaxValue;
			return startTime + duration;
		}
	}

	//不直接用x2dAnima.IsPlaying();，可能动画播完了，但是这个特效还没播完
	private bool _isPlaying;
	public bool isPlaying
	{
		get{
			return _isPlaying;
		}
	}

	public bool isFaceRight
	{
		get { return x2dAnima.isFaceRight; }
		set
		{
			x2dAnima.isFaceRight = value; ;
		}
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Play()
	{
		if(_isPlaying) return;
		_isPlaying = true;
		x2dAnima.Play("");
	}

	public void Stop()
	{
		_isPlaying = false;
		x2dAnima.Stop();
		//Debug.Log("stop at " + Time.time);
	}
}
