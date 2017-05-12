using UnityEngine;

[System.Serializable]
public class DBSkillAttackConf
{
	///编号
	public int ID;
	///技能id
	public int skillID;
	///攻击名称
	public string attackName;
	///音效名
	public string soundName;
	///自动切到下个攻击
	public bool autoNextAttack;
	///释放时间，如果为0，则没有释放时间，并且对释放+动作的攻击忽略释放动作
	public float castTime;
	///角色攻击动画名称
	public string actorClipName;
	///攻击物名称
	public string attackObjectName;
	///是否无敌
	public bool isGod;
	///是否可被打断
	public bool couldInterrupt;
	///是否等特效播完再销毁
	public bool waitForObjectEnd;
	///攻击物出现位置
	public Vector2 attackObjectAppearPos;
	///攻击物出现时刻，从技能触发开始算
	public float attackObjectAppearTime;
	///持续时间
	public float duration;
	///连击开始时间
	public float comboStartTime;
	///伤害模式
	public int damageMode;
	///伤害表
	public int[] damages;
	///攻击物运动模式
	public EmAttackObjectMoveMode attackObjectMoveMode;
	///移动时间
	public float attackObjectMoveDuration;
	///移动开始时间
	public float attackObjectMoveStartTime;
	///移动向量
	public Vector2 attackObjectMoveVector;
	///移动曲线
	public string attackObjectMoveVectorCurve;
	///移动速度
	public Vector2 attackObjectMoveSpeed;
	///移动加速度
	public Vector2 attackObjectMoveAccelerate;
}