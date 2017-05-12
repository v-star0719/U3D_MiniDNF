using UnityEngine;
using System.Collections;

public class UIMainMenuPanel : UIPanelBase
{
	public GameObject mainMenu;
	public GameObject playMenu;
	public GameObject settingMenu;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public override void OnOpen(params object[] args)
	{
		Init();
	}

	private void Init()
	{
		mainMenu.SetActive(true);
		playMenu.SetActive(false);
		settingMenu.SetActive(false);
	}
	
	public override void OnClose()
	{
	}

	public void OnPlayItemClicked()
	{
		mainMenu.gameObject.SetActive(false);
		playMenu.SetActive(true);
	}

	public void OnSettingItemClicked()
	{
		mainMenu.gameObject.SetActive(false);
		settingMenu.SetActive(true);
	}

	public void OnTutorialItemClicked()
	{
		GameMain.EnterTutorialScene();
	}

	public void OnBackItmeClicked()
	{
		Init();
	}

	public void OnPlayEasyItemClicked()
	{
		GameMain.EnterBattle();
	}

	public void OnPlayNormalItemClicked()
	{
	}

	public void OnPlayHardItemClicked()
	{
	}
}
