using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SkillAIDriver : MonoBehaviour
{
	private List<SkillBase> skillList = new List<SkillBase>();

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		for(int i=0; i<skillList.Count; i++)
		{
			SkillBase skill = skillList[i];
			//放在ManualUpdate的前面，不当帧移除，下一帧再移除
			if(!skill.isActive)
			{
				skillList.RemoveAt(i);
				Debug.LogFormat("Remove skill index = {0}, runningSkillCount = {1}", i, skillList.Count);
				i--;
				continue;
			}
				
			skill.ManualUpdate();
		}
	}
}
