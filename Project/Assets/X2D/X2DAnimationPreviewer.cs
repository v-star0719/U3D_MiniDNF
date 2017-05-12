using UnityEngine;
using System.Collections;

public class X2DAnimationPreviewer : MonoBehaviour
{
	//记录当前播放状态
	public X2DAnimationClip playingClip;
	public EmX2DAnimationPlayMode playMode = EmX2DAnimationPlayMode.Loop;
	public int curFrame;
	public float timer;
	public bool isForward;
	public ActorBase actorContainer;
	public Transform normalContainer;

	private X2DAnimation anima;
	private X2DAnimationClip previewClip;
	private bool isPreviewingAll;
	private bool isPreviewingOne;
	private int curClipIndex = 0;

	void Update()
	{
		if(isPreviewingOne)
		{
			//PreviewOneUpdate();
			return;
		}
		if(isPreviewingAll)
		{
			//PreviewAllUpdate();
			return;
		}
	}

	void PreviewAllUpdate()
	{
		X2DAnimationClip[] clipArray = anima.clipArray;
		if(clipArray.Length == 0) return;

		timer += Time.deltaTime;
		X2DAnimationClip clip = clipArray[curClipIndex];
		if(timer >= clip.frameList[curFrame].duration)
		{
			curFrame++;
			if(curFrame >= clip.frameList.Count)
			{
				curFrame = 0;
				curClipIndex++;
				if(curClipIndex >= clipArray.Length)
					curClipIndex = 0;
			}
			timer = 0;
			anima.PlayFrame(clipArray[curClipIndex], curFrame);
		}
	}
	
	void PreviewOneUpdate()
	{
		if(previewClip == null) return;
		if(curFrame >= previewClip.frameList.Count)
			curFrame = 0;
		timer += Time.deltaTime;
		if(timer >= previewClip.frameList[curFrame].duration)
		{
			curFrame++;
			timer = 0;
			if(curFrame >= previewClip.frameList.Count)
				curFrame = 0;
			anima.PlayFrame(previewClip, curFrame);
		}
	}

	//预览全部时，在这里切换到下一帧
	public void OnClipPlayEnd()
	{
		curClipIndex++;
		if(curClipIndex >= anima.clipArray.Length)
			curClipIndex = 0;
		anima.PlayClip(anima.clipArray[curClipIndex], 0);
	}

	public void PreviewAll()
	{
		if(anima == null)
			anima = GetComponent<X2DAnimation>();
		isPreviewingAll = true;
		isPreviewingOne = false;
		anima.EditorStartPreview(this, anima.clipArray[curClipIndex], true, OnClipPlayEnd);
	}

	public void PreviewOne(X2DAnimationClip clip)
	{
		if(anima == null)
			anima = GetComponent<X2DAnimation>();
		previewClip = clip;
		isPreviewingAll = false;
		isPreviewingOne = true;
		anima.EditorStartPreview(this, clip, false, null);
	}

	public void StopPreview()
	{
		anima.EditorStopPreview(this);
		anima = null;
		previewClip = null;
		isPreviewingOne = false;
		isPreviewingAll = false;
	}
}
