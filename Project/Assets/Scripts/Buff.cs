using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//中毒，出血，灼伤作为技能

//buff和技能的不同在哪儿：buff只影响属性
//buff需要单独配表？，如何区分是否是同一个buff
public class Buff 
{
	public bool isAlive = false;

	private DBBuffConf buffConf;
	private float buffValue;
	private float timer;
	private float duration;
	private ActorBase actor;

	//buff
	public void ManualUpdate()
	{
		timer += Time.deltaTime;
		if(timer >= duration)
		{
			isAlive = false;
			End();
		}
	}

	public void Start(ActorBase actor, DBBuffConf conf)
	{
		timer = 0f;
		isAlive = true;
		buffConf = conf;
		this.actor = actor;
		duration = conf.duration;
		SetBuffValue(true);
		Debug.Log("Start buff " + buffConf.ID);
	}

	public void End()
	{
		isAlive = false;
		SetBuffValue(false);
		Debug.Log("End buff " + buffConf.ID + "  " + timer);
	}

	private void SetBuffValue(bool isStart)
	{
		//actor.curAttribute.attack += buffConf.attack;
		//actor.curAttribute.attackSpeed += buffConf.attackSpeed;
		SetValue(ref actor.curAttribute.attack, actor.orgAttribute.attack, buffConf.attack, isStart);
		SetValue(ref actor.curAttribute.attackSpeed, actor.orgAttribute.attackSpeed, buffConf.attackSpeed, isStart);
	}

	private void SetValue(ref float curValue, float orgValue,  int buffValue, bool isStart)
	{
		//1千万以内的为直接数值，最后两位为小数
		//1千万到2千万为负的百分比值，10000001为1%
		//2千万到3千万为正的百分比值

		if(!isStart)
			buffValue = -buffValue;

		if(buffValue < 10000000)
		{
			curValue += buffValue / 100f;
		}
		else if(buffValue < 20000000)
		{
			curValue += orgValue * (buffValue - 10000000) / 100f;
		}
		else if(buffValue < 30000000)
		{
			curValue -= orgValue * (buffValue - 20000000) / 100f;
		}
		else
		{
			Debug.LogError("unknow buff value format");
		}
	}
}
