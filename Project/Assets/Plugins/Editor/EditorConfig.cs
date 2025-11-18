using UnityEngine;
using System.Collections;

public class EditorConfig
{
	//Application.dataPath最后没有带'/'，所以这里补上
	public const string dbTableScriptsDir = "/Scripts/ConfigTemplates/";
	public const string dbTablePrefabDir = "Resources/Configs/";
	public const string dbTableTemplatePath ="/Plugins/Editor/DBTableTemplate.cs";
	public const string dbTableNameTemplate = "DB{0}Table";
	public const string dbTableDefineNameTemplate = "DB{0}Record";//"DB{0}TableDefine"，换成conf后缀，代码中使用更直接
	public const string dbTableEnumPrefix = "Em";//表中枚举类型前缀
}
