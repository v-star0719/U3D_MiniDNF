using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum EmActorStatus
{
	None,
	Idle,
	Walk,
	Run,
	Jump,
	JumpHigh_Up,
	JumpHigh_Down,
	JumpBack,

	//
	CastAttackChargeStart,
	CastAttackCharging,
	CastAttack,

	//
	DoAttack,
	Attack1,
	Attack2,
	Attack3,

	//
	BeAttacked,
	BeAttackedFrozen,
	BeAttackedHitMove,
	BeAttackedHitMoveDist,
	BeAttackedFly,
	BeAttackedToss,
	BeAttackedFallOff,//浮空后的自由落体
	BeAttackedFallDown,//倒地
	BeAttackedLieOnGround,

	//
	BeAttackedFreeze,//冰冻
	BeAttackedMelt,//解冻
	BeAttackedStun,//眩晕

	Parry,//该状态一直持续，知道按键松开
	ParryBeAttacked,//仍然是格挡状态

	Die,//播放死亡动作
	Dead,//已死亡
}

public enum EmActorAnimationName
{
	Idle,
	Walk,
	Run,
	Jump,
	JumpHigh_Up,
	JumpHigh_Down,
	JumpBack,

	CastAttackChargeStart,
	CastAttack,
	Attack1,
	Attack2,
	Attack3,

	BeAttacked,
	BeAttackedFly,
	BeAttackedFallOff,
	BeAttackedFallDown,
	BeAttackedLieOnGround,

	Parry,
	ParryBeAttacked,
}

public enum EmActorFaction
{
	None,
	Teammate,
	Enemy,
	Neutrality
}

public enum EmActorType
{
	None,
	MainPlayer,
	Monster,
	Boss,
}

public class ActorBase : MonoBehaviour
{
	const float FALLDOWN_BOUNCE_SPEED = -40f;//低于该速度不发生反弹
	const float FALLDOWN_BOUNCE_FACOTR = -0.4f;//反弹系数
	public const float STAND_ON_GROUND_THROLD = 5f;//大于该值认为浮空，否则是站地上

	public ObjectBase objectbase;
	public ObjectCollider objectcollider;
	public AudioSource audioSource;

	public EmActorType actorType;
	public EmActorFaction actorFaction;
	public ActorController actorController;
	public EmMonsterType monsterType;

	public ActorAttributeData orgAttribute;//原始属性
	public ActorAttributeData curAttribute;//当前属性
	public DBActorAttributeConf actorConf;
	public float attack1ComboStartTime;
	public float attack2ComboStartTime;

	private EmActorStatus curStatus = EmActorStatus.None;
	private Vector3 moveDirection;
	private float statusDuration = 0;
	private float timer;
	private float curFallSpeed = 0f;

	//private int curAttackIndex;
	[HideInInspector]//不隐藏的话，为null的时候编辑器会创建一个空的出来
	//private DBAttackConf curAttackConf;
	private bool isReadyToCombo = false;
	private int nextAttackDir;//0表示继续当前方向，1表示右，-1表示左

	//被击效果
	private DBSkillDamageConf damageConf;
	private Vector3 hitMoveStartPos;
	private AnimationCurve hitMoveCurve;
	private Vector2 hitMoveSpeed;
	private float damageEffectTime;
	private Vector3 hitMoveVector;
	//
	private EffectObject freezeEffect;
	private EffectObject meltEffect;
	private EffectObject stunEffect;

	//应用动画中的位置变化
	private Vector3 animationStartPos;

	//buff列表，考虑到待触发技能列表，可能需要一个技能管理器
	public List<Buff> buffList = new List<Buff>();

	//技能，考虑到待触发技能列表，可能需要一个技能管理器
	public List<SkillState> activeSkillList = new List<SkillState>();//主动技能列表 
	private SkillBase mySkill;//以后再考虑多个技能同时存在的情况
	//private DBSkillConf attackGroup;
	public SkillStateManager skillStateManager = new SkillStateManager();

	private bool _isLieDown = false;
	public bool isLieDown
	{
		get{
			return _isLieDown;
		}
		set
		{
			if(value == _isLieDown) return;
			_isLieDown = value;
			if(_isLieDown)
			{
				//将人物坐标原点上调半个身高
				Vector3 pos = objectbase.realLocalPos;
				pos.y += actorConf.size.y*0.5f;
				objectbase.realLocalPos = pos;
			}
			else
			{
				//将人物坐标原点下调半个身高
				Vector3 pos = objectbase.realLocalPos;
				pos.y -= actorConf.size.y*0.5f;
				if(pos.y < 0) pos.y = 0;
				objectbase.realLocalPos = pos;
			}
		}
	}

	public float percentHp
	{
		get{
			return curAttribute.hp/actorConf.hp;
		}
	}

