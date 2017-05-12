using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UISkillTipCtrl : MonoBehaviour 
{
	public UILabel nameLabel;
	public UILabel cdLabel;
	public UILabel castTimeLabel;
	public UILabel castCombinKeyLabel;
	public UILabel attributeLabel;
	public UILabel descLabel;

	// Use this for initialization
	void Start ()
 	{
	}

	// Update is called once per frame
	void Update ()
	{
	}

	public void Show(DBSkillInfoConf skillInfoConf)
	{
		nameLabel.text = skillInfoConf.skillNameText;
		cdLabel.text = skillInfoConf.cd + "秒";
		castTimeLabel.text = skillInfoConf.castTime == 0 ? "瞬发" : skillInfoConf.castTime + "秒";
		castCombinKeyLabel.text = skillInfoConf.castCombianKey;
		descLabel.text = skillInfoConf.instruction;

		DBSkillConf skillConf = DBSkillTable.GetRecord(skillInfoConf.skillID, false);

		if(skillConf == null)
		{
			//没有伤害的技能
			attributeLabel.text = "";
			return;
		}

		List<DBSkillDamageConf> damageList = new List<DBSkillDamageConf>();
		//跳过伤害为0的攻击
		for(int i=0; i<skillConf.attackList.Count; i++)
		{
			for(int j=0; j<skillConf.attackList[i].damages.Length; j++)
			{
				DBSkillDamageConf damageConf = DBSkillDamageTable.GetRecord(skillConf.attackList[i].damages[j]);
				if(damageConf.damagePercent > 0)
					damageList.Add(damageConf);
			}
		}

		if(damageList.Count == 0)
			attributeLabel.text = "";
		else if(damageList.Count == 1)
			attributeLabel.text = string.Format("造成{0}%攻击力的伤害", damageList[0].damagePercent);
		else
		{
			string text = "";
			for(int i=0; i<damageList.Count; i++)
			{
				DBSkillDamageConf damageConf = damageList[i];
				string damageText = "";
				if(damageConf.times <= 1)
					damageText = string.Format("造成{0}%攻击力的伤害", damageConf.damagePercent);
				else
					damageText = string.Format("每次造成{0}%攻击力的伤害", damageConf.damagePercent);

				if(string.IsNullOrEmpty(damageConf.desc))
					text += string.Format("　第{0}段，进行{1}次攻击，{2}\n", i+1, damageConf.times, damageText);
				else
					text += string.Format("　第{0}段，{1}, 进行{2}次攻击，{3}\n", i+1, damageConf.desc, damageConf.times, damageText);
			}
			attributeLabel.text = text;
		}
	}
}
