//该文件是通过脚本生成的，注意手动修改会被覆盖。
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//如果需要其他获取表记录的方法，请统一在DBDatabase里添加。
public class DBSkillInfoTable : MonoBehaviour
{
	public DBSkillInfoConf[] recordArray = new DBSkillInfoConf[]{};
	private static Dictionary<int, DBSkillInfoConf> recordDict = null;
	public static DBSkillInfoTable instance;

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
		recordDict = new Dictionary<int, DBSkillInfoConf>();
		for(int i=0; i<recordArray.Length; i++)
		{
			DBSkillInfoConf record = recordArray[i];
			if(!recordDict.ContainsKey(record.skillID))
				recordDict.Add(record.skillID, record);
			else
				Debug.LogErrorFormat("表DBSkillInfoTable有重复的记录，id = {0}", record.skillID);
		}
	}

	//获取记录，如果不存在返回null
	public static DBSkillInfoConf GetRecord(int skillID, bool errorMsg = true)
	{
		if(instance == null){
			Debug.LogError("表DBSkillInfoTable未加载");
			return null;
		}
		DBSkillInfoConf record = null;
		if(recordDict.TryGetValue(skillID, out record))
			return record;
		if(errorMsg)
			Debug.LogErrorFormat("表DBSkillInfoTable没有skillID = {0}的记录", skillID);
		return null;
	}
}