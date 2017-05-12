using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public enum EmX2DAnimationPlayMode
{
	Once,
	Loop,
	Pingpong
}

[System.Serializable]
public class X2DAnimationFrame
{
	public Texture2D tex;
	public float duration;
	public float startTime;//在整个帧序列里，该帧的播放时刻，也就是上一帧的结束时间，自动计算
	public Vector2 pos;
	public Vector2 size;
	public Vector2 region;

	public float endTime
	{
		get{return startTime + duration;}
	}
}

public class X2DAnimationClip : MonoBehaviour
{
	public const string DEFAULT_SHADER_NAME = "X2D/UnlitCullOff";

	[SerializeField][HideInInspector]
	private string _renderShaderName;
	private Shader _renderShader;
	public string clipName;
	public float duration;
	public EmX2DAnimationPlayMode playMode = EmX2DAnimationPlayMode.Once;
	public List<X2DAnimationFrame> frameList;

	//坐标变化功能，需要反馈到上层container，坐标直接写在curve里
	public bool hasPosCurve;
	public AnimationCurve xPosCurve;
	public AnimationCurve yPosCurve;

	[SerializeField] private bool _posCorrecting = false;
	private bool hasReadFramePos = false;
	public List<Transform> framePosMarkList = new List<Transform>();
	private List<Material> materialList = new List<Material>();

	public string renderShaderName
	{
		get
		{
			if(string.IsNullOrEmpty(_renderShaderName))
				return DEFAULT_SHADER_NAME;
			else return _renderShaderName;
		}
		set
		{
			_renderShaderName = value;
		}
	}

	public Vector2 GetPos(float time)
	{
		if(!hasPosCurve){
			Debug.LogFormat("clip = {0} has no pos curve", clipName);
			return Vector2.zero;
		}
		Vector2 pos = Vector2.zero;
		float f = time;
		pos.x = xPosCurve.Evaluate(f);
		pos.y = yPosCurve.Evaluate(f);
		return pos;
	}

	///返回-1说明播到头了
	public int GetCurFrame(float timer, int curFrame, bool isForward, float acc)
	{
		if(isForward)
		{
			for(int i=curFrame; i<frameList.Count; i++)
			{
				X2DAnimationFrame frame = frameList[i];
				if(timer < frame.endTime*acc)
					return i;
			}
		}
		else
		{
			for(int i=curFrame; i>=0; i--)
			{
				X2DAnimationFrame frame = frameList[i];
				if(timer > frame.startTime*acc)
					return i;
			}
		}
		return -1;
	}

	public bool posCorrecting
	{
		get{return _posCorrecting;}
		set{_posCorrecting = value;}
	}
	
	//在editor中处理
	public void Update_PosCorrecting()
	{
		if(!posCorrecting)
		{
			hasReadFramePos = false;
			for(int i=0; i<framePosMarkList.Count; i++){
				if(framePosMarkList[i] != null)
					DestroyImmediate(framePosMarkList[i].gameObject);
			}
			framePosMarkList.Clear();
			return;
		}

		if(!hasReadFramePos)
		{
			//初始化，创建位置对象
			for(int i=0; i<frameList.Count; i++)
			{
				Transform trans = null;
				if(i >= framePosMarkList.Count)
				{
					//不够，新建
					GameObject go = GameObject.CreatePrimitive(PrimitiveType.Quad);
					go.layer = transform.gameObject.layer;
					trans = go.transform;
					trans.parent = transform;
					trans.name = i.ToString();
					framePosMarkList.Add(trans);
					Renderer rd = go.GetComponent<Renderer>();
					rd.sharedMaterial.shader = Shader.Find(renderShaderName);
				}
				else
					trans = framePosMarkList[i];

				trans.gameObject.SetActive(true);
				if(!hasReadFramePos){
					Vector3 pos = frameList[i].pos;
					pos.z = -i;
					trans.localPosition = pos;
					trans.localScale = frameList[i].size;
				}

				Material mtrl = null;
				if(i >= materialList.Count)
				{
					mtrl = new Material(Shader.Find(renderShaderName));
					materialList.Add(mtrl);
				}
				else
					mtrl = materialList[i];
				if(mtrl.shader.name != renderShaderName)
					mtrl.shader = Shader.Find(renderShaderName);

				MeshRenderer render = trans.GetComponent<MeshRenderer>();
				render.material = mtrl;
				mtrl.mainTexture = frameList[i].tex;
			}
			//多余的的隐藏
			for(int i=frameList.Count; i<framePosMarkList.Count; i++)
				framePosMarkList[i].gameObject.SetActive(false);
			hasReadFramePos = true;
			return;
		}
		else
		{
			//读取位置对象的位置
			for(int i=0; i<frameList.Count; i++)
			{
				frameList[i].pos = framePosMarkList[i].localPosition;
			}
		}
	}
}
