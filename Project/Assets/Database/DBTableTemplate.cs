//说明：
//1.表的模板文件，通过替换DBTableDefineTemplate，DBTableTemplate，DBTableRecordIDType, recordId即可得到实际代码文件
//2.生成实际表脚本时跳过这3行

//该文件是通过脚本生成的，注意手动修改会被覆盖。
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//如果需要其他获取表记录的方法，请统一在DBDatabase里添加。
public class DBTableTemplate : MonoBehaviour
{
	public DBTableDefineTemplate[] recordArray = new DBTableDefineTemplate[]{};
	private static Dictionary<DBTableRecordIDType, DBTableDefineTemplate> recordDict = null;
	public static DBTableTemplate instance;

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
		recordDict = new Dictionary<DBTableRecordIDType, DBTableDefineTemplate>();
		for(int i=0; i<recordArray.Length; i++)
		{
			DBTableDefineTemplate record = recordArray[i];
			if(!recordDict.ContainsKey(record.recordId))
				recordDict.Add(record.recordId, record);
			else
				Debug.LogErrorFormat("表DBTableTemplate有重复的记录，id = {0}", record.recordId);
		}
	}

	//获取记录，如果不存在返回null
	public static DBTableDefineTemplate GetRecord(DBTableRecordIDType recordId, bool errorMsg = true)
	{
		if(instance == null){
			Debug.LogError("表DBTableTemplate未加载");
			return null;
		}
		DBTableDefineTemplate record = null;
		if(recordDict.TryGetValue(recordId, out record))
			return record;
		if(errorMsg)
			Debug.LogErrorFormat("表DBTableTemplate没有recordId = {0}的记录", recordId);
		return null;
	}
}