	// Use this for initialization
	void Start ()
	{
#if UNITY_EDITOR
		if(!Application.isPlaying) return;
#endif
	}

	//useConfAtt是否使用配表的属性
	public void Init(int actorID, int actorLv, bool useConfAtt, EmActorFaction _actorFaction, EmActorType _actorType)
	{
		actorConf = DBActorAttributeTable.GetRecord(actorID);
		actorFaction = _actorFaction;
		actorType = _actorType;

		if(useConfAtt)
			orgAttribute = TqmDatabase.CreateActorAttributeData(actorConf, actorLv);
		else
			orgAttribute = ActorAttributeDataCollection.GetActorAttributeData(actorID);
		curAttribute = TqmDatabase.CloneActorAttributeData(orgAttribute);

		objectbase = gameObject.GetComponent<ObjectBase>();
		objectcollider = gameObject.GetComponent<ObjectCollider>();
		audioSource = gameObject.AddComponent<AudioSource>();
		objectbase.x2dAnim = ResourceLoader.GetX2dAniamtion(actorConf.animationName, transform);

		foreach(int i in actorConf.skillIdList)
		{
			activeSkillList.Add(skillStateManager.GetSkillState(i));
		}

		objectcollider.size = actorConf.size;

		ChangeStatus(EmActorStatus.Idle);
	}

	public void Reset()
	{
		curAttribute = TqmDatabase.CloneActorAttributeData(orgAttribute);
		ChangeStatus(EmActorStatus.Idle);
	}
	
