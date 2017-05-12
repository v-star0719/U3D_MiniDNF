using UnityEngine;
using System.Collections;

[System.Serializable]
public class ActorAttributeData
{
	///编号
	public int actorID;
	///角色名
	public string charactorName;
	///
	public string animationName;
	///头像ID
	public int portraitID;
	///照片ID
	public int photoID;
	///普攻ID
	public int normalAttackID;
	///技能ID
	public int[] skillIdList;
	///血量
	public float hp;
	///攻击
	public float attack;
	///减伤
	public int mitigation;
	///防御
	public int defense;
	///血量成长
	public float hpGrowth;
	///攻击成长
	public float attackGrowth;
	///减伤
	public int mitigationGrowth;
	///防御成长
	public int defenseGrowth;
	/////一秒钟多少次攻击，基准为1
	public float attackSpeed;
	///
	public int walkSpeed;
	///
	public int runSpeed;
	///
	public int normalAttackDistance;
	///
	public float patrolRadius;
	///
	public Vector3 size;
	///
	public float jumpHeight;
	///
	public float jumpYSpeed;
	///
	public float jumpBackHeight;
	///
	public float jumpBackXSpeed;
	///
	public float jumpBackYSpeed;
	///
	public float falldownAcc;
}
