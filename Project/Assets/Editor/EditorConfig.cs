using UnityEngine;
using System.Collections;

public class EditorConfig
{
	//Application.dataPath最后没有带'/'，所以这里补上
	public const string dbDirectory = "/Database/";
	public const string dbTableScriptsDir = dbDirectory + "TableScripts/";
	public const string dbTablePrefabDir = dbDirectory + "Tables/";
	public const string dbTableTemplatePath = dbDirectory + "DBTableTemplate.cs";
	public const string dbTableNameTemplate = "DB{0}Table";
	public const string dbTableDefineNameTemplate = "DB{0}Conf";//"DB{0}TableDefine"，换成conf后缀，代码中使用更直接
	public const string dbTableEnumPrefix = "Em";//表中枚举类型前缀
}
