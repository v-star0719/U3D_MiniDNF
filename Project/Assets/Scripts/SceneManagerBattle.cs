using UnityEngine;
using System.Collections;

public class SceneManagerBattle : SceneManagerBase
{
	//以后把战斗逻辑单独分离到BattlManager里
	public static int curLevel;

	private static bool isStageClear;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public override void Init()
	{
		UIManager.OpenPanel(UIManager.instance.battleNamePanel);
		base.Init();
		isStageClear = false;
	}

	public override void StartScene()
	{
		UIManager.OpenPanel(EmPanelName.MainPanel);
		UIManager.OpenPanel(EmPanelName.BattleDamageNumberPanel);
		UIManager.OpenPanel(EmPanelName.BattleHpPanel);
		UIManager.OpenPanel(EmPanelName.BattlePanel);
	}

	public override void ActorDead(ActorBase actor)
	{
		UIBattleActorNamePanel.instance.Hide(actor);

		if(actor.actorType == EmActorType.MainPlayer)
		{
			UIManager.OpenPanel(EmPanelName.BattleFailedPanel);
		}
		else
		{
			deadEnemyCount++;
			if(deadEnemyCount == enemiesList.Count)
			{
				StageClear();
			}
		}
	}

	public void StageClear()
	{
		//UIManager.instance.battlePanel.ShowEndPanel();
		isStageClear = true;
		foreach(SceneLevelGate g in sceneInfo.gateArray)
			g.OpenOrClose(true);

		if(curLevel == GameConfig.maxBattleLevel)
			UIManager.OpenPanel(EmPanelName.BattleCompeletePanel);
	}

	public void OnPlayerEnterDoor()
	{
		if(isStageClear && GameMain.curStatus != GameStatus.Loading)
		{
			GameMain.ChangeBattleLevel();
		}
	}
}
