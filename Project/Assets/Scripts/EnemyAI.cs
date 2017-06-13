using UnityEngine;
using System.Collections;
using System.Collections.Generic;


//发呆后有一定概率进行巡视（乱走）
//如果发现敌人，则进行攻击

public class EnemyAI : MonoBehaviour 
{
	enum EmMyStatus
	{
		Idle,
		RandomMoveDirection,
		MoveToTarget,
		RandomMoveDirectionAfterAttack,
		AttackPrepair,
		DoAttack,
		NormalAttack,
		CastSkill,
		BeAttacked,
	}

	public DBActorAiConf aiConf;
	public ActorBase actor;

	private EmMyStatus curStatus;
	private float statusDuration;
	private float timer;
	
	private Vector3 randomMoveDir;
	
	private float lastAttackTime = 0;//上次攻击的时刻，限制连续两轮攻击的时间间隔
	private float curNormalAttack = 0;

	private ActorBase attackTarget;
	private ObjectBase moveToTarget;

	private int canCastSkillId = 0;

	public void StartAI(ActorBase actor)
	{
		this.actor = actor;
		aiConf = DBActorAiTable.GetRecord(actor.actorConf.actorID);
		ChangeStatus(EmMyStatus.Idle);
	}

	// Update is called once per frame
	void Update ()
	{
		if(UIManager.instance.loadingPanel.gameObject.activeSelf) return;//进入游戏后再活动

		timer += Time.deltaTime;

		if(actor.isBeattacked)
		{
			ChangeStatus(EmMyStatus.BeAttacked);
			return;
		}

		if(actor.isDead || actor.isDieing)
		{
			return;
		}

		switch(curStatus)
		{
		case EmMyStatus.Idle:
			if(timer >= statusDuration)
			{
				int r = Random.Range(0, 101);
				if(r < aiConf.idleRandom)
				{ }
				else if(r < aiConf.idleRandom + aiConf.moveRandom)
					ChangeStatus(EmMyStatus.RandomMoveDirection);
			}

			if(Patrol()) MoveToTarget();

			break;
		case EmMyStatus.RandomMoveDirection:
		case EmMyStatus.RandomMoveDirectionAfterAttack:
			actor.Walk(randomMoveDir);
			if(timer >= statusDuration)
				ChangeStatus(EmMyStatus.Idle);

			if(curStatus == EmMyStatus.RandomMoveDirection)
				if(Patrol()) MoveToTarget();
			break;
		case EmMyStatus.MoveToTarget:
			Vector3 dir = moveToTarget.realPos - actor.objectbase.realPos;
			dir.y = 0;
			if(dir.x < 0) dir.x = -1;
			if(dir.x > 0) dir.x = 1;
			if(dir.z > 0) dir.z = 1;
			if(dir.z < 0) dir.z = -1;
			actor.Walk(dir);
			if(Vector3.SqrMagnitude(actor.objectbase.realPos - moveToTarget.realPos) < actor.actorConf.normalAttackDistance*actor.actorConf.normalAttackDistance)
			{
				ChangeStatus(EmMyStatus.AttackPrepair);
			}
			else if(timer > aiConf.traceDuration)
			{
				ChangeStatus(EmMyStatus.Idle);
			}
			break;

		case EmMyStatus.AttackPrepair:
			if(timer >= aiConf.attackPrepairDuration)
			{
				if(attackTarget != null && attackTarget.isAlive && 
				   (attackTarget.objectbase.realPos - actor.objectbase.realPos).sqrMagnitude <= actor.actorConf.normalAttackDistance*actor.actorConf.normalAttackDistance)
					DoAttack();
				else
					ChangeStatus(EmMyStatus.Idle);
			}

			break;

		case EmMyStatus.NormalAttack:

			//第一次立即触发
			if((timer >= aiConf.normalAttackInterval || curNormalAttack == 0) && curNormalAttack < aiConf.normalAttackTimes)
			{
				timer = 0f;
				curNormalAttack++;
				actor.DoAttack(actor.actorConf.normalAttackID);
			}

			if(curNormalAttack >= aiConf.normalAttackTimes && !actor.isAttacking)
			{
				lastAttackTime = Time.realtimeSinceStartup;
				ChangeStatus(EmMyStatus.RandomMoveDirectionAfterAttack);//打完就跑
			}
			break;

		case EmMyStatus.CastSkill:
			if(!actor.isAttacking)
				ChangeStatus(EmMyStatus.RandomMoveDirectionAfterAttack);//打完就跑
			break;

		case EmMyStatus.BeAttacked:
			if(actor.isIdle)
				ChangeStatus(EmMyStatus.Idle);
			break;
		}
	}

