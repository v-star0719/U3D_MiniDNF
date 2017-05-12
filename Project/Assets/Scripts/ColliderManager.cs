using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ColliderManager : MonoBehaviour 
{
	static public LinkedList<ObjectCollider> colliderList = new LinkedList<ObjectCollider>();

	// Use this for initialization
	void Start ()
 	{
	}

	// Update is called once per frame
	void LateUpdate ()
	{
		LinkedListNode<ObjectCollider> node1 = colliderList.First;
		while(node1 != null)
		{
			LinkedListNode<ObjectCollider> node2 = node1.Next;
			while(node2 != null)
			{
				if(IsCollision(node1.Value, node2.Value))
				{
					if(node1.Value.AddToCollisionObjectList(node2.Value))
					{
						//Debug.Log("In: " + c1.name + "---" + c2.name); 
						node1.Value.EnterCollier(node2.Value);
					}
					if(node2.Value.AddToCollisionObjectList(node1.Value))
					{
						//Debug.Log("In: " + c2.name + "---" + c1.name); 
						node2.Value.EnterCollier(node1.Value);
					}
				}
				else
				{
					if(node1.Value.RemoveFromCollisionObjectList(node2.Value))
					{
						//Debug.Log("Out: " + c1.name + "---" + c2.name);
					}
					if(node2.Value.RemoveFromCollisionObjectList(node1.Value))
					{
						//Debug.Log("Out: " + c2.name + "---" + c1.name); 
					}
				}
				node2 = node2.Next;
			}
			node1 = node1.Next;
		}
	}

	//需要统一到世界坐标下处理，size也要受scale影响
	private static bool IsCollision(ObjectCollider c1, ObjectCollider c2)
	{
		return IsCollision(c1.realPos, c1.size, c2.realPos, c2.size);
	}

	//Box和Box
	public static bool IsCollision(Vector3 center1, Vector3 size1, Vector3 center2, Vector3 size2)
	{
		//两个矩形水平是否相交的判断：x方向两个点之间的距离小于两个矩形的宽度和的一半，以此类推到3维即可
		Vector3 dict = center1 - center2;
		if(dict.x < 0) dict.x = -dict.x;
		if(dict.y < 0) dict.y = -dict.y;
		if(dict.z < 0) dict.z = -dict.z;

		//不相交的最小距离，不考虑中心点
		Vector3 minDict = (size1 + size2)*0.5f;
		return (dict.x < minDict.x && dict.y < minDict.y && dict.z < minDict.z);
	}

	//Box和Sphere，这个不好判断，先用立方体做判断吧
	public static bool IsCollision(Vector3 center1, float radius, Vector3 center2, Vector3 size2)
	{
		//圆和矩形水平是否相交：x方向的两个中心点距离和小于直径和宽的一半，以此类推到3维即可
		Vector3 dict = center1 - center2;
		if(dict.x < 0) dict.x = -dict.x;
		if(dict.y < 0) dict.y = -dict.y;
		if(dict.z < 0) dict.z = -dict.z;

		//不相交的最小距离，不考虑中心点
		Vector3 minDict = Vector3.zero;
		minDict.x = radius + size2.x*0.5f;
		minDict.y = radius + size2.y*0.5f;
		minDict.z = radius + size2.z*0.5f;
		return (dict.x < minDict.x && dict.y < minDict.y && dict.z < minDict.z);
	}

	private static bool IsCollision(Vector3 center1, Vector3 size1, Vector3 center2, Vector3 size2, out Vector3 collideSign)
	{
		collideSign = Vector3.zero;
		//两个矩形水平是否相交的判断：x方向两个点之间的距离小于两个矩形的宽度和的一半，以此类推到3维即可
		Vector3 dict = center1 - center2;
		if(dict.x < 0) dict.x = -dict.x;
		if(dict.y < 0) dict.y = -dict.y;
		if(dict.z < 0) dict.z = -dict.z;

		//不相交的最小距离，不考虑中心点
		Vector3 minDict = (size1 + size2)*0.5f;
		if(dict.x < minDict.x) collideSign.x = 1;
		if(dict.y < minDict.y) collideSign.y = 1;
		if(dict.z < minDict.z) collideSign.z = 1;

		return (collideSign.x > 0 && collideSign.y > 0 && collideSign.z > 0);
	}

	public static void Regist(ObjectCollider c)
	{
		if(colliderList.Find(c) == null)
		{
			colliderList.AddLast(c);
			//Debug.Log("colliderList.count = " + colliderList.Count);
		}
		else
			Debug.LogError(c.name + " is alread existed");
	}

	public static void UnRegist(ObjectCollider c)
	{
		if(colliderList.Remove(c))
		{
			//Debug.Log("colliderList.count = " + colliderList.Count);
		}
		else
			Debug.LogError(c.name + " is not existed");
	}

	//触发器不参与判断，返回false
	public static bool IsInCollision(ObjectBase ob)
	{
		ObjectCollider c = ob.GetComponent<ObjectCollider>();
		if(c == null) return false;

		LinkedListNode<ObjectCollider> node = colliderList.First;
		while(node != null)
		{
			if(node.Value != c && !node.Value.isTrriger)//不要和自己比较
			{
				if(IsCollision(c, node.Value))
					return true;
			}
			node = node.Next;
		}
		return false;
	}

	public static bool MoveWithCollision(ObjectBase ob, Vector3 newPos)
	{
		if(colliderList.Count == 0) return true;
		
		ObjectCollider c = ob.GetComponent<ObjectCollider>();
		if(c == null) return false;

		//对所有物体进行碰撞判断
		Vector3 oldPos = ob.realPos;
		Vector3[] moveSignArray = new Vector3[colliderList.Count];//xyz：0表示这个方向不会碰撞，1表示会碰撞
		LinkedListNode<ObjectCollider> node1 = colliderList.First;
		int i=-1;
		while(node1 != null)
		{
			i++;
			if(node1.Value.gameObject.layer != LayerMask.NameToLayer("Barrier"))
			{
				node1 = node1.Next;
				continue;
			}
			if(node1.Value == c)
			{
				node1 = node1.Next; 
				continue;
			}

			Vector3 sign = moveSignArray[i];
			Vector3 t = oldPos;
			t.x = newPos.x;
			if(IsCollision(t, c.size, node1.Value.realPos, node1.Value.size))
				sign.x = 1;

			t.x = oldPos.x;
			t.y = newPos.y;
			if(IsCollision(t, c.size, node1.Value.realPos, node1.Value.size))
				sign.y = 1;

			t.y = oldPos.y;
			t.z = newPos.z;
			if(IsCollision(t, c.size, node1.Value.realPos, node1.Value.size))
				sign.z = 1;

			node1 = node1.Next;
		}
		Debug.Log(i);

		//确定最终可以移动至的位置
		//如果所有的x都为0，说明x是可以移动的
		//for()


		return false;
	}
}
