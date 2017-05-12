using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SkillHellBenter : SkillBase 
{
	public const string growingSwordBloodEffectName = "HellBenter_GrowingSwordPart";
	public const float growingSwordBloodStartPos = -53f;
	public const float growingSwordBloodGap = 40f;

	private EffectObject[] growingSwardArray = new EffectObject[5];//一共5截
	private int curGrowStep = 0;

	//进入第二阶段后，积攒血气，curAttackIndex == 1
	//初始就有一截，后面还差4截，满5截后转换为血刃
	//
	//EffectFly 组件

	protected override void LateManualUpdate()
	{
		//第二击完成后，显示第一截剑体
		if(curAttackIndex == 1 && curGrowStep == 0)
		{
			SkillAttack attack = attackList[curAttackIndex];
			//if(attack.isEffectPlayEnd)
			{
				EffectObject eo = EffectManager.GetEffect(growingSwordBloodEffectName, actor.transform);
				eo.effectObject.realLocalPos = attack.eo.effectObject.realLocalPos + new Vector3(0, growingSwordBloodStartPos, 0);
				growingSwardArray[curGrowStep++] = eo;
			}
		}
	}

	protected override void AfterDoCurAttack(SkillAttack curAttack)
	{
		
	}
}