	// Update is called once per frame
	void Update ()
	{
		#if UNITY_EDITOR
		if(!Application.isPlaying) return;
		#endif

		if(curStatus == EmActorStatus.None) return;

		Update_Buff();
		skillStateManager.ManualUpdate();

		Vector3 pos = Vector3.zero;
		timer += Time.deltaTime;
		switch(curStatus)
		{
		case EmActorStatus.Idle:
			if(objectbase.realLocalPos.y > 0)
			{
				curFallSpeed += curAttribute.falldownAcc * Time.deltaTime;
				pos = objectbase.realLocalPos;
				pos.y += curFallSpeed*Time.deltaTime;
				if(pos.y < 0)
					pos.y = 0;
				objectbase.realLocalPos = pos;
			}
			break;
		case EmActorStatus.Walk:
			SetPosWithJudgeCollide(objectbase.realLocalPos + curAttribute.walkSpeed * moveDirection * Time.deltaTime);
			break;
		case EmActorStatus.Run:
			SetPosWithJudgeCollide(objectbase.realLocalPos + curAttribute.runSpeed * moveDirection * Time.deltaTime);
			break;
		case EmActorStatus.Jump:
			curFallSpeed += Time.deltaTime * curAttribute.falldownAcc;
			pos = objectbase.realLocalPos;
			pos.y = objectbase.realLocalPos.y + curFallSpeed * Time.deltaTime;
			if(pos.y < 0)
			{
				//掉回地面了
				pos.y = 0;
				ChangeStatus(EmActorStatus.Idle);
			}
			SetPosWithJudgeCollide(pos + curAttribute.runSpeed * moveDirection * Time.deltaTime);
			break;
		case EmActorStatus.JumpHigh_Up:
			curFallSpeed += Time.deltaTime * curAttribute.falldownAcc;
			pos = objectbase.realLocalPos;
			pos.y = objectbase.realLocalPos.y + curFallSpeed * Time.deltaTime;
			if(curFallSpeed <= 0)
				ChangeStatus(EmActorStatus.JumpHigh_Down);
			SetPosWithJudgeCollide(pos + curAttribute.runSpeed * moveDirection * Time.deltaTime);
			break;
		case EmActorStatus.JumpHigh_Down:
			curFallSpeed += Time.deltaTime * curAttribute.falldownAcc;
			pos = objectbase.realLocalPos;
			pos.y = objectbase.realLocalPos.y + curFallSpeed * Time.deltaTime;
			if(pos.y < 0)
			{
				//掉回地面了
				pos.y = 0;
				ChangeStatus(EmActorStatus.Idle);
			}
			SetPosWithJudgeCollide(pos + curAttribute.runSpeed * moveDirection * Time.deltaTime);
			break;

		case EmActorStatus.JumpBack:
			curFallSpeed += Time.deltaTime * curAttribute.falldownAcc;
			pos = objectbase.realLocalPos;
			pos.y = objectbase.realLocalPos.y + curFallSpeed * Time.deltaTime;
			if(objectbase.isFacingRight)
				pos.x = objectbase.realLocalPos.x + curAttribute.jumpBackXSpeed * Time.deltaTime;//相比跳跃，多了一个X方向的移动
			else
				pos.x = objectbase.realLocalPos.x - curAttribute.jumpBackXSpeed * Time.deltaTime;//相比跳跃，多了一个X方向的移动
			if(pos.y < 0)
			{
				//掉回地面了
				pos.y = 0;
				ChangeStatus(EmActorStatus.Idle);
			}
			SetPosWithJudgeCollide(pos);
			//objectbase.realLocalPos = pos;// + curAttribute.runSpeed * moveDirection * Time.deltaTime;//相比跳跃，不处理跳跃中移动了
			break;
		
		case EmActorStatus.CastAttackChargeStart:
			if(timer >= statusDuration)
				ChangeStatus(EmActorStatus.CastAttackCharging);
			break;
		case EmActorStatus.CastAttackCharging:
			if(timer >= statusDuration)
				ChangeStatus(EmActorStatus.CastAttack);
			break;
		case EmActorStatus.CastAttack:
			if(timer >= statusDuration)
				ChangeStatus(EmActorStatus.DoAttack);
			break;

		case EmActorStatus.DoAttack:
			if(timer >= statusDuration)
			{
				if(isReadyToCombo){
					DoAttack(mySkill.skillID);
					isReadyToCombo = false;
				}
				else{
					mySkill = null;
					ChangeStatus(EmActorStatus.Idle);
				}
			}
			break;

		case EmActorStatus.BeAttackedFrozen:
			if(timer >= statusDuration)
			{
				if(objectbase.realLocalPos.y > 0)
					ChangeStatus(EmActorStatus.BeAttackedFallOff);
				else
				{
					if(curAttribute.hp > 0)
						ChangeStatus(EmActorStatus.Idle);
					else
						ChangeStatus(EmActorStatus.Die);
				}
			}
			break;

		case EmActorStatus.BeAttackedHitMove:
			if(timer <= damageConf.hitMoveTime)
			{
				pos = objectbase.realLocalPos;
				pos.x += hitMoveSpeed.x * Time.deltaTime;
				pos.y += hitMoveSpeed.y * Time.deltaTime;
				hitMoveSpeed.y += curAttribute.falldownAcc*Time.deltaTime;
				curFallSpeed = hitMoveSpeed.y;
				if(pos.y < 0) pos.y = 0;
				SetPosWithJudgeCollide(pos);
			}
			if(timer >= statusDuration)
			{
				if(objectbase.realLocalPos.y > 0)
					ChangeStatus(EmActorStatus.BeAttackedFallOff);
				else
				{
					if(curAttribute.hp > 0)
						ChangeStatus(EmActorStatus.Idle);
					else
						ChangeStatus(EmActorStatus.Die);
				}
			}
			break;

		case EmActorStatus.BeAttackedHitMoveDist:
			if(timer <= damageConf.hitMoveTime)
			{
				SetPosWithJudgeCollide(hitMoveStartPos + hitMoveVector*hitMoveCurve.Evaluate(timer / damageConf.hitMoveTime));
				if(pos.y < 0) pos.y = 0;
			}
			if(timer >= statusDuration)
			{
				if(objectbase.realLocalPos.y > 0)
					ChangeStatus(EmActorStatus.BeAttackedFallOff);
				else
				{
					if(curAttribute.hp > 0)
						ChangeStatus(EmActorStatus.Idle);
					else
						ChangeStatus(EmActorStatus.Die);
				}
			}
			break;

		case EmActorStatus.BeAttackedFallOff:
			curFallSpeed += curAttribute.falldownAcc * Time.deltaTime;
			pos = objectbase.realLocalPos;
			pos.x += hitMoveSpeed.x*Time.deltaTime;
			pos.y += curFallSpeed*Time.deltaTime;
			if(pos.y < 0)
			{
				hitMoveSpeed.x = 0;
				pos.y = 0;
				if(curFallSpeed > FALLDOWN_BOUNCE_SPEED)
					ChangeStatus(EmActorStatus.BeAttackedLieOnGround);
				else
					curFallSpeed *= FALLDOWN_BOUNCE_FACOTR;
			}
			SetPosWithJudgeCollide(pos);
			break;

		case EmActorStatus.BeAttackedLieOnGround:
			if(timer > statusDuration)
			{
				if(curAttribute.hp > 0)
					ChangeStatus(EmActorStatus.Idle);
				else
					ChangeStatus(EmActorStatus.Die);
			}
			break;

		case EmActorStatus.BeAttackedFallDown:
			if(timer > statusDuration)
			{
				if(curAttribute.hp > 0)
					ChangeStatus(EmActorStatus.Idle);
				else
					ChangeStatus(EmActorStatus.Die);
			}
			break;

		case EmActorStatus.BeAttackedToss:
			ChangeStatus(EmActorStatus.Idle);
			break;

		case EmActorStatus.BeAttackedFreeze:
			if(timer > statusDuration)
			{
				freezeEffect.x2dEffect.Stop();
				EffectManager.KillEffect(freezeEffect);
				freezeEffect = null;
				ChangeStatus(EmActorStatus.BeAttackedMelt);
			}
			break;
		case EmActorStatus.BeAttackedMelt:
			if(timer > statusDuration)
			{
				meltEffect.x2dEffect.Stop();
				EffectManager.KillEffect(meltEffect);
				meltEffect = null;
				if(curAttribute.hp > 0)
					ChangeStatus(EmActorStatus.Idle);
				else
					ChangeStatus(EmActorStatus.Die);
			}
			break;
		case EmActorStatus.BeAttackedStun:
			if(timer > statusDuration)
			{
				stunEffect.x2dEffect.Stop();
				EffectManager.KillEffect(stunEffect);
				stunEffect = null;
				if(curAttribute.hp > 0)
					ChangeStatus(EmActorStatus.Idle);
				else
					ChangeStatus(EmActorStatus.Die);
			}
			break;

		case EmActorStatus.Parry:
		case EmActorStatus.ParryBeAttacked://啥也不用做。。。。
			if(timer > statusDuration)
				ChangeStatus(EmActorStatus.Parry);
			break;

		case EmActorStatus.Die:
			if(timer > statusDuration)
				ChangeStatus(EmActorStatus.Dead);
			break;
		case EmActorStatus.Dead:
			break;
		}

		UpdateAnimationPos();
	}

