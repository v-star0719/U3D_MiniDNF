using UnityEngine;
using System.Collections;

public class UICityPanel : UIPanelBase {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnChallengeBtnClicked()
	{
		GameMain.EnterBattle();
	}
}
