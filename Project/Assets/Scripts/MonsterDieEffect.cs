using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct MonsterDieEffectData
{
	public X2DEffectClip clip;
	public Vector3 realPos;
	public Vector3 curSpeed;
	public bool isEnd;
}

[ExecuteInEditMode]
public class MonsterDieEffect : MonoBehaviour 
{
	public X2DEffectClip[] blockArray;
	public float maxDist;
	public Vector3 maxStartSpeed;
	public float falldownSpeed;

	public float maxFallTime;

	private EffectObject effectObject;
	private MonsterDieEffectData[] dataArray;
	private bool isPlaying = false;

	// Use this for initialization
	void Start ()
 	{
	}

	public bool testBtn;
	private bool isTesting;
	// Update is called once per frame
	void Update ()
	{
		if(!Application.isPlaying)
		{
			if(falldownSpeed != 0)
			{
				maxFallTime = maxStartSpeed.y / falldownSpeed * 2;
			}

			blockArray = GetComponentsInChildren<X2DEffectClip>();
		}

		if(testBtn)
		{
			EffectObject eo = new EffectObject();
			eo.x2dEffect = GetComponent<X2DEffect>();
			Play(eo);
			isTesting = true;
			testBtn = false;
		}

		if(!isPlaying) return;

		int endCount = 0;
		for(int i=0; i<dataArray.Length; i++)
		{
			if(dataArray[i].isEnd)
			{
				endCount++;
				continue;
			}

			Vector3 delta = dataArray[i].curSpeed * Time.deltaTime;
			Vector3 pos = dataArray[i].realPos + delta;
			if(pos.y <= 0)
			{
				pos.y = 0;
				dataArray[i].isEnd = true;
			}

			dataArray[i].clip.transform.localPosition  = ObjectBase.WorldTo2DPos(pos);
			dataArray[i].curSpeed.y -= falldownSpeed * Time.deltaTime;
			dataArray[i].realPos = pos;
		}

		if(endCount == dataArray.Length)
		{
			isPlaying = false;
			if(isTesting)
				effectObject.x2dEffect.Stop();
			else
				EffectManager.KillEffect(effectObject);
		}
	}

	public void Play(EffectObject eo)
	{
		effectObject = eo;
		if(dataArray == null)
		{
			dataArray = new MonsterDieEffectData[blockArray.Length];
			for(int i=0; i<dataArray.Length; i++)
				dataArray[i].clip = blockArray[i];
		}

		for(int i=0; i<dataArray.Length; i++)
		{
			dataArray[i].clip.transform.localPosition = Vector3.zero;
			dataArray[i].isEnd = false;
			dataArray[i].curSpeed.x = Random.Range(-maxStartSpeed.x, maxStartSpeed.x);
			dataArray[i].curSpeed.y = Random.Range(0, maxStartSpeed.y);
			dataArray[i].curSpeed.z = Random.Range(-maxStartSpeed.z, maxStartSpeed.z);
			dataArray[i].realPos = Vector3.zero;
		}
		isPlaying = true;
		effectObject.x2dEffect.Play();
	}
}