	//将动画里的位置变化应用上
	public void UpdateAnimationPos()
	{
		if (!objectbase.x2dAnim.curClip.hasPosCurve)
			return;
		SetPosWithJudgeCollide(animationStartPos + objectbase.GetFaceBasedPos(objectbase.x2dAnim.GetCurAnimationPos()));
	}

	public void Update_Buff()
	{
		for(int i=0; i<buffList.Count; i++)
		{
			//放在update前面进行删除，意味着这帧执行完，下一帧再进行删除
			if(!buffList[i].isAlive)
			{
				buffList.RemoveAt(i);
				i--;
				continue;
			}
			buffList[i].ManualUpdate();
		}
	}

	public EmActorStatus actorCurStatus{get{return curStatus;}}
	public bool isIdle{get{return curStatus == EmActorStatus.Idle;}}
	public bool isWalking{get{return curStatus == EmActorStatus.Walk;}}
	public bool isRunning{get{return curStatus == EmActorStatus.Run;}}
	public bool isJumping{get{return curStatus == EmActorStatus.JumpHigh_Up;}}
	public bool isJumpingBack{get{return curStatus == EmActorStatus.JumpBack;}}
	public bool isAttacking
	{
		get
		{
			return curStatus == EmActorStatus.CastAttack ||
				curStatus == EmActorStatus.CastAttackChargeStart ||
				curStatus == EmActorStatus.CastAttackCharging ||
				curStatus == EmActorStatus.DoAttack;
		}
	}
	public bool isBeattacked
	{
		get
		{
			return curStatus == EmActorStatus.BeAttacked ||
				curStatus == EmActorStatus.BeAttackedFallDown ||
				curStatus == EmActorStatus.BeAttackedFallOff ||
				curStatus == EmActorStatus.BeAttackedFly ||
				curStatus == EmActorStatus.BeAttackedFrozen ||
				curStatus == EmActorStatus.BeAttackedHitMove ||
				curStatus == EmActorStatus.BeAttackedHitMoveDist ||
				curStatus == EmActorStatus.BeAttackedLieOnGround ||
				curStatus == EmActorStatus.BeAttackedToss||
				curStatus == EmActorStatus.BeAttackedFreeze ||
				curStatus == EmActorStatus.BeAttackedMelt ||
				curStatus == EmActorStatus.BeAttackedStun;
		}
	}
	public bool isNormalAttacking{get{return curStatus == EmActorStatus.Attack1 || curStatus == EmActorStatus.Attack2 || curStatus == EmActorStatus.Attack3;}}
	public bool isParrying { get { return curStatus == EmActorStatus.Parry || curStatus == EmActorStatus.ParryBeAttacked;} }
	public bool isDieing{get {return curStatus == EmActorStatus.Die;}}
	public bool isDead{get {return curStatus == EmActorStatus.Dead;}}
	public bool isAlive{get {return curStatus != EmActorStatus.Die && curStatus != EmActorStatus.Dead;}}

	public void Walk(Vector3 dir)
	{
		moveDirection = dir;
		if(dir.x < 0) objectbase.isFacingRight = false;
		else if(dir.x > 0) objectbase.isFacingRight = true;
		if(!isWalking) ChangeStatus(EmActorStatus.Walk);
	}

	public void Run(Vector3 dir)
	{
		moveDirection = dir;
		if(dir.x < 0) objectbase.isFacingRight = false;
		else if(dir.x > 0) objectbase.isFacingRight = true;
		if(!isRunning) ChangeStatus(EmActorStatus.Run);
	}

