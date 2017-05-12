using UnityEngine;
using System.Collections;

public class ActorAttributeDataCollection : MonoBehaviour
{
	private static ActorAttributeDataCollection instance;
	public ActorAttributeData[] actorAttrbuteData;
	
	void Awake()
	{
		instance = this;
	}
	
	void OnDestroy()
	{
		instance = null;
	}
	
	public static ActorAttributeData GetActorAttributeData(int id)
	{
		if(id < 0 || id >= instance.actorAttrbuteData.Length) return null;
		foreach(ActorAttributeData item in instance.actorAttrbuteData)
		{
			if(item.actorID == id)
				return item;
		}
		Debug.LogErrorFormat("角色属性集中没有id为{0}的记录", id);
		return null;
	}
}
