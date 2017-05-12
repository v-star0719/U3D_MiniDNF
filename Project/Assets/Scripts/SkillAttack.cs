using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SkillAttack
{
	public SkillBase skill;
	public ActorBase actor;
	public ObjectBase damageCenter;//如果为null，则表示中心为出生位置
	public Vector3 bornPos;

	public DBSkillAttackConf attackConf;

	public bool isFinished = false;

	public EffectObject eo;
	private float damageTimer = 0f;//
	private float lifeTimer = 0f;//
	private DBSkillDamageConf[] damageConfArray;
	private float attackTimer;
	private int attackTimes;
	private int damageIndex;

	//显示
	private float showTimer = 0f;

	//移动
	private float attackObjectMoveDuration = 0f;
	private float attackObjectMoveTimer = 0f;
	//按向量移动
	private bool isAttackObjectMoving = false;
	private AnimationCurve attackObjectMoveCurve;
	private Vector3 attackObjectMoveVector;
	private Vector3 attackObjectMoveStartPos;
	//按速度移动
	public Vector3 attackObjectMoveSpeed;

	public bool isEffectPlayEnd = false;
	private bool isAllDamageEnd = false;
	private bool isCurDamageEnd = false;
	private bool isBuffEnabled = false;
	private bool isInCollision = false;
	private bool isLifeTimeEnd = false;
	private bool isShowed = false;//伤害的时间另外走，没显示也会造成伤害

	// Update is called once per frame
	public void ManualUpdate()
	{
		if(isFinished) return;

		Update_AttackObjectShow();
		Update_AttackObjectMove();
		Update_Damage();
		Update_Collide();
		

		if(!isEffectPlayEnd)
		{
			if(attackConf.waitForObjectEnd)
			{
				if(eo != null && eo.x2dEffect != null)
				{
					//需要等特效播完
					if(eo.x2dEffect.playTimer >= eo.x2dEffect.duration)
						isEffectPlayEnd = true;
				}
				else
					isEffectPlayEnd = true;
			}
			else
				isEffectPlayEnd = true;
		}

		Update_LifeTime();

		if(isEffectPlayEnd && isAllDamageEnd && !isAttackObjectMoving && isLifeTimeEnd)
			AttackEnd();
	}

	void Update_AttackObjectShow()
	{
		if(isShowed) return;

		showTimer += Time.deltaTime;
		if(showTimer >= attackConf.attackObjectAppearTime)
		{
			if(eo != null && eo.effectObject != null)
				eo.effectObject.gameObject.SetActive(true);
			isShowed = true;
		}
	}

	void Update_AttackObjectMove()
	{
		if(!isAttackObjectMoving || !isShowed) return;

		attackObjectMoveTimer += Time.deltaTime;

		if(attackObjectMoveTimer > attackConf.attackObjectMoveStartTime)
		{
			if(attackConf.attackObjectMoveMode == EmAttackObjectMoveMode.AutoMove_Vector)
			{
				float t = attackObjectMoveTimer - attackConf.attackObjectMoveStartTime;
				eo.effectObject.realPos = attackObjectMoveStartPos +
					attackObjectMoveVector * attackObjectMoveCurve.Evaluate(t / attackObjectMoveDuration);
			}
			else if(attackConf.attackObjectMoveMode == EmAttackObjectMoveMode.AutoMove_Speed)
			{
				eo.effectObject.realPos += attackObjectMoveSpeed * Time.deltaTime;
			}
		}

		if(attackObjectMoveTimer >= attackObjectMoveDuration + attackConf.attackObjectMoveStartTime)
		{
			isAttackObjectMoving = false;
			if(attackConf.damageMode == 2)
			{
				//这时候没有击中目标，特效要强制销毁了
				isAllDamageEnd = true;
			}
		}
	}

	void Update_Damage()
	{
		if(attackConf.damageMode == 2 && !isInCollision) return;

		if(damageIndex < damageConfArray.Length)
		{
			damageTimer += Time.deltaTime;
			DBSkillDamageConf damage = damageConfArray[damageIndex];

			if(!isBuffEnabled)
			{
				if(damageTimer >= damage.buffEnableTime)
				{
					isBuffEnabled = true;
					if(damage.buffID > 0)
						actor.BeBuff(damage.buffID);
				}
			}

			//多久后开始攻击
			if(!isCurDamageEnd && damageTimer >= damage.startTime)
			{
				//进行多次攻击，第一次攻击立即触发
				attackTimer += Time.deltaTime;
				if(attackTimes == 0 || attackTimer >= damage.interval)
				{
					DoDamage(damage);
					attackTimer = 0f;
					attackTimes++;//伤害次数+1
					if(attackTimes >= damage.times)
					{
						isCurDamageEnd = true;
					}
				}
			}

			if(isBuffEnabled && isCurDamageEnd)
			{
				damageTimer = 0f;
				attackTimes = 0;
				damageIndex++;//这次damage完成，下个继续
				isCurDamageEnd = false;
				isBuffEnabled = false;
			}
		}
		else
		{
			isAllDamageEnd = true;
		}
	}

	void Update_LifeTime()
	{
		if(isLifeTimeEnd  || !isShowed) return;

		lifeTimer += Time.deltaTime;
		isLifeTimeEnd = lifeTimer >= attackConf.duration;
	}

	void Update_Collide()
	{
		if(attackConf.damageMode != 2) return;

		//目前只与怪物做碰撞
		List<ActorBase> enemyList = GameMain.curSceneManager.GetEnemyList(actor);
		foreach(ActorBase a in enemyList)
		{
			if(ColliderManager.IsCollision(eo.effectObject.realPos, new Vector3(1, 1, 1),
				a.objectbase.realPos, a.objectcollider.size))
			{
				isInCollision = true;
				isAttackObjectMoving = false;//不让攻击物继续移动了
				isLifeTimeEnd = true;//碰撞后即消失
				eo.x2dEffect.Stop();
				Debug.Log("isInCollision");
				break;
			}
		}

	}

	//
	public void DoDamage(DBSkillDamageConf damage)
	{
		List<ActorBase> enemyList = GameMain.curSceneManager.GetEnemyList(actor);
		if(enemyList == null)
			return;

		for(int i = 0; i < enemyList.Count; i++)
		{
			if(!enemyList[i].isAlive) continue;
			ActorBase enemy = enemyList[i];
			if(!IsInAttackRange(enemy, damage))
				continue;
			enemy.BeAttacked(actor, damage, skill);
			//Debug.Log("Do damage: " + damage.ID);
		}
	}

	public bool IsInAttackRange(ActorBase enemy, DBSkillDamageConf damage)
	{
		//伤害中心
		Vector3 damageCenter = Vector3.zero;
		Vector3 t = damage.damageCenterPos;
		if(damage.damageCenterType == EmDamageCenterType.Attacker)
		{
			if(!actor.objectbase.isFacingRight) t.x = -t.x;
			damageCenter = actor.objectbase.realPos + t;
		}
		else if(damage.damageCenterType == EmDamageCenterType.Target)
			damageCenter = enemy.objectbase.realPos + damage.damageCenterPos;
		else if(damage.damageCenterType == EmDamageCenterType.AttackObject)
			damageCenter = eo.effectObject.realPos + damage.damageCenterPos;
		else
			Debug.LogError("damage.damageCenterType is none");

		//球形区域不用判断
		if(damage.damageArenaType == EmDamageArenaType.FrontArena)
		{
			if(!actor.objectbase.IsInFrontOfMe(enemy.objectbase.realLocalPos))
				return false;
		}
		//伤害区域这样处理是以伤害中心为球心的球
		//todo,中心为Target和AttackObject作为球体处理，Attacker才具有多样化，因为attacker可以获取到朝向。其他的朝向都是未知/变动的。
		//float dist = (damageCenter - enemy.objectbase.realLocalPos).sqrMagnitude;
		//Debug.Log(dist);
		//if(dist > damage.damageDistance * damage.damageDistance)
		//	return false;

		//伤害区域这样处理是以伤害中心为中心的立方体
		return (ColliderManager.IsCollision(damageCenter, new Vector3(damage.damageArenaRadius, damage.damageArenaRadius, damage.damageArenaRadius),
			enemy.objectbase.realLocalPos + new Vector3(0, enemy.actorConf.size.y*0.5f, 0), enemy.actorConf.size));

		//return true;
	}

	public void StartAttack(SkillBase skill, ActorBase actor, DBSkillAttackConf attackConf)
	{
		//Debug.Log("Do attack: " + attackConf.ID);
		isAttackObjectMoving = false;
		isAllDamageEnd = false;
		isEffectPlayEnd = false;

		this.skill = skill;

		damageConfArray = new DBSkillDamageConf[attackConf.damages.Length];
		for(int i = 0; i < attackConf.damages.Length; i++)
			damageConfArray[i] = DBSkillDamageTable.GetRecord(attackConf.damages[i]);

		if(!string.IsNullOrEmpty(attackConf.attackObjectName))
		{
			eo = EffectManager.GetEffect(attackConf.attackObjectName, null);
			eo.effectObject.gameObject.SetActive(attackConf.attackObjectAppearTime <= 0);//需要倒计时显示的话，先隐藏
			X2DEffect effect = eo.x2dEffect;
			if(effect != null)
			{
				effect.Play();
				Vector3 pos = Vector3.zero;
				pos.x = attackConf.attackObjectAppearPos.x;
				pos.y = attackConf.attackObjectAppearPos.y;
				pos = actor.objectbase.GetFaceBasedPos(pos);

				if(attackConf.attackObjectMoveMode == EmAttackObjectMoveMode.FollowCaster)
				{
					eo.effectObject.transform.parent = actor.transform;
					eo.effectObject.transform.localRotation = Quaternion.identity;
					eo.effectObject.realLocalPos = pos;
				}
				else
				{
					eo.effectObject.realPos = actor.objectbase.realLocalPos + pos;
				}
				eo.effectObject.isFacingRight = actor.objectbase.isFacingRight;

				//攻击物体移动
				if(attackConf.attackObjectMoveMode == EmAttackObjectMoveMode.AutoMove_Vector)
				{
					//向量移动
					isAttackObjectMoving = true;
					attackObjectMoveTimer = 0;
					attackObjectMoveDuration = attackConf.attackObjectMoveDuration;
					//
					attackObjectMoveCurve = AnimationCurveCollection.GetAnimationCurve(attackConf.attackObjectMoveVectorCurve);
					attackObjectMoveVector = attackConf.attackObjectMoveVector;
					attackObjectMoveStartPos = eo.effectObject.realLocalPos;
					if(!eo.effectObject.isFacingRight)
						attackObjectMoveVector.x = -attackObjectMoveVector.x;
				}
				else if(attackConf.attackObjectMoveMode == EmAttackObjectMoveMode.AutoMove_Speed)
				{
					//曲线移动
					isAttackObjectMoving = true;
					attackObjectMoveTimer = 0;
					attackObjectMoveDuration = attackConf.attackObjectMoveDuration;
					//
					attackObjectMoveSpeed = attackConf.attackObjectMoveSpeed;
					if(!eo.effectObject.isFacingRight)
						attackObjectMoveSpeed.x = -attackObjectMoveSpeed.x;
				}
			}
		}

		//音效
		if(!string.IsNullOrEmpty(attackConf.soundName))
		{
			actor.audioSource.clip = ResourceLoader.LoadCharactorSound(attackConf.soundName);
			actor.audioSource.Play();
		}

		this.actor = actor;
		this.attackConf = attackConf;
		damageTimer = 0f;
		attackTimer = 0f;
		attackTimes = 0;
		showTimer = 0;
		damageIndex = 0;
		isFinished = false;
		isCurDamageEnd = false;
		isBuffEnabled = false;
		isInCollision = false;
		isLifeTimeEnd = false;
		isShowed = false;
	}

	public void Destroy()
	{
		actor = null;
		eo = null;
	}

	///要求：1特效播完，2特效移动玩，3攻击完成
	public void AttackEnd()
	{
		isFinished = true;
		if(eo != null)
		{
			eo.x2dEffect.Stop();
			EffectManager.KillEffect(eo);
			eo = null;
		}
		skill.OnAttackEnd(this);
	}

	public void Stop()
	{
		AttackEnd();
		Destroy();
	}

	public void DrawGizmos()
	{
		if(damageIndex < damageConfArray.Length)
			Gizmos.DrawSphere(actor.transform.position, damageConfArray[damageIndex].damageArenaRadius);
	}

	public static float GetDamageDuration(DBSkillDamageConf conf)
	{
		return conf.startTime + conf.interval*(conf.times - 1);
	}
}
