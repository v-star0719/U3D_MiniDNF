using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TqmLog
{
	public static bool enableLogSkillAttackManager = true;
	public static bool enableLogSkillManager = true;

	public static void LogSkillAttackManager(string str)
	{
		if(enableLogSkillAttackManager) Debug.Log("[SkillAttackManager]" + str);
	}
	public static void LogSkillAttackManagerFormat(string str, params object[] args)
	{
		if(enableLogSkillAttackManager) Debug.LogFormat("[SkillAttackManager]" + str, args);
	}

	public static void LogSkillManager(string str)
	{
		if(enableLogSkillManager) Debug.Log("[SkillManager] " + str);
	}
	public static void LogSkillManagerFormat(string str, params object[] args)
	{
		if(enableLogSkillManager) Debug.LogFormat("[SkillManager] " + str, args);
	}
}