	public void Jump()
	{
		if(!isJumping) ChangeStatus(EmActorStatus.Jump);
	}

	public void JumpBack()
	{
		ChangeStatus(EmActorStatus.JumpBack);
	}

	public void JumpMove(Vector3 dir)
	{
		if(!isJumping) return;
		moveDirection = dir;
		if(dir.x < 0) objectbase.isFacingRight = false;
		else if(dir.x > 0) objectbase.isFacingRight = true;
	}

	public void Idle()
	{
		ChangeStatus(EmActorStatus.Idle);
	}

	public void Die()
	{
		float preHp = curAttribute.hp;
		curAttribute.hp = 0;
		ChangeStatus(EmActorStatus.Die);
		UIBattleHpPanel.instance.ShowMonsterHP(actorConf, orgAttribute.hp, curAttribute.hp, preHp, monsterType);//应该把hp变化放到一个函数里
	}

	//连续攻击处理方式：当前攻击快要完成时，按下攻击键
	public void DoAttack(int skillID)
	{
		//连击处理
		if(mySkill != null && skillID == mySkill.skillID)
		{
			//已有技能在释放中
			EmPlaySKillResult rt = mySkill.PlaySKill(timer, statusDuration, curAttribute.attackSpeed);
			if(rt == EmPlaySKillResult.WaitToCombo)
			{
				isReadyToCombo = true;
				nextAttackDir = 0;
				//三段斩要立即记录按键的方向，切换攻击时立即应用
				if(mySkill.skillID == (int)EmSkillIDSet.TripleSlash)
				{
					if(actorController.curPressKey == KeyCode.RightArrow || actorController.curPressKey == KeyCode.LeftArrow)
						nextAttackDir = actorController.curPressKey == KeyCode.RightArrow ? 1 : -1;
				}
				return;
			}
			else if(rt == EmPlaySKillResult.None)
				return;
			//else if(rt == EmPlaySKillResult.PlayNext)
			//{
			//	//进行后续处理即可
			//}
		}
		else
		{
			//没有技能在释放中
			SkillState state = skillStateManager.GetSkillState(skillID);
			if(!state.isCding)
			{
				mySkill = SkillManager.GetSkill(this, skillID);
				state.StartCd();
			}
			else
			{
				audioSource.clip = ResourceLoader.LoadCharactorSound("effect/zieg_stab_02");
				audioSource.Play();
				Debug.Log("Skill is cding, remain + " + state.curCd + "s");
			}
			if(mySkill == null) return;
		}

		if(nextAttackDir != 0)
		{
			objectbase.isFacingRight = nextAttackDir == 1;
			nextAttackDir = 0;
		}

		if(mySkill.curAttackConf.castTime > 0)
			ChangeStatus(EmActorStatus.CastAttackChargeStart);
		else
			ChangeStatus(EmActorStatus.DoAttack);
	}

	public void DoParry(bool isCancel)
	{
		if(isCancel)
			ChangeStatus(EmActorStatus.Idle);
		else
			ChangeStatus(EmActorStatus.Parry);
	}

	

