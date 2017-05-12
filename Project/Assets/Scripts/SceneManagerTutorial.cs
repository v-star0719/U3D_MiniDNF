using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SceneManagerTutorial : SceneManagerBase
{
	// Use this for initialization
	void Start()
	{
	}

	// Update is called once per frame
	void Update()
	{
	}

	public override void Init()
	{
		base.Init();
	}

	public override void StartScene()
	{
		UIManager.OpenPanel(EmPanelName.MainPanel);
		UIManager.OpenPanel(EmPanelName.TutorialPanel);
		UIManager.OpenPanel(EmPanelName.BattleDamageNumberPanel);
		UIManager.OpenPanel(EmPanelName.BattleHpPanel);
	}
}
