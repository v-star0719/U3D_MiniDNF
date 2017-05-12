using UnityEngine;
using System.Collections;
using System.Collections.Generic;

///这个不是导出表生成的，而是运行中生成的
public class DBSkillConf
{
	public int skillID;
	public List<DBSkillAttackConf> attackList = new List<DBSkillAttackConf>();
}

//将攻击
public class DBSkillTable
{
	//攻击表，将子攻击合并
	public static Dictionary<int, DBSkillConf> attackDict;

	public static void Init()
	{
		if(DBSkillAttackTable.instance == null)
		{
			Debug.LogError("DBAttackTable has not initalized");
			return;
		}
		if(attackDict != null)
		{
			Debug.LogError("DBAttackTableWrap has been initalized");
			return;
		}

		attackDict = new Dictionary<int, DBSkillConf>();
		DBSkillAttackConf[] recordArray = DBSkillAttackTable.instance.recordArray;
		DBSkillConf skill;
		for(int i=0; i<recordArray.Length; i++)
		{
			DBSkillAttackConf conf = recordArray[i];
			if(!attackDict.TryGetValue(conf.skillID, out skill))
			{
				skill = new DBSkillConf();
				skill.skillID = conf.skillID;
				attackDict.Add(conf.skillID, skill);
			}
			skill.attackList.Add(conf);
		}

		/*log
		foreach(DBAttackGroup g  in attackDict.Values)
		{
			Debug.Log("Group " + g.attackID);
			for(int i = 0; i < g.childAttackList.Count; i++)
			{
				Debug.Log("   " + i + " " + g.childAttackList[i].ID);
			}
		}
		 * */
	}

	//获取记录，如果不存在返回null
	public static DBSkillConf GetRecord(int skillID, bool errorMsg = true)
	{
		if(attackDict == null)
		{
			Debug.LogError("DBAttackTableWrap is not init");
			return null;
		}
		DBSkillConf record = null;
		if(attackDict.TryGetValue(skillID, out record))
			return record;
		if(errorMsg)
			Debug.LogError("Skill is not in DBAttackTableWrap: " + skillID);
		return null;
	}
}