	public void BeAttacked(ActorBase attacker, DBSkillDamageConf damage, SkillBase skill)
	{
		if(isParrying)
		{
			ChangeStatus(EmActorStatus.ParryBeAttacked);//暂时不掉血了，背击可以破
			return;
		}

		if(isAttacking)
		{
			if(mySkill != null)
			{
				//技能已释放出了
				if(mySkill.curAttackConf.isGod)
					return;//无敌
				else
				{
					mySkill.Stop();
					mySkill = null;//被打断
					isReadyToCombo = false;
				}
			}
			else
			{
				//技能还未释放出，被打断
			}
		}

		//伤害计算
		float damageValue = attacker.curAttribute.attack * damage.damagePercent/100f + damage.damageFixed;
		float preHp = curAttribute.hp;
		curAttribute.hp -= damageValue;
		if(GameMain.curStatus != GameStatus.Tutorial)
		{
			if(curAttribute.hp < 0) curAttribute.hp = 0;
		}
		else
		{
			if(curAttribute.hp < 0) curAttribute.hp = orgAttribute.hp;//教学模式下，满血继续打
		}

		//伤害数字等提示
		Vector3 damageTipPos = objectbase.transform.position;
		if(attacker.objectbase.isFacingRight)
			damageTipPos += new Vector3(actorConf.size.x * 0.7f, actorConf.size.y*0.5f, 0);
		else
			damageTipPos += new Vector3(actorConf.size.x * -0.7f, actorConf.size.y*0.5f, 0);

		UIDamageNumberPanel.ShowTipText(damageTipPos, (int)damageValue);

		if(actorType == EmActorType.MainPlayer)
			UIMainPanel.instance.ShowHp();
		else
			UIBattleHpPanel.instance.ShowMonsterHP(actorConf, orgAttribute.hp, curAttribute.hp, preHp, monsterType);

		//Debug.LogFormat("Be attacked: {0}-->{1}", preHp, curAttribute.hp);

		//转向，面向攻击者
		objectbase.isFacingRight = attacker.objectbase.realPos.x - objectbase.realPos.x > 0;


		damageConf = damage;

		//状态效果是否触发
		EmDamageStatusEffect statusEffect = EmDamageStatusEffect.None;
		if(damageConf.damageStatusEffectProbability >= Random.Range(0, 101))
			statusEffect = damageConf.damageStatusEffect;

		//如果当前是眩晕或者冰冻，则不改变当前效果
		//在changeStatus的时候，保持原来的持续时间，不做修改
		if(actorCurStatus == EmActorStatus.BeAttackedStun)
			statusEffect = EmDamageStatusEffect.Stun;
		else if(actorCurStatus == EmActorStatus.BeAttackedFreeze)
			statusEffect = EmDamageStatusEffect.Freeze;


		//状态效果
		if(statusEffect == EmDamageStatusEffect.Freeze)
			ChangeStatus(EmActorStatus.BeAttackedFreeze);
		else if(statusEffect == EmDamageStatusEffect.Stun)
			ChangeStatus(EmActorStatus.BeAttackedStun);
		else
		{

			//移动效果
			if(damage.damageMoveEffect == EmDamageMoveEffect.Frozen)
				ChangeStatus(EmActorStatus.BeAttackedFrozen);
			else if(damage.damageMoveEffect == EmDamageMoveEffect.HitMove)
			{
				hitMoveSpeed = damage.hitMoveSpeed;//要阻力计算
				ChangeStatus(EmActorStatus.BeAttackedHitMove);
			}
			else if(damage.damageMoveEffect == EmDamageMoveEffect.HitMoveFreezeInAir)
			{
				if(objectbase.realLocalPos.y > 0)
				{
					hitMoveSpeed = Vector2.zero;
					ChangeStatus(EmActorStatus.BeAttackedFrozen);
					Debug.Log("HitMoveFreeze.BeAttackedFreeze");
				}
				else
				{
					hitMoveSpeed = damage.hitMoveSpeed;//要阻力计算
					ChangeStatus(EmActorStatus.BeAttackedHitMove);
				}
			}
			else if(damage.damageMoveEffect == EmDamageMoveEffect.HitMoveDist || damage.damageMoveEffect == EmDamageMoveEffect.HitMoveTo)
			{
				//去除移动速度
				hitMoveSpeed = Vector2.zero;
				if(damage.damageMoveEffect == EmDamageMoveEffect.HitMoveDist)
				{
					hitMoveCurve = AnimationCurveCollection.GetAnimationCurve(damageConf.hitMoveVectorCurveName);
					hitMoveVector = damage.hitMoveVector;
					if(!attacker.objectbase.isFacingRight) hitMoveVector.x = -hitMoveVector.x;
				}
				else
				{
					hitMoveCurve = AnimationCurveCollection.GetAnimationCurve(damageConf.hitMoveToCurveName);
					Vector3 t = Vector3.zero;
					if(attacker.objectbase.isFacingRight)
						t.x = damage.hitMoveTo.x + actorConf.size.x*0.4f;
					else
						t.x = -damage.hitMoveTo.x - actorConf.size.x*0.4f;
					if(skill.skillConf.skillID == (int)EmSkillIDSet.GrabBlast)
					{
						//如果是嗜魂之手的话，对于高度低抓取位置的怪，头部对齐到目标位置
						if(actorConf.size.y < damage.hitMoveTo.y)//目标怪的高度低于抓取的位置，高度要升高
						{
							t.y = damage.hitMoveTo.y - actorConf.size.y*0.8f;
						}
					}
					else
					{
						//默认是怪物中心对齐到指定地点，如果怪物中心高于指定点，则只平移
						if(actorConf.size.y*0.5f < damage.hitMoveTo.y)
						{
							t.y = damage.hitMoveTo.y - actorConf.size.y*0.5f;
						}
					}
					t.z -= 10;//将怪放到玩家下面，避免怪遮住手
					hitMoveVector = attacker.objectbase.realPos + t - objectbase.realPos;
					Debug.Log(hitMoveVector);
				}

				hitMoveStartPos = objectbase.realLocalPos;
				ChangeStatus(EmActorStatus.BeAttackedHitMoveDist);
			}
			else if(damage.damageMoveEffect == EmDamageMoveEffect.HitFallDown)
			{
				ChangeStatus(EmActorStatus.BeAttackedFallDown);
			}
			else
				Debug.LogError("Unkown damage result");
		}

		if(!attacker.objectbase.isFacingRight)
			hitMoveSpeed.x = -hitMoveSpeed.x;

		//音效
		if(!string.IsNullOrEmpty(damageConf.hitSoundName))
		{
			audioSource.clip = ResourceLoader.LoadCharactorSound(damageConf.hitSoundName);
			audioSource.Play();
		}

		//击中效果，目前只有鬼剑士普攻有
		if(attacker.actorConf.actorID < 1004 && damage.ID/10 <= 1002)
		{
			EffectObject eo = EffectManager.GetEffect("Common_HitEffect_Slash" + (damage.ID%10), transform);
			Vector3 pos = objectbase.realLocalPos;
			pos.y += actorConf.size.y * 0.5f;
			eo.effectObject.realPos = pos;
			eo.autoDestroy = true;
			eo.x2dEffect.Play();
		}
	}

