using UnityEngine;
using System.Collections;

public class UIBattleHpPanel : UIPanelBase
{
	public UIHpCtrl smallHpCtrl;
	public UIHpCtrl bigHpCtrl;

	public static UIBattleHpPanel instance;

	public override void OnOpen(params object[] args)
	{
		instance = this;
		smallHpCtrl.gameObject.SetActive(false);
		bigHpCtrl.gameObject.SetActive(false);
	}
	public override void OnClose()
	{
		instance = null;
	}

	public void ShowMonsterHP(DBActorAttributeConf actorConf, float maxHp, float curHp, float preHp, EmMonsterType monsterType)
	{
		UIHpCtrl hpCtrl = null;
		if(monsterType == EmMonsterType.Boss)
		{
			hpCtrl = bigHpCtrl;
			smallHpCtrl.gameObject.SetActive(false);
		}
		else
		{
			hpCtrl = smallHpCtrl;
			bigHpCtrl.gameObject.SetActive(false);
		}
		hpCtrl.gameObject.SetActive(true);
		hpCtrl.Show(actorConf, maxHp, curHp, preHp, monsterType);
	}
}
