using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIHpCtrl : MonoBehaviour 
{
	public UISprite hpSpriteNormal;
	public UISprite hpSpriteDark;
	public float autoHideTime = 2f;//血量为0时倒计时自动隐藏
	public float darkShowTime = 1f;
	public UILabel nameLvLabel;

	public UIIconCtrl iconCtrl;
	public GameObject deadIconMark;

	//hp削减动画
	private float maxHp;
	private float hpFrom;
	private float hpTo;
	private float hpAnimaTimer;
	private bool isPlaying = false;

	//自动隐藏
	private float autoHideTimer = 0;
	private bool autoHide = false;

	// Use this for initialization
	void Start ()
 	{

	}

	// Update is called once per frame
	void Update ()
	{
		Update_HPAnima();
		Update_AutoHide();
	}

	void Update_HPAnima()
	{
		if(!isPlaying) return;

		hpAnimaTimer += Time.deltaTime;

		float f = hpAnimaTimer < darkShowTime ? hpAnimaTimer/darkShowTime : 1f;
		float curHp = hpFrom + (hpTo - hpFrom)*f;
		hpSpriteDark.fillAmount = curHp/maxHp;

		if(hpAnimaTimer >= darkShowTime)
		{
			isPlaying = false;
			autoHideTimer = 0;//血量动画完了，准备自动隐藏
		}
	}

	void Update_AutoHide()
	{
		if(!autoHide) return;
		autoHideTimer += Time.deltaTime;
		if(autoHideTimer >= autoHideTime)
		{
			gameObject.SetActive(false);
			autoHide = false;
		}
	}

	public void Show(DBActorAttributeConf actorConf, float maxHp, float curHp, float preHp, EmMonsterType monsterType)
	{
		string prefix = "";
		if(monsterType == EmMonsterType.Elite)
			prefix = "精英 · ";
		else if(monsterType == EmMonsterType.Boss)
			prefix = "领主 · ";

		gameObject.SetActive(true);
		nameLvLabel.text = prefix + actorConf.charactorName + " Lv.1";
		iconCtrl.Set(actorConf.portraitID);
		deadIconMark.SetActive(curHp == 0);

		hpSpriteNormal.fillAmount = curHp/maxHp;
		this.maxHp = maxHp;
		hpFrom = preHp;
		hpTo = curHp;
		hpSpriteDark.fillAmount = hpFrom/maxHp;
		hpAnimaTimer = 0;
		isPlaying = true;
		autoHide = curHp == 0f;
	}
}
