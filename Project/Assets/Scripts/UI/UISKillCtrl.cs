using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UISKillCtrl : MonoBehaviour 
{
	public UIMainPanel mainPanel;
	public UISprite grayMask;
	public UISprite skillIcon;//图标数字+1就是灰色图标
	public UISprite pressKeySprite;

	public SkillState skillState;

	// Use this for initialization
	void Start ()
 	{
	}

	// Update is called once per frame
	void Update ()
	{
		if(skillState != null)
			SetSkillState();
	}

	void SetSkillState()
	{
		if(skillState.isCding)
		{
			grayMask.gameObject.SetActive(true);
			grayMask.fillAmount = skillState.cdingPercent;
			skillIcon.spriteName = skillState.skillInfoConf.icon.ToString();
		}
		else
		{
			skillIcon.spriteName = skillState.skillInfoConf.icon.ToString();
			grayMask.gameObject.SetActive(false);
		}
	}

	public void SetData(int skillID, char skillKey)
	{
		if(skillID <= 0)
		{
			pressKeySprite.spriteName = string.Empty;
			skillIcon.gameObject.SetActive(false);
			skillState = null;
			return;
		}

		skillState = GameMain.instance.mainPlayer.skillStateManager.GetSkillState(skillID);

		pressKeySprite.spriteName = skillKey.ToString();
		pressKeySprite.MakePixelPerfect();
		skillIcon.spriteName = skillState.skillInfoConf.icon.ToString();
		skillIcon.gameObject.SetActive(true);
		SetSkillState();
	}

	public void OnHover(bool isOver)
	{
		if(skillState != null)
			mainPanel.ShowSkillTip(isOver, this);
		else
			mainPanel.ShowSkillTip(false, null);
	}
}
