using UnityEngine;
using System.Collections;

public class UIBattlePausePanel : UIPanelBase {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public override void OnOpen(params object[] args)
	{
		Time.timeScale = 0;
	}
	public override void OnClose()
	{
		Time.timeScale = 1;
	}

	public void OnResumeBtnClicked()
	{
		CloseMySelf();
	}

	public void OnExistBtnClicked()
	{
		GameMain.EnterMainMenuScene();
	}
}
