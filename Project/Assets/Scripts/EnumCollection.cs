using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnumCollection 
{
	
}

//用来在代码中使用技能ID，只把用到的技能放过来
public enum EmSkillIDSet
{
	//鬼剑士通用技能
	None,
	NormalAttack = 100001,
	UpStrike = 100002,
	VaneSlash = 100004,
	GeroCross = 100003,
	TripleSlash = 100005,
	JumpBack = 100006,
	Parry = 100007,

	//狂战士
	GoreCross_Berserker = 100101,
	BlastBlood = 100103,
	OutrageBreak = 100104,
	GrabBlast = 100105,
	HellBenter = 100106,
	GiveBlood = 100107,
}
