using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//在这里为所有的Object创建3D镜像
[ExecuteInEditMode]
public class ThreeDMirrorManager : MonoBehaviour 
{
	public const float MIRROR_OFFSET = 5000;

	public ThreeeDMirror cubePrefab;
	public ThreeeDMirror spherePrefab;

	private static ThreeDMirrorManager instance;

	void OnEnable() 
	{
		instance = this;
	}

	void OnDisable()
	{
		instance = null;
	}

	void OnDestroy()
	{
		instance = null;
	}

	// Update is called once per frame
	void Update ()
	{
	}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.blue * 0.2f;
		Gizmos.DrawCube(transform.position, new Vector3(1000, 1, 1000));
	}

	public static void Create3DMirror(ObjectBase ob)
	{
		if(instance == null){
			//Debug.LogError("Create3DMirror.instance == null");
			return;
		}
		instance.Create3DMirror_(ob);
	}
	private void Create3DMirror_(ObjectBase ob)
	{
		ThreeeDMirror mirror = GameObject.Instantiate<ThreeeDMirror>(cubePrefab);
		mirror.gameObject.hideFlags = HideFlags.DontSave;
		mirror.transform.parent = transform;
		ob.mirror = mirror;
		mirror.realObject = ob;
		mirror.Update();
	}
}