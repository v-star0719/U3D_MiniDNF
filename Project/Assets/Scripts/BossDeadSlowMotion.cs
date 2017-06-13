using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BossDeadSlowMotion : MonoBehaviour 
{
	public delegate void VoidCallback();

	public const float duuration = 1.5f;
	public const float slowdwonTimeScale = 0.1f;

	private float timer = 0;
	private bool isPlaying = false;
	private VoidCallback onPlayEnd;

	// Use this for initialization
	void Start ()
 	{
	}

	// Update is called once per frame
	void Update ()
	{
		if(!isPlaying) return;

		timer += Time.unscaledDeltaTime;
		if(timer >= duuration)
		{
			isPlaying = false;
			if(onPlayEnd != null)
				onPlayEnd();
			Time.timeScale = 1;
		}
	}

	public void Play(VoidCallback onPlayEndCallback)
	{
		isPlaying = true;
		timer = 0;
		onPlayEnd = onPlayEndCallback;
		Time.timeScale = slowdwonTimeScale;
	}
}
