using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SkillAttaclManager : MonoBehaviour
{
	public static LinkedList<SkillBase> activeSkillList = new LinkedList<SkillBase>();//活动的
	public static LinkedList<SkillBase> inactiveSkillList = new LinkedList<SkillBase>();//不活动的

	void Update()
	{
		LinkedListNode<SkillBase> node = activeSkillList.First;
		while(node != null)
		{
			if(node.Value.isActive)
				node.Value.ManualUpdate();

			//在这里移除节点
			if(!node.Value.isActive)
			{
				LinkedListNode<SkillBase> removeNode = node;
				node = node.Next;
				activeSkillList.Remove(removeNode);
				inactiveSkillList.AddFirst(removeNode);
				TqmLog.LogSkillAttackManagerFormat("Destory a attack, activeAttackCount = {0}, inactiveAttackCount = {1}", activeSkillList.Count, inactiveSkillList.Count);
			}
			else
				node = node.Next;
		}
	}

	void OnDrawGizmos()
	{
		LinkedListNode<SkillBase> node = activeSkillList.First;
		while(node != null)
		{
			if(node.Value.isActive)
				node.Value.DrawGizmos();
			node = node.Next;
		}
	}

	//没有可用的就创建，先直接返回可用的，以后还需要匹配名字
	public static SkillBase GetSkill(ActorBase actor, int skillID)
	{
		LinkedListNode<SkillBase> node = inactiveSkillList.First;
		SkillBase skill = null;
		if(node != null)
		{
			skill = node.Value;
			inactiveSkillList.RemoveFirst();
			activeSkillList.AddFirst(node);
			TqmLog.LogSkillAttackManagerFormat("Get exist skill, activeAttackCount = {0}, inactiveAttackCount = {1}", activeSkillList.Count, inactiveSkillList.Count);
		}
		if(skill == null)
		{
			skill = new SkillBase();
			activeSkillList.AddFirst(skill);
			TqmLog.LogSkillAttackManagerFormat("Create new skill, activeAttackCount = {0}, inactiveAttackCount = {1}", activeSkillList.Count, inactiveSkillList.Count);
		}
		skill.isActive = true;
		skill.Init(actor, skillID);
		return skill;
	}
}
