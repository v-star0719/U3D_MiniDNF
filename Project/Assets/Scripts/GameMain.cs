using UnityEngine;
using System.Collections;

public enum GameStatus
{
	None,
	Loading,
	City,
	Battle,
	Tutorial,
	MainMenu
}

public class GameMain : MonoBehaviour
{
	public static GameMain instance;
	public static GameStatus curStatus;

	public static MainCameraController mainCameraController;//MainCameraController.Awake中赋值
	public static SceneManagerBase curSceneManager;//场景载入后赋值
	public static bool isMainPlayerInited = false;

	public SceneManagerCity sceneMgrCity;
	public SceneManagerBattle sceneMgrBattle;
	public SceneManagerMainMenu sceneMgrMainMenu;
	public SceneManagerTutorial sceneMgrTutorial;
	public ActorBase mainPlayer;
	public Camera mainCamera;
	public AudioSource bgmAudioSource;
	public Camera uiCamera;

	void Awake()
	{
		instance = this;
	}

	void OnDestroy()
	{
		instance = null;
	}

	// Use this for initialization
	void Start ()
	{
		StartGame();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public static void StartGame()
	{
		SceneLoader.Load(SceneNameSet.mainMenuScene, EmSceneLoadingType.EnterGame);
	}

	public static void EnterBattle()
	{
		SceneManagerBattle.curLevel = 1;
		SceneLoader.Load(SceneNameSet.level_n.ToString() + SceneManagerBattle.curLevel, EmSceneLoadingType.Normal);
	}

	public static void ChangeBattleLevel()
	{
		SceneManagerBattle.curLevel++;
		SceneLoader.Load(SceneNameSet.level_n.ToString() + SceneManagerBattle.curLevel, EmSceneLoadingType.BattleChangeLevel);
	}

	public static void EnterMainMenuScene()
	{
		SceneLoader.Load(SceneNameSet.mainMenuScene, EmSceneLoadingType.Normal);
	}

	public static void EnterTutorialScene()
	{
		SceneLoader.Load(SceneNameSet.tutorial, EmSceneLoadingType.Normal);
	}


	public static void PlayBgm()
	{
		string bgm = "";
		if(curStatus == GameStatus.MainMenu)
			bgm = GameConfig.mainMenuSceneBgm;
		else if(curStatus == GameStatus.Battle)
				bgm = GameConfig.battleBgm;
		else if(curStatus == GameStatus.Tutorial)
				bgm = GameConfig.tutorialSceneBgm;

		instance.bgmAudioSource.clip = ResourceLoader.LoadBgmSound(bgm);
		instance.bgmAudioSource.Play();
	}
}