	void ChangeStatus(EmMyStatus newStatus)
	{
		timer = 0f;
		curStatus = newStatus;
		switch(newStatus)
		{
		case EmMyStatus.Idle:
			actor.Idle();
			statusDuration = Random.Range(0, aiConf.idlMaxDuration);
			break;

		case EmMyStatus.RandomMoveDirection:
		case EmMyStatus.RandomMoveDirectionAfterAttack:
			Vector3[] dirs = new Vector3[]
			{
				new Vector3(0, 0, 1),
				new Vector3(1, 0, 1),
				new Vector3(1, 0, 0),
				new Vector3(1, 0, -1),
				new Vector3(0, 0, -1),
				new Vector3(-1, 0, -1),
				new Vector3(-1, 0, 0),
				new Vector3(-1, 0, 1),
			};
			randomMoveDir = dirs[Random.Range(0, 8)];
			statusDuration = aiConf.moveMaxDuratoin;
			break;

		case EmMyStatus.MoveToTarget:
			break;

		case EmMyStatus.AttackPrepair:
			actor.Idle();
			statusDuration = aiConf.attackPrepairDuration;
			break;

		case EmMyStatus.NormalAttack:
			curNormalAttack = 0;
			break;

		case EmMyStatus.CastSkill:
			actor.DoAttack(canCastSkillId);
			break;

		case EmMyStatus.BeAttacked:
			break;
		}
	}

	void MoveToTarget()
	{
		//找最近的敌人，移动过去进行攻击
		ActorBase actor = GetNearestAttackTarget();
		if(actor != null)
		{
			attackTarget = actor;
			moveToTarget = actor.objectbase;
			ChangeStatus(EmMyStatus.MoveToTarget);
		}
		else
			ChangeStatus(EmMyStatus.Idle);
	}

	void DoAttack()
	{
		//如果有技能，则放技能，否则进行普攻
		canCastSkillId = 0;
		foreach(SkillState s in actor.activeSkillList)
		{
			if(!s.isCding)
			{
				canCastSkillId = s.skillID;
				break;
			}
		}

		if(canCastSkillId > 0)
			ChangeStatus(EmMyStatus.CastSkill);
		else
			ChangeStatus(EmMyStatus.NormalAttack);
	}


	//巡逻，发现敌人返回true，否则返回false
	bool Patrol()
	{
		if(Time.realtimeSinceStartup - lastAttackTime < aiConf.attackInterval)
			return false;

		List<ActorBase> enemyList = GameMain.curSceneManager.GetEnemyList(actor);
		if(enemyList == null)
			return false;
		
		for(int i=0; i<enemyList.Count; i++)
		{
			if(!enemyList[i].isAlive) continue;
			float d = Vector3.SqrMagnitude(actor.objectbase.realPos - enemyList[i].objectbase.realPos);
			if(d < actor.actorConf.patrolRadius * actor.actorConf.patrolRadius)
			{
				return true;
			}
		}
		return false;
	}

	ActorBase GetNearestAttackTarget()
	{
		List<ActorBase> enemyList = GameMain.curSceneManager.GetEnemyList(actor);
		if(enemyList == null)
			return null;

		float minSqrDist = float.MaxValue;
		ActorBase minDistActor = null;
		for(int i=0; i<enemyList.Count; i++)
		{
			if(!enemyList[i].isAlive) continue;
			float d = Vector3.SqrMagnitude(actor.objectbase.realPos - enemyList[i].objectbase.realPos);
			if(d < minSqrDist)
			{
				minSqrDist = d;
				minDistActor = enemyList[i];
			}
		}
		return minDistActor;
	}
}
