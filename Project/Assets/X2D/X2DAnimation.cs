using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class X2DAnimation : MonoBehaviour
{
	public delegate void VoidDelegate();

	public const string RENDERER_NAME = "#Renderer";

	public delegate void X2DAnimationPlayEndCallback();
	public X2DAnimationClip defaultClip;
	public X2DAnimationClip[] clipArray;
	
	public bool isForward = true;
	public bool playOnAwake = false;
	public Vector2 frameOffset;

	[HideInInspector]
	public X2DAnimationClip curClip;

	private float timer = 0;
	private int curFrame = 0;
	private X2DAnimationPlayEndCallback playEndCallback;
	private EmX2DAnimationPlayMode playMode;
	private bool isPlaying = false;

	public ActorBase actorContainer;
	public Transform normalContainer;

	public Vector2 curFrameTopLeft;
	public Vector2 curFrameBottomRight;

	private float accelerate;//如果是快两倍，为0.5f；慢两倍，为2

	//预览功能
	[HideInInspector]
	public bool isPreviewing = false;
	private VoidDelegate previewClipPlayEndCallbabck;

	//渲染器可以保存在预置里
	[SerializeField]
	private MeshRenderer _myRenderer;
	private MeshRenderer myRenderer
	{
		get
		{
			if(_myRenderer == null)
			{
				GameObject go = GameObject.CreatePrimitive(PrimitiveType.Quad);
				//属性设置
				go.name = RENDERER_NAME;
				go.layer = gameObject.layer;
				go.transform.parent = transform;
				go.transform.localScale = Vector3.one;
				go.transform.localRotation = Quaternion.identity;
				go.transform.localPosition = Vector3.zero;
				_myRenderer = go.GetComponent<MeshRenderer>();
				if(_myMaterial != null)
					_myRenderer.material = _myMaterial;//编辑器中直接删除renderer的话，材质还留着，复用即可
				Debug.Log("Create new X2DAnimation renderer");
			}
			else
			{
				//编辑过程中，可能会丢失材质，比如revert prefab后，renderer会重置
				if(_myMaterial != null)
					_myRenderer.material = _myMaterial;
			}
			return _myRenderer;
		}
	}

	//材质不序列化
	private Material _myMaterial;
	private Material myMaterial
	{
		get
		{
			if(_myMaterial == null)
			{
				_myMaterial = new Material(Shader.Find(X2DAnimationClip.DEFAULT_SHADER_NAME));
				_myMaterial.hideFlags = HideFlags.DontSave;
				myRenderer.material = _myMaterial;
			}
			return _myMaterial;
		}
	}

	//X方向翻转
	[HideInInspector][SerializeField]
	private bool _flipX = false;
	public bool flipX
	{
		get { return _flipX; }
		set
		{
			if(_flipX == value) return;
			_flipX = value;
			if(_flipX)
				myMaterial.SetFloat("_FlipX", 1);
			else
				myMaterial.SetFloat("_FlipX", 0);
		}
	}

	//y方向翻转
	[HideInInspector][SerializeField]
	private bool _flipY = false;
	public bool flipY
	{
		get { return _flipY; }
		set
		{
			if(_flipY == value) return;
			_flipY = value;
			if(_flipY)
				myMaterial.SetFloat("_FlipY", 1);
			else
				myMaterial.SetFloat("_FlipY", 0);
		}
	}

	//是否朝右，默认是朝右的，朝左的话是关于y轴的对称图形。例如角色朝右朝左射的箭
	//对称轴是以X2DAnimation为原点的，所以作为特效剪辑时，X2DAnimation的坐标值应保持为（0，0，0），这样就也是以特效根节点为原点了。
	[HideInInspector][SerializeField]
	private bool _isFaceRight = true;
	public bool isFaceRight
	{
		get{return _isFaceRight;}
		set
		{
			if(_isFaceRight == value) return;
			_isFaceRight = value;
			flipX = !flipX;
			RefreshCurFrame();
		}
	}

	public float playTime
	{
		get
		{
			if(curClip == null) return 0f;
			return curClip.duration * accelerate;
		}
	}

	public float playTimer
	{
		get{return timer;}
	}

	void Awake()
	{
#if UNITY_EDITOR
		if(!Application.isPlaying) return;
#endif

		if(playOnAwake && defaultClip != null){
			Play(defaultClip.clipName);
		}

		//序列化的时候不是走的Set方法，这里强制走一下
		_flipX = !_flipX; flipX = !_flipX;
		_flipY = !_flipY; flipY = !_flipY;
	}

	void OnEnable()
	{
		if(!Application.isPlaying)
		{
			//序列化的时候不是走的Set方法，这里强制走一下
			_flipX = !_flipX; flipX = !_flipX;
			_flipY = !_flipY; flipY = !_flipY;
		}
	}

	void Update()
	{
		X2DTools.DrawPosMark(gameObject, transform.position, gameObject.layer);

		#if UNITY_EDITOR
		if(!Application.isPlaying)
		{
			EditorPlayFirstFrame();
			return;
		}
		#endif

		if(!isPlaying)
			return;
		if(curClip == null){
			isPlaying = false;
			return;
		}

		float deltaTime = Time.deltaTime;
		if(isForward) timer += deltaTime;
		else timer -= deltaTime;

		//根据当前所处的时间获得应该播放的帧
		//如果是播一帧然后等这一帧播放后继续下帧，累计误差问题不好处理
		int nextFrame = curClip.GetCurFrame(timer, curFrame, isForward, accelerate);
		if(nextFrame != -1)
		{
			if(nextFrame != curFrame){
				PlayFrame(curClip, nextFrame);
				curFrame = nextFrame;
			}
		}
		else
		{
			//播完了一溜
			if(playMode == EmX2DAnimationPlayMode.Once)
				isPlaying = false;
			else
			{
				if(playMode == EmX2DAnimationPlayMode.Pingpong)
					isForward = !isForward;

				int firstFrame = 0;
				timer = 0f;
				if(!isForward)
				{
					firstFrame = curClip.frameList.Count-1;
					timer = curClip.duration;
				}

				if(playMode == EmX2DAnimationPlayMode.Loop)
					curFrame = firstFrame;
				else if(playMode == EmX2DAnimationPlayMode.Pingpong)
				{
					float t = curClip.frameList[curFrame].duration;
					if(isForward)
					{
						timer += t;//不再在最后一帧做停留
						curFrame++;
					}
					else
					{
						timer -= t;//不再在最后一帧做停留
						curFrame--;
					}
				}
				PlayFrame(curClip, curFrame);
			}

			//预览全部
			if(previewClipPlayEndCallbabck != null)
				previewClipPlayEndCallbabck();
		}

		if(curClip.hasPosCurve)
		{
			if(actorContainer != null)
				actorContainer.objectbase.realLocalPos = GetCurAnimationPos();
			if(normalContainer != null)
				normalContainer.localPosition = GetCurAnimationPos();
		}
	}

	//这里就不考虑朝向了
	public Vector3 GetCurAnimationPos()
	{
		if (!curClip.hasPosCurve) return Vector3.zero;

		Vector3 pos3 = Vector3.zero;
		Vector2 pos2 = curClip.GetPos(timer / accelerate);
		//if(isFaceRight)
		//	pos3.x = pos2.x;
		//else
		//	pos3.x = -pos2.x;
		pos3.x = pos2.x;
		pos3.y = pos2.y;
		return pos3;
	}

	public void PlayFrame(X2DAnimationClip anima, int frameIndex)
	{
		//Debug.Log(index + ", " + timer);
		X2DAnimationFrame frame = anima.frameList[frameIndex];
		string shaderName = anima.renderShaderName;
		if(myMaterial.shader == null)
			myMaterial.shader = Shader.Find(shaderName);
		else if(myMaterial.shader.name != shaderName)
			myMaterial.shader = Shader.Find(shaderName);

		myMaterial.mainTexture = frame.tex;
		myRenderer.transform.localScale = new Vector3(frame.size.x, frame.size.y, 1);
		Vector3 pos = anima.transform.localPosition;
		pos.x += frame.pos.x;//pos是相对于frameRegion左上角的偏移量
		pos.y += frame.pos.y;//pos是相对于frameRegion左上角的偏移量（y轴竖直向下）
		if(!isFaceRight)
			pos.x = -pos.x;//关于y轴对称
		myRenderer.transform.localPosition = pos;

		curFrameTopLeft.x = pos.x - frame.size.x * 0.5f;
		curFrameTopLeft.y = pos.y - frame.size.y * 0.5f;
		curFrameBottomRight = curFrameTopLeft - frame.size;
	}

	private void RefreshCurFrame()
	{
		if(curClip != null)
			PlayFrame(curClip, curFrame);
	}

	///clipName剪辑名称，如果是null或""，则播放默认剪辑
	///acc加速值，大于1表示加速，等于1表示不加速，小于1表示减速
	public void Play(string clipName, float acc = 1)
	{
		myRenderer.gameObject.SetActive(true);

		if(string.IsNullOrEmpty(clipName)){
			PlayClip(defaultClip, acc);
			return;
		}

		isPlaying = false;
		for(int i=0; i<clipArray.Length; i++)
		{
			X2DAnimationClip clip = clipArray[i];
			if(clip.clipName == clipName)
			{
				PlayClip(clip, acc);
				return;
			}
		}
		Debug.LogErrorFormat("clip is not found: {0}/{1}", name, clipName);
	}

	public void Play(string clipName, EmX2DAnimationPlayMode pm)
	{
		Play(clipName);
		playMode = pm;
	}

	public void PlayClip(X2DAnimationClip clip, float acc = 1)
	{
		isPlaying = true;
		curClip = clip;
		playMode = curClip.playMode;
		curFrame = isForward ? 0 : curClip.frameList.Count - 1;
		PlayFrame(curClip, curFrame);
		timer = isForward ? 0f : curClip.duration;
		accelerate = acc;
	}

	//停止会隐藏renderer
	public void Stop()
	{
		isPlaying = false;
		myRenderer.gameObject.SetActive(false);
	}

	public void SetContainer(Transform trans)
	{
		normalContainer = trans;
	}

	public bool IsPlaying()
	{
		return isPlaying;
	}

	//////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////////////////////////////////////////////////
	//下面是编辑器功能

	//用于在编辑器中显示固定的一帧
	private void EditorPlayFirstFrame()
	{
		if(defaultClip == null) return;
		PlayFrame(defaultClip, 0);
	}

	//做下标记，不再刷新帧，保存当前播放状态
	public void EditorStartPreview(X2DAnimationPreviewer previewer, X2DAnimationClip clip, bool previewAll, VoidDelegate callback)
	{
		previewer.playingClip = curClip;
		previewer.playMode = playMode;
		previewer.curFrame = curFrame;
		previewer.timer = timer;
		previewer.isForward = isForward;
		previewer.actorContainer = null;
		previewer.normalContainer = normalContainer;

		previewClipPlayEndCallbabck = callback;
		normalContainer = transform;
		isPreviewing = true;
		PlayClip(clip, 1);
	}

	//继续刷新帧，回复当前播放状态
	public void EditorStopPreview(X2DAnimationPreviewer previewer)
	{
		curClip = previewer.playingClip;
		playMode = previewer.playMode;
		curFrame = previewer.curFrame;
		timer = previewer.timer;
		isForward = previewer.isForward;
		actorContainer = previewer.actorContainer;
		normalContainer = previewer.normalContainer;
		isPreviewing = false;
		isPlaying = true;
		previewClipPlayEndCallbabck = null;
	}

	//
	public void EditorHideRenderer()
	{
		myRenderer.gameObject.SetActive(false);
	}

	public void EditorShowRenderer()
	{
		myRenderer.gameObject.SetActive(true);
	}
}
