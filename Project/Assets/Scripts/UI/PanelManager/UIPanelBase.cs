using UnityEngine;
using System.Collections;

/*
	public override void OnOpen(params object[] args)
	{
	}
	public override void OnClose()
	{
	}
	*/

public class UIPanelBase : MonoBehaviour
{
	public EmPanelName panelName;

	/*
	public string panelName
	{
		get{return _panelName;}
		set{_panelName = value;
			gameObject.name = _panelName;}
	}
	*/

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public virtual void OnOpen(params object[] args){}
	public virtual void OnClose(){}

	public void CloseMySelf()
	{
		UIManager.ClosePanel(this);
	}
}
