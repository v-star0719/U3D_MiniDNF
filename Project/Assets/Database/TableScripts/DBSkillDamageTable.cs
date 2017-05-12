//该文件是通过脚本生成的，注意手动修改会被覆盖。
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//如果需要其他获取表记录的方法，请统一在DBDatabase里添加。
public class DBSkillDamageTable : MonoBehaviour
{
	public DBSkillDamageConf[] recordArray = new DBSkillDamageConf[]{};
	private static Dictionary<int, DBSkillDamageConf> recordDict = null;
	public static DBSkillDamageTable instance;

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
		recordDict = new Dictionary<int, DBSkillDamageConf>();
		for(int i=0; i<recordArray.Length; i++)
		{
			DBSkillDamageConf record = recordArray[i];
			if(!recordDict.ContainsKey(record.ID))
				recordDict.Add(record.ID, record);
			else
				Debug.LogErrorFormat("表DBSkillDamageTable有重复的记录，id = {0}", record.ID);
		}
	}

	//获取记录，如果不存在返回null
	public static DBSkillDamageConf GetRecord(int ID, bool errorMsg = true)
	{
		if(instance == null){
			Debug.LogError("表DBSkillDamageTable未加载");
			return null;
		}
		DBSkillDamageConf record = null;
		if(recordDict.TryGetValue(ID, out record))
			return record;
		if(errorMsg)
			Debug.LogErrorFormat("表DBSkillDamageTable没有ID = {0}的记录", ID);
		return null;
	}
}