using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//创建一个3D镜像ThreeeDMirrorReality
[ExecuteInEditMode]
public class ThreeeDMirror : MonoBehaviour
{
	public ObjectBase realObject;

	// Use this for initialization
	void Start ()
 	{
	}

//#if UNITY_EDITOR
	// Update is called once per frame
	public void Update ()
	{
		if(realObject == null)
		{
			enabled = false; 
			DestroyMyself();
			return;
		}

		Update_Name();
		Update_Pos();
		UpdateSize();
	}

	public void Update_Name()
	{
		if(name != "MIRROR#" + realObject.name)
			name = "MIRROR#" + realObject.name;
	}

	public void Update_Pos()
	{
		Vector3 pos = Vector3.zero;
		ObjectCollider collider = realObject.GetComponent<ObjectCollider>();
		if(collider != null)
			pos = collider.realPos;
		else
			pos = realObject.realPos;
		pos.x += ThreeDMirrorManager.MIRROR_OFFSET;//平移到镜像所在区域

		transform.position = pos;
	}

	public void UpdateSize()
	{
		ObjectCollider collider = realObject.GetComponent<ObjectCollider>();
		Vector3 size = Vector3.zero;
		if(collider == null)
			size = new Vector3(5, 5, 5);
		else if(collider.colliderType == EmObjectColliderType.Box)
			size = collider.size;
		else if(collider.colliderType == EmObjectColliderType.Sphere)
			size = new Vector3(collider.radius, collider.radius, collider.radius);
		transform.localScale = size;
	}

	void DestroyMyself()
	{
		Debug.Log("Destroy: " + name);
		DestroyImmediate(gameObject);
	}

//#endif
}
