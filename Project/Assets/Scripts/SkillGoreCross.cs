using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class SkillGoreCross : SkillBase 
{
	//提高第一击的移动速度
	protected override void AfterDoCurAttack(SkillAttack curAttack)
	{
		if(attackList.Count > 1)
		{
			SkillAttack preAttack = attackList[0];
			preAttack.AttackEnd();
		}
	}
}
