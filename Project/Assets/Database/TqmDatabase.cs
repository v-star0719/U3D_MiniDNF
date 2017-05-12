using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//只是在游戏中记录表
public class TqmDatabase : MonoBehaviour
{
	public GameObject[] tableArray;
	
	public static TqmDatabase instance;
	
	void Awake(){
		instance = this;
		Init();
	}
	void OnDestroy(){
		instance = null;
	}
	
	public void Init()
	{
		for(int i=0; i<tableArray.Length; i++)
		{
			GameObject go = GameObject.Instantiate(tableArray[i]);
			go.transform.parent = transform;
		}

		DBSkillTable.Init();
	}

	public static ActorAttributeData CloneActorAttributeData(ActorAttributeData org)
	{
		ActorAttributeData newConf = new ActorAttributeData();
		newConf.actorID = org.actorID;
		newConf.hp = org.hp;
		newConf.walkSpeed = org.walkSpeed;
		newConf.runSpeed = org.runSpeed;
		newConf.normalAttackDistance = org.normalAttackDistance;
		newConf.jumpHeight = org.jumpHeight;
		newConf.jumpYSpeed = org.jumpYSpeed;
		newConf.jumpBackXSpeed = org.jumpBackXSpeed;
		newConf.jumpBackYSpeed = org.jumpBackYSpeed;
		newConf.falldownAcc = org.falldownAcc;
		newConf.attackSpeed = org.attackSpeed;
		newConf.attack = org.attack;
		return newConf;
	}

	public static ActorAttributeData CreateActorAttributeData(DBActorAttributeConf conf, int actorLv)
	{
		actorLv--;//最终值是基础值 + 成长*(lv-1)

		ActorAttributeData newConf = new ActorAttributeData();
		newConf.actorID = conf.actorID;
		//
		newConf.hp = conf.hp + conf.hpGrowth * actorLv;
		newConf.attack = conf.attack + conf.attackGrowth * actorLv;
		newConf.mitigation = conf.mitigation + conf.mitigation * actorLv;
		newConf.defense = conf.defense + conf.defenseGrowth * actorLv;
		//
		newConf.walkSpeed = conf.walkSpeed;
		newConf.runSpeed = conf.runSpeed;
		newConf.normalAttackDistance = conf.normalAttackDistance;
		newConf.jumpHeight = conf.jumpHeight;
		newConf.jumpYSpeed = conf.jumpYSpeed;
		newConf.jumpBackXSpeed = conf.jumpBackXSpeed;
		newConf.jumpBackYSpeed = conf.jumpBackYSpeed;
		newConf.falldownAcc = conf.falldownAcc;
		newConf.attackSpeed = conf.attackSpeed;
		return newConf;
	}
}