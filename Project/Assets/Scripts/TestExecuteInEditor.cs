using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class TestExecuteInEditor : MonoBehaviour 
{
	void Awake()
	{
		Debug.Log("TestExecuteInEditor" + "Awake");
	}

	void OnEnable()
	{
		Debug.Log(Application.dataPath);
		Debug.Log(Application.persistentDataPath);
		Debug.Log(System.IO.Directory.GetCurrentDirectory());
		Debug.Log("TestExecuteInEditor" + "OnEnable");

	}

	void OnDisable()
	{
		Debug.Log("TestExecuteInEditor" + "OnDisable");
	}

	void OnDestroy()
	{
		Debug.Log("TestExecuteInEditor" + "OnDestory");
	}

	// Use this for initialization
	void Start ()
 	{
		Debug.Log("TestExecuteInEditor" + "Start");
	}

	// Update is called once per frame
	void Update ()
	{ 
		Debug.Log("TestExecuteInEditor" + "Update");
	}

}
