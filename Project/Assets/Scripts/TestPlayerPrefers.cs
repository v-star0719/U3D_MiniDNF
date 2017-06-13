using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TestPlayerPrefers : MonoBehaviour 
{
	public bool resetHasPlayedTutorial;

	// Use this for initialization
	void Start ()
 	{
	}

	// Update is called once per frame
	void Update ()
	{
		if(resetHasPlayedTutorial)
		{
			PlayerPrefs.SetInt(PlayerPreferManager.hasPlayedTutorial, 0);
			resetHasPlayedTutorial = false;
		}
	}
}
