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
			Invoke("ShowBattleFailedPanel", 1.5f);
		}
		else
		{
			deadEnemyCount++;
			if(deadEnemyCount == enemiesList.Count)
			{
				StageClear();
			}
		}

		//boss死亡播放慢镜头，其他小怪自动死亡
		if(actor.monsterType == EmMonsterType.Boss)
		{
			BossDeadSlowMotion slowMotion = GameMain.instance.gameObject.AddComponent<BossDeadSlowMotion>();
			slowMotion.Play(OnBossDeadSlowMotionPlayEnd);
		}
	}

	public void StageClear()
	{
		//UIManager.instance.battlePanel.ShowEndPanel();
		isStageClear = true;
		foreach(SceneLevelGate g in sceneInfo.gateArray)
			g.OpenOrClose(true);

		if(curLevel == GameConfig.maxBattleLevel)
			Invoke("ShowBattleCompeletePanel", 1);
	}

	//用于Invoke
	void ShowBattleCompeletePanel()
	{
		UIManager.OpenPanel(EmPanelName.BattleCompeletePanel);
	}
	//用于Invoke
	void ShowBattleFailedPanel()
	{
		UIManager.OpenPanel(EmPanelName.BattleFailedPanel);
	}


	public void OnPlayerEnterDoor()
	{
		if(isStageClear && GameMain.curStatus != GameStatus.Loading)
		{
			GameMain.ChangeBattleLevel();
		}
	}

	public void OnBossDeadSlowMotionPlayEnd()
	{
		BossDeadSlowMotion slowMotion = GameMain.instance.gameObject.AddComponent<BossDeadSlowMotion>();
		Destroy(slowMotion);

		//其他小怪自动死亡
		for(int i=0; i<enemiesList.Count; i++)
		{
			if(enemiesList[i].isAlive)
				enemiesList[i].Die();
		}
	}
}
