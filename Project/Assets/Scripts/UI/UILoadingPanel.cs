using UnityEngine;
using System.Collections;

public class UILoadingPanel : UIPanelBase
{
	public const int MODE_DONT_SHOW = 0;
	public const int MODE_TEXTURE = 1;
	public const int MODE_DARK = 2;

	public static UILoadingPanel instance;

	public GameObject textureGroup;
	public GameObject darkGroup;

	public TweenAlpha darkTween;

	private int mode;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public override void OnOpen(params object[] args)
	{
		mode = 1;
		if(args.Length > 0)
			mode = (int)args[0];

		textureGroup.SetActive(mode == MODE_TEXTURE);
		darkGroup.SetActive(mode == MODE_DARK);

		if(mode == MODE_DARK)
		{
			darkTween.ResetToBeginning();
			darkTween.PlayForward();
		}
    }
    
    public override void OnClose()
	{
	}

	public static void Open(int mode)
	{
		UIManager.OpenPanel(UIManager.instance.loadingPanel, mode);
	}

	//动画播放完了再关闭
	public static void Close()
	{
		UIManager.ClosePanel(UIManager.instance.loadingPanel);
	}
}