	private void BeAttackedEnd()
	{
		hitMoveSpeed = Vector2.zero;
		damageConf = null;
		hitMoveVector = Vector3.zero;
		//Debug.Log("BeAttackedEnd");
	}

	private void ChangeStatus(EmActorStatus s)
	{
		if(curStatus == s && s != EmActorStatus.DoAttack)
			Debug.LogWarning("Reset staus: " + s);
		timer = 0;
		switch(s)
		{
		case EmActorStatus.Idle:
			statusDuration = PlayAnimation(EmActorAnimationName.Idle);
			break;

		case EmActorStatus.Walk:
			statusDuration = PlayAnimation(EmActorAnimationName.Walk);
			break;

		case EmActorStatus.Run:
			statusDuration = PlayAnimation(EmActorAnimationName.Run);
			break;

		case EmActorStatus.Jump:
			moveDirection = Vector3.zero;
			curFallSpeed = curAttribute.jumpYSpeed;
			PlayAnimation(EmActorAnimationName.Jump);
			break;

		case EmActorStatus.JumpHigh_Up:
			moveDirection = Vector3.zero;
			curFallSpeed = curAttribute.jumpYSpeed;
			PlayAnimation(EmActorAnimationName.JumpHigh_Up);
			break;

		case EmActorStatus.JumpHigh_Down:
			moveDirection = Vector3.zero;
			PlayAnimation(EmActorAnimationName.JumpHigh_Down);
			break;

		case EmActorStatus.JumpBack:
			moveDirection = Vector3.zero;
			curFallSpeed = curAttribute.jumpBackYSpeed;
			statusDuration = PlayAnimation(EmActorAnimationName.JumpBack);
			break;

		case EmActorStatus.CastAttackChargeStart:
			statusDuration = PlayAnimation(EmActorAnimationName.CastAttackChargeStart);
			break;
		case EmActorStatus.CastAttackCharging:
			statusDuration = mySkill.curAttackConf.castTime;
			break;
		case EmActorStatus.CastAttack:
			statusDuration = PlayAnimation(EmActorAnimationName.CastAttack);
			break;

		case EmActorStatus.DoAttack:
			bool canSpeedUp =  mySkill.skillID <= 100003 || mySkill.skillID == 100101;//就这么几个技能动作受加速影响
			statusDuration = PlayAnimation(mySkill.curAttackConf.actorClipName, canSpeedUp);
			mySkill.DoCurAttack();
			break;

		case EmActorStatus.BeAttackedFrozen:
		case EmActorStatus.BeAttackedHitMove:
		case EmActorStatus.BeAttackedHitMoveDist:
		case EmActorStatus.BeAttackedToss:
			if(objectbase.realLocalPos.y > STAND_ON_GROUND_THROLD)
				PlayAnimation(damageConf.actorBeHitFlyInAirAnima);
			else
				PlayAnimation(damageConf.actorBeHitStandOnGroundAnima);
			statusDuration = damageConf.hitMoveTime;
			break;
		case EmActorStatus.BeAttackedFallOff:
			PlayAnimation(EmActorAnimationName.BeAttackedFly);//横躺着"飞"下来
			break;
		case EmActorStatus.BeAttackedLieOnGround:
			PlayAnimation(EmActorAnimationName.BeAttackedLieOnGround);
			statusDuration = SkillAttack.GetDamageDuration(damageConf);
			break;

		case EmActorStatus.BeAttackedFallDown:
			statusDuration = PlayAnimation(EmActorAnimationName.BeAttackedFallDown);
			break;

		case EmActorStatus.BeAttackedFreeze:
			PlayAnimation(EmActorAnimationName.BeAttacked);
			if(actorCurStatus != EmActorStatus.BeAttackedFreeze)
			{
				//如果当前正处于冰冻状态，则不用播
				statusDuration = damageConf.damageStatusEffectDuration;
				freezeEffect = EffectManager.GetEffect(GameConfig.effect_Common_Freeze, transform);
				freezeEffect.effectObject.realLocalPos = new Vector3(0, -10, -10);
				freezeEffect.x2dEffect.Play();
			}
			break;
		case EmActorStatus.BeAttackedMelt:
			PlayAnimation(EmActorAnimationName.BeAttacked);
			meltEffect = EffectManager.GetEffect(GameConfig.effect_Common_Melt, transform);
			meltEffect.x2dEffect.Play();
			meltEffect.effectObject.realLocalPos = new Vector3(0, -10, -10);
			statusDuration = meltEffect.x2dEffect.duration;
			break;
		case EmActorStatus.BeAttackedStun:
			PlayAnimation(EmActorAnimationName.BeAttacked);
			if(actorCurStatus != EmActorStatus.BeAttackedStun)
			{
				statusDuration = damageConf.damageStatusEffectDuration;
				stunEffect = EffectManager.GetEffect(GameConfig.effect_Common_Stun, transform);
				stunEffect.effectObject.realLocalPos = new Vector3(0, 120, -10);
				stunEffect.x2dEffect.Play();
			}
			break;

		case EmActorStatus.Parry:
			PlayAnimation(EmActorAnimationName.Parry);
			statusDuration = float.MaxValue;
			break;
		case EmActorStatus.ParryBeAttacked:
			statusDuration = PlayAnimation(EmActorAnimationName.ParryBeAttacked);
			break;

		case EmActorStatus.Die:
			if(actorType == EmActorType.MainPlayer)//主角尸体不消失，播放倒地动作
			{
				statusDuration = 0.5f;
				PlayAnimation(EmActorAnimationName.BeAttackedFallDown);
				if(actorConf.actorID == 1001)
				{
					audioSource.clip = ResourceLoader.LoadCharactorSound("sm_die");//先这样直接写死了
					audioSource.Play();
				}
			}
			else
			{
				statusDuration = 0.1f;
			}
			break;
		case EmActorStatus.Dead:
			statusDuration = float.MaxValue;
			if(actorType == EmActorType.MainPlayer)//主角尸体不消失
			{
				;
			}
			else
			{
				//尸体消失，炸裂
				objectbase.x2dAnim.Stop();
				EffectObject eo = EffectManager.GetEffect(GameConfig.effect_MonsterDie_1, transform);
				MonsterDieEffect mdf = eo.x2dEffect.GetComponent<MonsterDieEffect>();
				eo.effectObject.realLocalPos = Vector3.zero;
				mdf.Play(eo);
			}
			GameMain.curSceneManager.ActorDead(this);
			break;
		}
		curStatus = s;
	}

