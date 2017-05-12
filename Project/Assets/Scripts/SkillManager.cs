using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SkillManager : MonoBehaviour
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
				TqmLog.LogSkillManagerFormat("Destory a attack, activeSkillCount = {0}, inactiveSkillCount = {1}", activeSkillList.Count, inactiveSkillList.Count);
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

	//没有可用的就创建，先直接返回可用的
	public static SkillBase GetSkill(ActorBase actor, int skillID)
	{
		LinkedListNode<SkillBase> node = inactiveSkillList.First;
		SkillBase skill = null;
		while(node != null)
		{
			if(node.Value.skillID == skillID)
			{
				skill = node.Value;
				inactiveSkillList.Remove(node);
				activeSkillList.AddFirst(node);
				TqmLog.LogSkillManagerFormat("Get exist skill, activeSkillCount = {0}, inactiveSkillCount = {1}", activeSkillList.Count, inactiveSkillList.Count);
				break;
			}
			node = node.Next;
		}
		if(skill == null)
		{
			if(skillID == (int)EmSkillIDSet.GoreCross_Berserker)
				skill = new SkillGoreCross();
			else if(skillID == (int)EmSkillIDSet.HellBenter)
				skill = new SkillHellBenter();
			else
				skill = new SkillBase();
			activeSkillList.AddFirst(skill);
			TqmLog.LogSkillManagerFormat("Create new skill, activeSkillCount = {0}, inactiveSkillCount = {1}", activeSkillList.Count, inactiveSkillList.Count);
		}
		skill.isActive = true;
		skill.Init(actor, skillID);
		return skill;
	}
}
