using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;

//偷个懒，不动态加载场景元素，直接引用prefab引用，地图载入后自动进行实例化

public class SceneObjectMark : MonoBehaviour
{
	public GameObject prefab;

	[HideInInspector]
	public GameObject prefabInstance;

	// Use this for initialization
	void Start ()
	{
		//不自动创建实例，这会和手动创建冲突，统一要求手动创建
		//CreateInstance();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void CreateInstance()
	{
		if(prefabInstance != null) return;

		prefabInstance = GameObject.Instantiate<GameObject>(prefab);
		prefabInstance.transform.parent = transform;
		prefabInstance.transform.localPosition = Vector3.zero;
		prefabInstance.transform.localRotation = Quaternion.identity;
		prefabInstance.transform.localScale = Vector3.one;
	}

	public void DeleteInstance()
	{
		if(prefabInstance == null) return;

		if(!Application.isPlaying)
			GameObject.DestroyImmediate(prefabInstance);
		else
			GameObject.Destroy(prefabInstance);
		prefabInstance = null;
	}


		#if UNITY_EDITOR
	///编辑器菜单，一次性把选中节点下的SceneObjectMark都进行实例化
	[MenuItem("SceneEdit/CreateObjectInstance")]
	public static void EditorCreateInstanceBat()
	{
		if(Selection.activeGameObject == null) return;

		SceneObjectMark[] array = Selection.activeGameObject.GetComponentsInChildren<SceneObjectMark>(true);
		foreach(SceneObjectMark item in array)
		{
			item.CreateInstance();
		}
	}
		#endif
	
		#if UNITY_EDITOR
	///编辑器菜单，一次性把选中节点下的SceneObjectMark实例全都删除
	[MenuItem("SceneEdit/DeleteObjectInstance")]
	public static void EditorDeleteInstanceBat()
	{
		if(Selection.activeGameObject == null) return;
		
		SceneObjectMark[] array = Selection.activeGameObject.GetComponentsInChildren<SceneObjectMark>(true);
		foreach(SceneObjectMark item in array)
		{
			item.DeleteInstance();
		}
	}
		#endif
}
