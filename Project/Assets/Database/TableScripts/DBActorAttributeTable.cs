//该文件是通过脚本生成的，注意手动修改会被覆盖。
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//如果需要其他获取表记录的方法，请统一在DBDatabase里添加。
public class DBActorAttributeTable : MonoBehaviour
{
	public DBActorAttributeConf[] recordArray = new DBActorAttributeConf[]{};
	private static Dictionary<int, DBActorAttributeConf> recordDict = null;
	public static DBActorAttributeTable instance;

	void Awake(){
		instance = this;
		Init();
	}
	void OnDestroy(){
		instance = null;
	}

	public void Init()
	{
		//如果recordDict不为null，说明已经初始化了
		if(recordDict != null)
			return;
		recordDict = new Dictionary<int, DBActorAttributeConf>();
		for(int i=0; i<recordArray.Length; i++)
		{
			DBActorAttributeConf record = recordArray[i];
			if(!recordDict.ContainsKey(record.actorID))
				recordDict.Add(record.actorID, record);
			else
				Debug.LogErrorFormat("表DBActorAttributeTable有重复的记录，id = {0}", record.actorID);
		}
	}

	//获取记录，如果不存在返回null
	public static DBActorAttributeConf GetRecord(int actorID, bool errorMsg = true)
	{
		if(instance == null){
			Debug.LogError("表DBActorAttributeTable未加载");
			return null;
		}
		DBActorAttributeConf record = null;
		if(recordDict.TryGetValue(actorID, out record))
			return record;
		if(errorMsg)
			Debug.LogErrorFormat("表DBActorAttributeTable没有actorID = {0}的记录", actorID);
		return null;
	}
}