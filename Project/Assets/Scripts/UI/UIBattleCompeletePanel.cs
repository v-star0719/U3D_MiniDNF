using UnityEngine;
using System.Collections;

public class UIBattleCompeletePanel : UIPanelBase {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public override void OnOpen(params object[] args)
	{
	}
	public override void OnClose()
	{
	}

	public void OnOkBtnClicked()
	{
		GameMain.EnterMainMenuScene();
	}
}
