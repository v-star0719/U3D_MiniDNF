using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIBattleTipText : MonoBehaviour 
{
	public UIDamageNumberPanel controlPanel;
	public UILabel tipText;
	public float lifeTime = 1;

	// Use this for initialization
	void Start ()
 	{
	}

	// Update is called once per frame
	void Update ()
	{
	}

	public void Show(int n)
	{
		tipText.text = n.ToString();
		Invoke("Dead", lifeTime);
	}

	public void Dead()
	{
		controlPanel.OnTipDie(this);
	}
}
