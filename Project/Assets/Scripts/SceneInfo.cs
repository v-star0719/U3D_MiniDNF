using UnityEngine;
using System.Collections;

//这是获取场景信息的接口

//要根据所对应的场景，使用不同的SceneManager，这个可以配表，但是感觉这样更方便
public enum SceneType
{
	None,
	City,
	Battle,
	MainMenu,
	Tutorial,
}

[ExecuteInEditMode]
public class SceneInfo : MonoBehaviour
{
	public static SceneInfo instance;
	public SceneType sceneType;
	public Vector2 mapSize;
	public SceneObjectMark[] sceneObjectMarkArray;
	public Transform[] enemyArray;
	public SceneLevelGate[] gateArray;
	public ObjectBase playerInitialPos;

	void Awake()
	{
		instance = this;
	}

	void OnDestroy()
	{
		instance = null;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(!Application.isPlaying)
		{
			sceneObjectMarkArray = gameObject.GetComponentsInChildren<SceneObjectMark>(true);
		}
	}
}
