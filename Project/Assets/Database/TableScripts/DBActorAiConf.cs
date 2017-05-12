using UnityEngine;

[System.Serializable]
public class DBActorAiConf
{
	///编号
	public int actorID;
	///角色名
	public string charactorName;
	///待机概率
	public int idleRandom;
	///移动概率
	public int moveRandom;
	///待机最长时间
	public float idlMaxDuration;
	///移动最长时间
	public float moveMaxDuratoin;
	///跟踪时间
	public float traceDuration;
	///攻击准备时间
	public float attackPrepairDuration;
	///攻击间隔
	public float attackInterval;
	///普通攻击次数
	public float normalAttackTimes;
	///普通攻击间隔
	public float normalAttackInterval;
}