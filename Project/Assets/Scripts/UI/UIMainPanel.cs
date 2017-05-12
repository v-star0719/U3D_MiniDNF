using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//技能栏对应的按键和技能
public class SkillSetting
{
	public char key;
	public int skillID;

	public SkillSetting(char key, int skillID)
	{
		this.key = key;
		this.skillID = skillID;
	}
}

public class UIMainPanel : UIPanelBase 
{
	public static UIMainPanel instance;

	public UISKillCtrl[] skillCtrlArray;
	public GameObject tipContent;
	public UISkillTipCtrl skillTipCtrl;
	public UISlider hpSlider;
	public UITabCtrl skillTab;

	private int curSkillTabIndex = 0;

	private SkillSetting[] skillSettingArray = new SkillSetting[]
	{
		//第一排
		new SkillSetting('q', (int)EmSkillIDSet.GrabBlast),
		new SkillSetting('w', (int)EmSkillIDSet.VaneSlash),
		new SkillSetting('e', (int)EmSkillIDSet.BlastBlood),
		new SkillSetting('r', (int)EmSkillIDSet.GoreCross_Berserker),
		new SkillSetting('t', (int)EmSkillIDSet.OutrageBreak),
		new SkillSetting('f', (int)EmSkillIDSet.Parry),
		//第二排
		new SkillSetting('z', (int)EmSkillIDSet.None),
		new SkillSetting('x', (int)EmSkillIDSet.None),
		new SkillSetting('c', (int)EmSkillIDSet.None),
		new SkillSetting('v', (int)EmSkillIDSet.None),
		new SkillSetting('B', (int)EmSkillIDSet.None),//三段斩是组合键
		new SkillSetting('g', (int)EmSkillIDSet.None),
	};
	private SkillSetting[] skillSettingArray2 = new SkillSetting[]
	{
		new SkillSetting(' ', (int)EmSkillIDSet.UpStrike),
		new SkillSetting(' ', (int)EmSkillIDSet.TripleSlash),
		new SkillSetting(' ', (int)EmSkillIDSet.JumpBack),
		new SkillSetting(' ', (int)EmSkillIDSet.Parry),
	};
	private bool isInited = false;

	// Use this for initialization
	void Start ()
 	{
		//Time.timeScale = 0;
	}

	// Update is called once per frame
	void Update ()
	{
	}

	public override void OnOpen(params object[] args)
	{
		instance = this;
		Init();
		ShowHp();
		curSkillTabIndex = -1;
		skillTab.SelectTab(0);
	}
	public override void OnClose()
	{
		instance = null;
	}

	void Init()
	{
		if(isInited) return;
		isInited = true;

		if(skillSettingArray.Length != skillCtrlArray.Length)
		{
			Debug.LogError("skillSettingArray.Length != skillCtrlArray.Length");
			return;
		}

		
	}

	private void ShowSkillIcon()
	{
		if(curSkillTabIndex == 0)
		{
			//技能I
			for(int i=0; i<skillCtrlArray.Length; i++)
			{
				skillCtrlArray[i].SetData(skillSettingArray[i].skillID, skillSettingArray[i].key);
			}
		}
		else
		{
			//技能II
			for(int i=0; i<skillCtrlArray.Length; i++)
			{
				if(i <skillSettingArray2.Length)
					skillCtrlArray[i].SetData(skillSettingArray2[i].skillID, skillSettingArray2[i].key);
				else
					skillCtrlArray[i].SetData(0, ' ');
			}
			UITutorialPanel.isSKill2BtnClicked = true;
		}
	}

	public void ShowSkillTip(bool show, UISKillCtrl ctrl)
	{
		skillTipCtrl.gameObject.SetActive(show);
		if(show)
		{
			skillTipCtrl.Show(ctrl.skillState.skillInfoConf);
			UITutorialPanel.isSKillTipsShowd = true;
		}
	}

	public void ShowHp()
	{
		hpSlider.value = GameMain.instance.mainPlayer.percentHp;
	}


	public void OnTipButtonClicked()
	{
		tipContent.SetActive(!tipContent.activeSelf);
		//Time.timeScale = tipContent.activeSelf ? 0f : 1f;
	}

	public void OnNextLevelBtnClicked()
	{

	}

	public void OnLoadSecreaLevelBtnClicked()
	{

	}
	public void OnLoadDevelopLevelBtnClicked()
	{

	}
	public void OnLoadLevel1BtnClicked()
	{

	}

	public void OnPauseBtnClicked()
	{
		UIManager.OpenPanel(EmPanelName.BattlePausePanel);
	}

	public void OnTabSelected(int select)
	{
		if(curSkillTabIndex == select) return;

		curSkillTabIndex = select;
		ShowSkillIcon();
	}
}
