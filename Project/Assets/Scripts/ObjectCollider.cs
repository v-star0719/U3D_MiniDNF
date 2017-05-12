using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum EmObjectColliderType
{
	Box,
	Sphere,
}

public class ObjectCollider : MonoBehaviour 
{
	public EmObjectColliderType colliderType;
	public bool isEnabled = true;//未激活的话，不参与碰撞检测，可以用来看3D镜像
	public bool isTrriger = false;//true的话不会产生碰撞效果，会穿过去，但是会产生碰撞消息

	public Vector3 center;//世界坐标
	public Vector3 size;
	public float radius;

	public List<ObjectCollider> collisionObjectList = new List<ObjectCollider>();

	private ObjectBase _objectBase;
	public ObjectBase objectBase
	{
		get 
		{
			if(_objectBase == null)
				_objectBase = gameObject.GetComponent<ObjectBase>();
			return _objectBase;
		}
	}

	public Vector3 realPos
	{
		get
		{
			Vector3 pos = objectBase.realPos + center;
			if(colliderType == EmObjectColliderType.Box)
				pos.y += size.y * 0.5f;
			else if(colliderType == EmObjectColliderType.Sphere)
				pos.y += radius * 0.5f;
			return pos;
		}
	}

	void OnEnable()
	{
		if(isEnabled) ColliderManager.Regist(this);
	}

	void OnDisable()
	{
		if(isEnabled) ColliderManager.UnRegist(this);
	}

	// Use this for initialization
	void Start ()
 	{

	}

	// Update is called once per frame
	void Update ()
	{

	}

	public bool AddToCollisionObjectList(ObjectCollider oc)
	{
		int i = collisionObjectList.IndexOf(oc);
		if(i < 0)
		{
			collisionObjectList.Add(oc);
			return true;
		}
		return false;
	}

	public bool RemoveFromCollisionObjectList(ObjectCollider oc)
	{
		int i = collisionObjectList.IndexOf(oc);
		if(i >= 0)
		{
			collisionObjectList.RemoveAt(i);
			return true;
		}
		return false;
	}

	public void EnterCollier(ObjectCollider target)
	{
		objectBase.SendMessage("OnEnterCollier", target, SendMessageOptions.DontRequireReceiver);
	}
}
