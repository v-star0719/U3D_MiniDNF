using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIDamageNumberPanel : UIPanelBase 
{
	public static UIDamageNumberPanel instance;

	public Camera uiCamera;
	public UIBattleTipText tipPrefab;

	public List<UIBattleTipText> activeTipList = new List<UIBattleTipText>();
	public List<UIBattleTipText> inactiveTipList = new List<UIBattleTipText>();

	// Use this for initialization
	void Start ()
 	{
		instance = this;
	}

	void OnDestroy()
	{
		instance = null;
	}

	// Update is called once per frame
	void Update ()
	{
	}

	//目前只显示伤害数字
	public static void ShowTipText(Vector3 scenePos, int n)
	{
		if(n == 0) return;

		instance.ShowTipText__(scenePos, n);
	}
	public void ShowTipText__(Vector3 scenePos, int n)
	{
		UIBattleTipText tip = null;
		if(inactiveTipList.Count == 0)
		{
			//create new
			tip = Instantiate<UIBattleTipText>(tipPrefab);
			tip.name = tipPrefab.name + activeTipList.Count.ToString();
			tip.transform.parent = transform;
			tip.transform.localScale = Vector3.one;
		}
		else
		{
			tip = inactiveTipList[0];
			inactiveTipList.RemoveAt(0);
		}

		activeTipList.Add(tip);
		tip.gameObject.SetActive(true);
		tip.Show(n);

		Vector3 screenPos = GameMain.instance.mainCamera.WorldToScreenPoint(scenePos);
		screenPos.z = 0;
		Vector3 uiPos = uiCamera.ScreenToWorldPoint(screenPos);
		tip.transform.position = uiPos;
	}

	public void OnTipDie(UIBattleTipText tip)
	{
		tip.gameObject.SetActive(false);
		inactiveTipList.Add(tip);
		activeTipList.Remove(tip);
	}
}
