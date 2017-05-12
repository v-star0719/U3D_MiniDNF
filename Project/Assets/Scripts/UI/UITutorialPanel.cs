using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum EmTutorialStep
{
	None,
	Move,
	Run,
	S,
	D,
	A,
	SKills,
	ComboSKills1,
	ComboSKills2,
	End,
}

[System.Serializable]
public class TutorialData
{
	public EmTutorialStep step;
	public GameObject content;
	public string tipText;
	public float duration;//走、跑了多少时间，按下按键后延迟多长时间
}

public class UITutorialPanel : UIPanelBase 
{
	public TutorialData[] stepArray;
	public GameObject tipTextGroup;
	public UILabel tipTextLabel;

	private int curStep = -1;
	//private Vector3 orgPlayerPos;
	private float timer;
	private bool isKeyPressed = false;

	public static bool isSKill2BtnClicked = false;
	public static bool isSKillTipsShowd = false;

	public override void OnOpen(params object[] args)
	{
		for(int i=0; i<stepArray.Length; i++ ) stepArray[i].content.SetActive(false);
		curStep = -1;
		NextStep();
	}
	public override void OnClose()
	{
	}

	// Use this for initialization
	void Start ()
 	{
	}

	// Update is called once per frame
	void Update ()
	{
		if(curStep < 0 || curStep >= stepArray.Length) return;

		TutorialData data = stepArray[curStep];

		switch(data.step)
		{
		case EmTutorialStep.Move:
			if(GameMain.instance.mainPlayer.isWalking)
				timer += Time.deltaTime;
			if(timer >= data.duration)
				NextStep();
			break;
		case EmTutorialStep.Run:
			if(GameMain.instance.mainPlayer.isRunning)
				timer += Time.deltaTime;
			if(timer >= data.duration)
				NextStep();
			break;
		case EmTutorialStep.S:
			if(Input.GetKeyDown(KeyCode.S))
				isKeyPressed = true;
			if(isKeyPressed)
				timer += Time.deltaTime;
			if(timer >= data.duration)
				NextStep();
			break;
		case EmTutorialStep.A:
			if(Input.GetKeyDown(KeyCode.A))
				isKeyPressed = true;
			if(isKeyPressed)
				timer += Time.deltaTime;
			if(timer >= data.duration)
				NextStep();
			break;
		case EmTutorialStep.D:
			if(Input.GetKeyDown(KeyCode.D))
				isKeyPressed = true;
			if(isKeyPressed)
				timer += Time.deltaTime;
			if(timer >= data.duration)
				NextStep();
			break;
		case EmTutorialStep.SKills:
			timer += Time.deltaTime;
			if(timer >= data.duration)
				NextStep();
			break;
		case EmTutorialStep.ComboSKills1:
			if(isSKill2BtnClicked)
				timer += Time.deltaTime;
			if(timer >= data.duration)
				NextStep();
			break;
		case EmTutorialStep.ComboSKills2:
			if(isSKillTipsShowd)
				timer += Time.deltaTime;
			if(timer >= data.duration)
				NextStep();
			break;
		case EmTutorialStep.End:
			timer += Time.deltaTime;
			if(timer >= data.duration)
				NextStep();
			break;
		}
	}

	void NextStep()
	{
		isKeyPressed = false;
		//orgPlayerPos = GameMain.instance.mainPlayer.objectbase.realLocalPos;
		isSKill2BtnClicked = false;
		isSKillTipsShowd = false;
		timer = 0f;

		//前一个的动画暂停
		if(curStep >= 0)
		{
			TutorialData data = stepArray[curStep];
			TweenScale ts = data.content.GetComponent<TweenScale>();
			if(ts != null)
			{
				ts.enabled = false;
				ts.transform.localScale = Vector3.one;
			}
			tipTextGroup.SetActive(false);

			if(data.step == EmTutorialStep.ComboSKills1 || data.step == EmTutorialStep.ComboSKills2 || data.step == EmTutorialStep.End)
				data.content.SetActive(false);
		}


		curStep++;

		//新的出现
		if(curStep < stepArray.Length)
		{
			TutorialData data = stepArray[curStep];
			TweenAlpha ta = data.content.GetComponent<TweenAlpha>();
			if(ta != null)
			{
				ta.ResetToBeginning();
				ta.PlayForward();
			}
			data.content.SetActive(true);
			tipTextGroup.SetActive(true);
			tipTextLabel.text = data.tipText;
		}
	}

	public void OnOpenTutorialBtnClicked()
	{

	}

	public void OnExistBtnClicked()
	{
		GameMain.EnterMainMenuScene();
	}
}
