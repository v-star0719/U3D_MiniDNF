using UnityEngine;
using System.Collections;

public class UIBattlePanel : UIPanelBase {

	public UILabel curLevel;
	public GameObject endPanel;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public override void OnOpen(params object[] args)
	{
		Set();
		endPanel.SetActive(false);
	}
	
	public override void OnClose()
	{
	}

	public void Set()
	{
		curLevel.text = "第" + SceneManagerBattle.curLevel.ToString() + "关";
		endPanel.SetActive(false);
	}

	public void OnNextBtnClicked()
	{

	}

	public void ShowEndPanel()
	{
		endPanel.SetActive(true);
	}

	public void OnBackBtnClicked()
	{

	}
}
