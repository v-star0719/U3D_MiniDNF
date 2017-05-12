//该文件是通过脚本生成的，注意手动修改会被覆盖。
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//如果需要其他获取表记录的方法，请统一在DBDatabase里添加。
public class DBIconTable : MonoBehaviour
{
	public DBIconConf[] recordArray = new DBIconConf[]{};
	private static Dictionary<int, DBIconConf> recordDict = null;
	public static DBIconTable instance;

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
		recordDict = new Dictionary<int, DBIconConf>();
		for(int i=0; i<recordArray.Length; i++)
		{
			DBIconConf record = recordArray[i];
			if(!recordDict.ContainsKey(record.iconID))
				recordDict.Add(record.iconID, record);
			else
				Debug.LogErrorFormat("表DBIconTable有重复的记录，id = {0}", record.iconID);
		}
	}

	//获取记录，如果不存在返回null
	public static DBIconConf GetRecord(int iconID, bool errorMsg = true)
	{
		if(instance == null){
			Debug.LogError("表DBIconTable未加载");
			return null;
		}
		DBIconConf record = null;
		if(recordDict.TryGetValue(iconID, out record))
			return record;
		if(errorMsg)
			Debug.LogErrorFormat("表DBIconTable没有iconID = {0}的记录", iconID);
		return null;
	}
}