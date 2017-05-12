using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum EmTabIndices
{
    None = 0
}

public class UITabCellCtrl : MonoBehaviour 
{
	public EmTabIndices cellIndex;
	public GameObject imgNormal;   //普通状态的图片（即未选中状态）
	public GameObject imgSelected; //选中状态的图片
	public GameObject attachedPage;//对应的页面

	[HideInInspector]public int index = 0; //这个标签在标签栏里的索引

	//被点击了
	public void OnClick()
	{
		transform.parent.SendMessage("OnCellClicked", index);
	}

	//设置选择/不选择状态
	public void Select(bool bBeSelected)
	{
		if(imgNormal!=null)
			imgNormal.SetActive(!bBeSelected);
		if(imgSelected!=null)
			imgSelected.SetActive(bBeSelected);

		if(attachedPage != null)
			attachedPage.SetActive(bBeSelected);
	}
}
