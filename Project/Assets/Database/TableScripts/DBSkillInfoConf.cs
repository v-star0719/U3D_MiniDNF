using UnityEngine;

[System.Serializable]
public class DBSkillInfoConf
{
	///技能id
	public int skillID;
	///技能名称
	public string skillNameText;
	///图标
	public int icon;
	///冷却时间
	public float cd;
	///释放时间
	public float castTime;
	///释放组合键
	public string castCombianKey;
	///说明
	public string instruction;
}