//标签页控件
//可以在标签上指定对应的页面
//每次切换标签都会向_HandleObj发送消息调用_HandleFunName


using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//每个标签都包含UITabCellCtrl.cs组件，标签的切换操作由UITabCtrl完成，只管接收消息判断是哪个标签被选择即可
[ExecuteInEditMode]
public class UITabCtrl : MonoBehaviour 
{
    public GameObject _HandleObj;
    public string _HandleFunName = "OnTabSelected";

    public string _CheckFunName = "CheckTabCanSelected";
    public bool bConditionTab = false;

    public List<UITabCellCtrl> cellList = new List<UITabCellCtrl>();//新版本的标签列表

	public int curSelect = 0; //当前选择的标签，默认第一个

    void Awake()
    {
		//初始化，默认选择第一个
		if(cellList.Count > 0)
		{
			cellList[0].Select(true);
			cellList[0].index = 0;//给标签打上索引
			curSelect = 0;
			for(int j=1; j<cellList.Count; j++)
			{
				cellList[j].Select(false);
				cellList[j].index = j;//给标签打上索引
			}
		}
    }

	//新版本，当一个标签被点击了会收到这个消息
	public void OnCellClicked(object obj)
    {
        int index = (int)obj;
        if (!bConditionTab)
            SelectTab(index);
        else
            CheckTabCanCheck(index);
    }  
    
    /// <summary>
    /// 直接发送给调用者check方法,由调用者判断是否可以选中,并且调用selectTab
    /// </summary>
    /// <param name="index"></param>
    void CheckTabCanCheck(int index)
    {
        if (_HandleObj != null)
            _HandleObj.SendMessage(_CheckFunName, index, SendMessageOptions.DontRequireReceiver);//选择的标签索引放在参数里
    }

    //新版本，选择一个标签。可以通过该接口选择一个标签
    public void SelectTab(int index)
	{
		if(index < 0 || index >= cellList.Count)
			return;

		//是否是选择的同一个标签交给使用方去判断吧，不然的话这里使用默认值，然后使用方主动进行的SelectTab就会不执行
		//if(curSelect != index)
		{
			cellList[curSelect].Select(false);
			cellList[index].Select(true);
			curSelect = index;
			if(_HandleObj != null)
				_HandleObj.SendMessage(_HandleFunName, curSelect, SendMessageOptions.DontRequireReceiver);//选择的标签索引放在参数里

		}
	}

    public void ReSelect()
    {
        SelectTab(curSelect);
    }

	public EmTabIndices GetCellStringIndex(int index)
	{
		if(index < 0 || index >= cellList.Count)
			return EmTabIndices.None;

		return cellList[index].cellIndex;
	}

	#if UNITY_EDITOR
	public bool repositionBtn;
	void Update()
	{
		if(repositionBtn)
		{
			repositionBtn = false;
		}
	}
	#endif
}
