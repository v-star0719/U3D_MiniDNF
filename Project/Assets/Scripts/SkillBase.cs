using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum EmDamageMoveEffect
{
	None = 0, //神不知鬼不觉的打了下
	Frozen,//定身，如果当前在空中，则定在空中
	HitMove,//被击位移，通过x,y速度和持续时间控制，连续击中物体后切换移动速度
	HitMoveFreezeInAir,//同上，连续击中物体后，如果物体在空中，则将物体定住
	HitMoveTo,//被击后移动到指定位置，这个位置是表里配的相对角色的位置
	HitMoveDist,//被击移动一段距离
	HitFallDown,//被击倒地
}

public enum EmDamageStatusEffect
{
	None = 0,
	Freeze = 1,
	Stun = 2,
}

public enum EmDamageArenaType
{
	Sphere = 0,
	FrontArena = 1,
}

public enum EmDamageCenterType
{
	None = 0,
	Attacker = 1,
	Target = 2,
	AttackObject = 3,
}

public enum EmAttackObjectMoveMode
{
	None,//在出生位置播放特效
	FollowCaster,//跟随释放者
	AutoMove_Vector,//按向量移动
	AutoMove_Speed,//按速度移动
}

public enum EmPlaySKillResult
{
	None,
	WaitToCombo,
	PlayNext
}

public class SkillBase
{
	public ActorBase actor;
	public ObjectBase damageCenter;//如果为null，则表示中心为出生位置
	public Vector3 bornPos;
	public bool isActive;

	[HideInInspector]
	public DBSkillConf skillConf;
	public DBSkillAttackConf curAttackConf;

	protected int curAttackIndex;
	protected List<SkillAttack> attackList = new List<SkillAttack>();//触发的攻击列表

	//public int skillId
	//{
	//	get
	//	{
	//		return skillConf.skillID;
	//	}
	//}

	public int skillID
	{
		get
		{
			return skillConf.skillID;
		}
	}

	// Update is called once per frame
	public void ManualUpdate ()
	{
		if(!isActive) return;

		int activeAttackCount = 0;
		for(int i = 0; i < attackList.Count; i++)
		{
			SkillAttack att = attackList[i];
			if(att.isFinished)
				activeAttackCount++;
			else
				att.ManualUpdate();
		}

		//攻击虽然结束了，但是连击还有连击，技能不应该销毁
		if(activeAttackCount == attackList.Count)
		{
			isActive = false;
			Destroy();
		}

		LateManualUpdate();
	}

	protected virtual void LateManualUpdate()
	{ }

	public void Init(ActorBase actor, int skillID)
	{
		skillConf = DBSkillTable.GetRecord(skillID);
		this.actor = actor;
		curAttackIndex = 0;
		curAttackConf = skillConf.attackList[curAttackIndex];//actor需要判断当前攻击是否释放时间
		isActive = true;
	}

	public void DoCurAttack()
	{
		//以后再考虑要不要做缓存池
		SkillAttack attack = new SkillAttack();
		attack.StartAttack(this, actor, curAttackConf);
		attackList.Add(attack);
		AfterDoCurAttack(attack);
	}

	protected virtual void BeforeDoCurAttack()
	{ }

	protected virtual void AfterDoCurAttack(SkillAttack curAttack)
	{ }

	///通过多次释放技能来切换技能的多段攻击
	public EmPlaySKillResult PlaySKill(float timer, float statusDuration, float attackSpeed)
	{
		if(curAttackIndex < skillConf.attackList.Count - 1)
		{
			if(curAttackConf.autoNextAttack)
				return EmPlaySKillResult.None;

			if(timer >= statusDuration)
			{
				curAttackIndex++;//开始下个攻击
				curAttackConf = skillConf.attackList[curAttackIndex];//actor需要判断当前攻击是否释放时间
				return EmPlaySKillResult.PlayNext;
			}
			else
			{
				if(timer >= curAttackConf.comboStartTime * (1 - attackSpeed)) {
					return EmPlaySKillResult.WaitToCombo;
				}
			}
		}
		return EmPlaySKillResult.None;
	}

	public void PlaySkill_AutoNextAttack()
	{
		if(curAttackIndex < skillConf.attackList.Count - 1)
		{
			curAttackIndex++;//开始下个攻击
			curAttackConf = skillConf.attackList[curAttackIndex];//actor需要判断当前攻击是否释放时间
			DoCurAttack();
		}
	}

	public void OnAttackEnd(SkillAttack sa)
	{
		if(sa == attackList[attackList.Count - 1])
		{
			if(sa.attackConf.autoNextAttack)
			{
				PlaySkill_AutoNextAttack();
			}
		}
	}

	public void Stop()
	{
		isActive = false;
		Destroy();
	}

	private void Destroy()
	{
		actor = null;
		isActive = false;
		for(int i =0; i<attackList.Count; i++)
			attackList[i].Stop();
		attackList.Clear();
	}

	public void DrawGizmos()
	{
	}
}
