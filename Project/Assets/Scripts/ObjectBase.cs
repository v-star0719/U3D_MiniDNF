using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class ObjectBase : MonoBehaviour
{
	//2D世界，每一个物体都对应一个动画或者图片
	private X2DAnimation _x2dAnim;
	public X2DAnimation x2dAnim
	{
		get
		{
			if(_x2dAnim == null) _x2dAnim = GetComponent<X2DAnimation>();
			return _x2dAnim;
		}
		set
		{
			_x2dAnim = value;
		}
	}

	private X2DSprite _x2dSprite;
	public X2DSprite x2dSprite
	{
		get
		{
			if(_x2dSprite == null) _x2dSprite = GetComponent<X2DSprite>();
			return _x2dSprite;
		}
		set
		{
			_x2dSprite = value;
		}
	}

	private X2DEffect _x2dEffect;
	public X2DEffect x2dEffect
	{
		get
		{
			if(_x2dEffect == null) _x2dEffect = GetComponent<X2DEffect>();
			return _x2dEffect;
		}
		set
		{
			_x2dEffect = value;
		}
	}

	//[System.NonSerialized] 
	public ThreeeDMirror mirror;

	//transform的坐标屏幕上展示时的渲染位置，并不是实际运算逻辑里的位置
	//screen.y = (w.y + w.z)*0.7071
	[HideInInspector][SerializeField]
	private Vector3 _realLocalPos;
	public Vector3 realLocalPos
	{
		get{
			return _realLocalPos;
		}
		set
		{
			_realLocalPos = value;
			Vector3 pos = value;
			pos.y = value.y + value.z * 0.7071f;
			pos.z += _renderZOffset;
			transform.localPosition = pos;
			//同时刷新世界坐标
			_realPos = transform.position;
			_realPos.y = _realPos.y - _realPos.z * 0.7071f;
		}
	}
	[HideInInspector][SerializeField]
	private Vector3 _realPos;
	public Vector3 realPos
	{
		get
		{
			return _realPos;
		}
		set
		{
			_realPos = value;
			Vector3 pos = value;
			pos.y = value.y + value.z * 0.7071f;
			pos.z += renderZOffset;// 这里没处理scale，处理的话很麻烦
			transform.position = pos;
			//同时刷新局部坐标
			_realLocalPos = transform.localPosition;
			_realLocalPos.y = _realLocalPos.y - (_realLocalPos.z - renderZOffset) * 0.7071f;
		}
	}

	[HideInInspector][SerializeField]
	private float _renderZOffset;//渲染的物体Z值增加一个偏移量，主要是使地面的植被放到背景那层
	public float renderZOffset
	{
		get
		{
			return _renderZOffset;
		}
		set
		{
			_renderZOffset = value;
			Vector3 pos = _realLocalPos;
			pos.y = _realLocalPos.y + _realLocalPos.z * 0.7071f;
			pos.z += _renderZOffset;
			transform.localPosition = pos;
		}
	}

	private bool _isFacingRight = true;//默认物体是朝向右的
	public bool isFacingRight
	{
		get{return _isFacingRight;}
		set
		{
			if(_isFacingRight == value) return;
			_isFacingRight = value;
			if(x2dAnim != null)
				x2dAnim.isFaceRight = value;
			if(x2dEffect != null)
				x2dEffect.isFacingRight = value;
			//transform.localEulerAngles = _isFacingRight ? Vector3.zero : new Vector3(0f, 180f, 0f);//转180度
		}
	}

	public bool editMode;//编辑模式下，unity坐标映射为逻辑坐标。正常情况下是逻辑坐标映射为unity坐标。

	//获取基于朝向的坐标，如果朝向改变，那么坐标也得改变
	public Vector3 GetFaceBasedPos(Vector3 pos)
	{
		if(isFacingRight) return pos;
		pos.x = -pos.x;
		return pos;
	}

	public bool IsInFrontOfMe(Vector3 pos)
	{
		if(isFacingRight)
			return pos.x - realLocalPos.x > 0f;
		else
			return pos.x - realLocalPos.x < 0f;
	}

	//这是为EnemyMark开的后门
	public void ApplyransformPos()
	{
		_realLocalPos = TwoDToWorldPos(transform.localPosition);
		_realPos = TwoDToWorldPos(transform.position);
	}

	protected void Update()
	{
		//Debug.Log("Update");
		Create3DMirror();
		X2DTools.DrawPosMark(gameObject, transform.position, gameObject.layer);
	}

	protected void OnEnable()
	{
		Create3DMirror();
		//Debug.Log("OnEnable");
	}

	protected void OnDestroy()
	{
		Destroy3DMirror();
	}

	private void Create3DMirror()
	{
		if(mirror == null)
		{
			ThreeDMirrorManager.Create3DMirror(this);
		}
	}

	private void Destroy3DMirror()
	{
		if(mirror != null)
		{
			DestroyImmediate(mirror.gameObject);
			mirror = null;
		}
	}

	public static Vector3 WorldTo2DPos(Vector3 pos)
	{
		pos.y = pos.y + pos.z * 0.7071f;
		return pos;
	}

	public static Vector3 TwoDToWorldPos(Vector3 pos)
	{
		pos.y = pos.y - pos.z * 0.7071f;
		return pos;
	}
}
