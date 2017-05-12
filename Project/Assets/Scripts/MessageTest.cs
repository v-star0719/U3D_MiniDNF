using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class MessageTest : MonoBehaviour 
{
	void Awake()
	{
		Debug.Log("MessageTest.Awake");
	}

	// Use this for initialization
	void Start ()
 	{
		Debug.Log("MessageTest.Start");
	}

	void OnEnable()
	{
		Debug.Log("MessageTest.OnEnable");
	}

	void OnDisable()
	{
		Debug.Log("MessageTest.OnDisable");
	}

	void OnDestroy()
	{
		Debug.Log("MessageTest.OnDestroy");
	}


	// Update is called once per frame
	void Update ()
	{
		Debug.Log("MessageTest.Update");
	}
}
