using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SkillState 
{
	public EmSkillIDSet skillName;
	public int skillID;
	public float curCd;
	public DBSkillInfoConf skillInfoConf;
	
	public void ManualUpdate()
	{
		if(curCd > 0)
		{
			curCd -= Time.deltaTime;
			if(curCd <= 0)
				curCd = 0;
		}
	}

	public bool isCding
	{
		get { return curCd > 0; }
	}
	public float cdingPercent
	{
		get { return curCd / skillInfoConf.cd; }
	}

	public void StartCd()
	{
		curCd = skillInfoConf.cd;
	}
}


public class SkillStateManager
{
	public List<SkillState> skillList = new List<SkillState>();

	public void ManualUpdate()
	{
		for(int i=0; i<skillList.Count; i++)
			skillList[i].ManualUpdate();
	}

	///如果没有，则创建新的
	///也就是说，永不会返回null
	public SkillState GetSkillState(int skillID)
	{
		for(int i=0; i<skillList.Count; i++)
		{
			if(skillList[i].skillID == skillID)
				return skillList[i];
		}

		SkillState s = new SkillState();
		s.curCd = 0;
		s.skillID = skillID;
		s.skillInfoConf = DBSkillInfoTable.GetRecord(s.skillID);
		skillList.Add(s);
		return s;
	}
}