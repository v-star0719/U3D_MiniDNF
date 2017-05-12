using UnityEngine;
using System.Collections;

public enum EmSceneLoadingType
{
	Normal,//普通切场景
	EnterGame,//第一次进入场景
	BattleChangeLevel,//战斗切换关卡
}

public class SceneLoader : MonoBehaviour
{
	private static SceneLoader instance;

	private int step = 0;//step = 1时开始加载
	private string sceneToLoad;
	private GameStatus newGameStatus;
	private bool isLoading
	{
		get{return step != 0;}
	}

	private EmSceneLoadingType loadingType;

	void Awake()
	{
		instance = this;
	}

	// Use this for initialization
	void Start ()
	{
	
	}

	void OnDestroy()
	{
		instance = null;
	}

	// Update is called once per frame
	void Update ()
	{
		if(step == 0) return;

		//各个步骤该干嘛完全自定义
		switch(step)
		{
		case 1:
			ReleaseOldScene();
			step++;
			break;
		case 2:
			LoadNewScene();
			step++;
			break;
		case 3:
			GameMain.curSceneManager.Init();
			step++;
			break;
		case 4:
			GameMain.curSceneManager.StartScene();
			step++;
			break;
		case 5:
			step++;
			GameMain.curStatus = newGameStatus;
			Invoke("LoadingFinished", 2);//纯粹是未了等loading面板
			break;
		default:
			step = 0;
			break;
		}
	}

	private void LoadingFinished()
	{
		if(loadingType != EmSceneLoadingType.BattleChangeLevel)
			GameMain.PlayBgm();
		UILoadingPanel.Close();
		UIManager.ClosePanel(UIManager.instance.enterGamePanel);
	}

	private void ReleaseOldScene()
	{
		UIManager.CloseAllPanel();
		
		SceneManagerBase sm = GameMain.curSceneManager;
		if(sm == null) return;

		if(sm.sceneInfo != null)
			GameObject.Destroy(sm.sceneInfo.gameObject);

		if(loadingType != EmSceneLoadingType.BattleChangeLevel)
			GameMain.instance.bgmAudioSource.Stop();//非战斗内加载地图的时候，把声音关掉
		return;
	}

	private void LoadNewScene()
	{
		GameObject scene = ResourceLoader.LoadScene(sceneToLoad);
		if(scene == null) return;

		SceneInfo info = scene.GetComponent<SceneInfo>();
		if(info == null){
			Debug.LogError("SceneInfo component is not found");
			return;
		}

		switch(info.sceneType)
		{
		case SceneType.City:
			GameMain.curSceneManager = GameMain.instance.sceneMgrCity;
			newGameStatus = GameStatus.City;
			break;
		case SceneType.Battle:
			GameMain.curSceneManager = GameMain.instance.sceneMgrBattle;
			newGameStatus = GameStatus.Battle;
			break;
		case SceneType.Tutorial:
			GameMain.curSceneManager = GameMain.instance.sceneMgrTutorial;
			newGameStatus = GameStatus.Tutorial;
			break;
		case SceneType.MainMenu:
			GameMain.curSceneManager = GameMain.instance.sceneMgrMainMenu;
			newGameStatus = GameStatus.MainMenu;
			break;
		case SceneType.None:
			Debug.LogError("SceneType is not None");
			break;
		}
		GameMain.curSceneManager.sceneInfo = info;
	}


	//这个接口是切换场景的命令，但切换的时候还会有别的工作要做，所以不要直接调用
	public static void Load(string sceneName, EmSceneLoadingType lt)
	{
		if(instance.isLoading) return;

		instance.loadingType = lt;

		//启动游戏的载入界面是另外一个
		if(lt == EmSceneLoadingType.EnterGame)
			UIManager.OpenPanel(UIManager.instance.enterGamePanel);
		else if(lt == EmSceneLoadingType.BattleChangeLevel)
			UILoadingPanel.Open(UILoadingPanel.MODE_DARK);
		else
			UILoadingPanel.Open(UILoadingPanel.MODE_TEXTURE);
		
		GameMain.curStatus = GameStatus.Loading;
		instance.sceneToLoad = sceneName;
		instance.step = 1;

	}
}
