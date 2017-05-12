using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//以后再写缓存池吧
public class UIBattleActorNamePanel : UIPanelBase 
{
	public UIBattleActorNameCtrl nameCtrlPrefab;
	public static UIBattleActorNamePanel instance;

	public List<UIBattleActorNameCtrl> nameCtrlList;

	public override void OnOpen(params object[] args)
	{
		instance = this;
	}
	public override void OnClose()
	{
		instance = null;
		foreach(UIBattleActorNameCtrl item in nameCtrlList)
		{
			GameObject.Destroy(item.gameObject);
		}
		nameCtrlList.Clear();
	}

	public void Show(ActorBase actor, EmMonsterType monsterType)
	{
		if(monsterType == EmMonsterType.Normal) return;

		UIBattleActorNameCtrl item = Instantiate<UIBattleActorNameCtrl>(nameCtrlPrefab);
		item.transform.parent = transform;
		item.transform.localScale = Vector3.one;
		item.gameObject.SetActive(true);

		item.Show(actor, monsterType);
		nameCtrlList.Add(item);
	}

	public void Hide(ActorBase actor)
	{
		foreach(UIBattleActorNameCtrl item in nameCtrlList)
		{
			if(item.actor == actor)
			{
				nameCtrlList.Remove(item);
				GameObject.Destroy(item.gameObject);
				break;
			}
		}
	}
}
