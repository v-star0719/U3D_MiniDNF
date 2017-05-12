using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//LevelManager用于完成相关的功能

public class SceneManagerBase : MonoBehaviour
{
	public SceneInfo sceneInfo;

	public ActorBase mainPlayer;
	public Camera sceneCamera;

	public int deadEnemyCount;
	public List<ActorBase> enemiesList;
	public List<ActorBase> teammatesList;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public virtual void Init()
	{
		deadEnemyCount = 0;
		GameMain.instance.mainPlayer.objectbase.realLocalPos = sceneInfo.playerInitialPos.realLocalPos;

		enemiesList.Clear();
		teammatesList.Clear();

		//创建引用的场景元素(包括了怪物）
		SceneObjectMark[] array = sceneInfo.sceneObjectMarkArray;
		foreach(SceneObjectMark item in array)
		{
			item.CreateInstance();
		}

		//怪物初始化
		foreach(Transform item in sceneInfo.enemyArray)
		{
			if(!item.gameObject.activeSelf) continue;
			EnemyAI ai = item.gameObject.AddComponent<EnemyAI>();
			ActorBase actor = item.gameObject.AddComponent<ActorBase>();
			EnemyMark mark = item.gameObject.GetComponent<EnemyMark>();
			actor.Init(mark.enemyId, mark.enemyLv, true, EmActorFaction.Enemy, EmActorType.Monster);
			actor.objectbase.isFacingRight = actor.objectbase.isFacingRight;//将朝向应用到动画上
			actor.monsterType = mark.monsterType;
			ai.StartAI(actor);
			enemiesList.Add(actor);

			if(UIBattleActorNamePanel.instance != null)
				UIBattleActorNamePanel.instance.Show(actor, mark.monsterType);
		}

		teammatesList.Add(GameMain.instance.mainPlayer);

		foreach(SceneLevelGate g in sceneInfo.gateArray)
			g.OpenOrClose(false);

		if(GameMain.curStatus != GameStatus.Battle)
			InitPlayer();
	}

	private void InitPlayer()
	{
		//主角
		if(!GameMain.isMainPlayerInited)
		{
			GameMain.instance.mainPlayer.Init(1001, 1, true, EmActorFaction.Teammate, EmActorType.MainPlayer);
			GameMain.isMainPlayerInited = true;
		}
		else
		{
			GameMain.instance.mainPlayer.Reset();
		}
		GameMain.instance.mainCamera.orthographicSize = 300;
	}

	public virtual void StartScene()
	{
	}

	public virtual void Release()
	{
	}

	///获取角色的敌人，根据阵营获取
	public List<ActorBase> GetEnemyList(ActorBase actor)
	{
		if(actor.actorFaction == EmActorFaction.Teammate)
			return enemiesList;
		else if(actor.actorFaction == EmActorFaction.Enemy)
			return teammatesList;
		else
		{
			Debug.Log("Actor faction = " + actor.actorFaction + " has no enemy");
			return null;
		}
	}

	public virtual void ActorDead(ActorBase actor)
	{
	}
}
