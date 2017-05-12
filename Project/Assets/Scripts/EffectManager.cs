using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EffectObject
{
	//public ParticleSystem particleEffect;//先不处理这个
	public string effectName;
	public ObjectBase effectObject;
	public X2DEffect x2dEffect;//很容易就配错或者没配，应该兼容处理没有特效的情况
	public bool isAlive;
	public bool autoDestroy;
}

public class EffectManager : MonoBehaviour
{
	private static EffectManager instance;
	private static Dictionary<string, LinkedList<EffectObject>> effectDict = new Dictionary<string, LinkedList<EffectObject>>();//链表前面是可用的
	//记录已加载的特效，复用特效，缓存特效，删除特效
	//需要标记特效是否已死亡

	void Awake()
	{
		instance = this;
	}
	
	void OnDestroy()
	{
		instance = null;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		//优化，自动回收不遍历字典
		foreach(KeyValuePair<string, LinkedList<EffectObject>> kp in effectDict)
		{
			foreach(EffectObject eo in kp.Value)
			{
				if(eo.isAlive && eo.x2dEffect.hasPlayed && !eo.x2dEffect.isPlaying && eo.autoDestroy)
				{
					KillEffect(eo);//一次一个
					break;
				}
			}
		}
	
	}

	public static void KillEffect(EffectObject eo)
	{
		eo.isAlive = false;
		if(eo.effectObject != null)
		{
			eo.effectObject.gameObject.SetActive(false);
			eo.effectObject.transform.parent = instance.transform;
		}
		LinkedList<EffectObject> effectList = null;
		if(effectDict.TryGetValue(eo.effectName, out effectList))
		{
			LinkedListNode<EffectObject> node = effectList.Find(eo);
			effectList.Remove(node);
			effectList.AddFirst(node);
			//Debug.Log("Kill effect: " + eo.effectName);
		}
		else
			Debug.LogFormat("Effect {0} is not in effect manager", eo.effectName);
	}

	///如果_parent为NULL，则放在EffectManager节点下
	public static EffectObject GetEffect(string effectName, Transform _parent)
	{
		EffectObject eo = null;
		LinkedList<EffectObject> effectList = null;
		if(effectDict.TryGetValue(effectName, out effectList))
		{
			if(effectList.Count > 0)
			{
				eo = effectList.First.Value;
				if(!eo.isAlive)
				{
					LinkedListNode<EffectObject> node = effectList.First;
					effectList.RemoveFirst();
					effectList.AddLast(node);
					//Debug.LogFormat("Find exist effect: {0}, curEffectCount = {1}", effectName, effectList.Count);
				}
				else
					eo = null;//没有可用的
			}
			//执行到这里说明没有现成的了，后面创建新的
		}

		//如果列表为null，说明没有现成的列表
		if(effectList == null)
		{
			effectList = new LinkedList<EffectObject>();
			effectDict.Add(effectName, effectList);
		}

		if(eo == null)
		{
			//没有现成的可用，创建新的
			eo = new EffectObject();
			eo.x2dEffect = ResourceLoader.GetX2DEffect(effectName, _parent != null ? _parent : instance.transform);
			eo.effectObject = eo.x2dEffect.gameObject.GetComponent<ObjectBase>();
			if(eo.effectObject == null)
				eo.effectObject = eo.x2dEffect.gameObject.AddComponent<ObjectBase>();
			eo.effectName = effectName;
			effectList.AddLast(eo);
		}
		else if(eo.x2dEffect != null)
		{
			//已有，重置
			eo.effectObject.gameObject.SetActive(true);
			eo.effectObject.transform.localRotation = Quaternion.identity;
			if(_parent != null)
			{
				eo.effectObject.transform.parent = _parent;
				eo.effectObject.transform.localRotation = Quaternion.identity;//使用父节点的旋转，顺应父节点的朝向
			}
			else
			{
				eo.effectObject.transform.parent = instance.transform;
				eo.effectObject.isFacingRight = true;//初始化朝向
			}
		}

		//激活
		eo.isAlive = true;
		eo.x2dEffect.hasPlayed = false;
		eo.autoDestroy = false;

		//Debug.LogFormat("Create new effect: {0}, curEffectCount = {1}", effectName, effectList.Count);
		return eo;
	}
}
