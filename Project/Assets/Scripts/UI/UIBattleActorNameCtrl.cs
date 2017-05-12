using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIBattleActorNameCtrl : MonoBehaviour 
{
	public const float YOffset = 10;//pixel

	public UILabel nameLabel;

	public Color normalNameColor;
	public Color bossNameColor;
	public Color eliteNameColor;

	[HideInInspector]
	public ActorBase actor;

	// Use this for initialization
	void Start ()
 	{
	}

	// Update is called once per frame
	void Update ()
	{
		UpdatePos();
	}

	void UpdatePos()
	{
		if(actor == null) return;

		Vector3 pos = actor.transform.position;
		pos.y += actor.actorConf.size.y;
		Vector3 screenPos = GameMain.instance.mainCamera.WorldToScreenPoint(pos);
		screenPos.z = 0;
		pos = GameMain.instance.uiCamera.ScreenToWorldPoint(screenPos);
		transform.position = pos;
	}

	public void Show(ActorBase actor, EmMonsterType monsterType)
	{
		this.actor = actor;
		Color clr = Color.white;
		string prefix = "";
		if(monsterType == EmMonsterType.Normal)
			clr = normalNameColor;
		else if(monsterType == EmMonsterType.Elite)
		{
			clr = eliteNameColor;
			prefix = "精英 · ";
		}
		else if(monsterType == EmMonsterType.Boss)
		{
			prefix = "领主 · ";
			clr = bossNameColor;
		}
		nameLabel.color = clr;
		nameLabel.text = prefix + actor.actorConf.charactorName;
	}

	public void Hide()
	{
		actor = null;
	}
}