	///返回动画持续时间
	private float PlayAnimation(EmActorAnimationName animationName)
	{
		animationStartPos = objectbase.realLocalPos;
		isLieDown = animationName == EmActorAnimationName.BeAttackedLieOnGround ||
			animationName == EmActorAnimationName.BeAttackedFly;
		objectbase.x2dAnim.Play(animationName.ToString(), 1);
		return objectbase.x2dAnim.playTime;
	}

	///返回动画持续时间
	///只有攻击动画，会受攻击速度的影响
	private float PlayAnimation(string animationName, bool isAttackAnima = false)
	{
		animationStartPos = objectbase.realLocalPos;
		isLieDown = animationName == EmActorAnimationName.BeAttackedLieOnGround.ToString() ||
			animationName == EmActorAnimationName.BeAttackedFly.ToString();
		float speedUp = 1;
		if(isAttackAnima)
			speedUp = curAttribute.attackSpeed;
		objectbase.x2dAnim.Play(animationName, speedUp);
		return objectbase.x2dAnim.playTime;
	}

	public int GetCurSkillID()
	{
		if(mySkill != null) return mySkill.skillID;
		return 0;
	}

	public void BeBuff(int buffId)
	{
		DBBuffConf conf = DBBuffTable.GetRecord(buffId);
		Buff buff = new Buff();
		buff.Start(this, conf);
		buffList.Add(buff);
	}

	//提前判断走完后各个方向是否会造成碰撞，沿不会碰撞的方向走
	public void SetPosWithJudgeCollide(Vector3 newPos)
	{
		Vector3 oldPos = objectbase.realLocalPos;
		Vector3 tempPos = oldPos;
		Vector3 targetPos = oldPos;

		//因为只要有一维不层叠就不会碰撞，就可以走过去

		tempPos.x = newPos.x;
		objectbase.realLocalPos = tempPos;
		if(!ColliderManager.IsInCollision(objectbase))
			targetPos.x = newPos.x;

		tempPos.x = oldPos.x;
		tempPos.y = newPos.y;
		objectbase.realLocalPos = tempPos;
		if(!ColliderManager.IsInCollision(objectbase))
			targetPos.y = newPos.y;

		tempPos.y = oldPos.y;
		tempPos.z = newPos.z;
		objectbase.realLocalPos = tempPos;
		if(!ColliderManager.IsInCollision(objectbase))
			targetPos.z = newPos.z;

		objectbase.realLocalPos = targetPos;
	}
}
