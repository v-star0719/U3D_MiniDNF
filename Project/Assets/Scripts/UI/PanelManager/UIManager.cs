using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class UIManager : MonoBehaviour
{
	public static UIManager instance;
	public UIPanelBase[] panelArray;
	public UILoadingPanel loadingPanel;
	public UIPanelBase enterGamePanel;
	public UIPanelBase battleNamePanel;

	void Awake()
	{
		instance = this;
	}

	void OnDestroy(){
		instance = null;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		panelArray = GetComponentsInChildren<UIPanelBase>(true);
	
	}

	public static UIPanelBase GetPanelByName(EmPanelName panelName)
	{
		foreach(UIPanelBase panel in instance.panelArray)
		{
			if(panel.panelName == panelName)
				return panel;
		}
		Debug.LogError("PanelName = " + panelName + "is not found");
		return null;
	}

	
	public static void CloseAllPanel()
	{
		ClosePanel(instance.battleNamePanel);
		foreach(UIPanelBase panel in instance.panelArray)
			ClosePanel(panel);
	}

	public static void OpenPanel(EmPanelName panelName, params object[] args)
	{
		OpenPanel(GetPanelByName(panelName), args);
	}

	public static void OpenPanel(UIPanelBase panel, params object[] args)
	{
		panel.gameObject.SetActive(true);
		panel.OnOpen(args);
	}

	public static void ClosePanel(EmPanelName panelName)
	{
		ClosePanel(GetPanelByName(panelName));
	}

	public static void ClosePanel(UIPanelBase panel)
	{
		panel.gameObject.SetActive(false);
		panel.OnClose();
	}
}
