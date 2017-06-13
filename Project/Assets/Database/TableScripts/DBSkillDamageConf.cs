using UnityEngine;

[System.Serializable]
public class DBSkillDamageConf
{
	///编号
	public int ID;
	///攻击描述
	public string desc;
	///buffID
	public int buffID;
	///生效时间
	public float buffEnableTime;
	///音效
	public string hitSoundName;
	///攻击百分比伤害
	public int damagePercent;
	///固定伤害值
	public int damageFixed;
	///伤害开始时间
	public float startTime;
	///伤害间隔
	public float interval;
	///伤害次数
	public int times;
	///伤害中心类型
	public EmDamageCenterType damageCenterType;
	///伤害中心位置
	public Vector3 damageCenterPos;
	///伤害范围
	public EmDamageArenaType damageArenaType;
	///伤害距离
	public float damageArenaRadius;
	///伤害区域尺寸
	public Vector3 damageArenaSize;
	///站地上被击动画
	public string actorBeHitStandOnGroundAnima;
	///浮空被击动画
	public string actorBeHitFlyInAirAnima;
	///伤害移动效果
	public EmDamageMoveEffect damageMoveEffect;
	///伤害状态效果
	public EmDamageStatusEffect damageStatusEffect;
	///概率
	public int damageStatusEffectProbability;
	///持续时间
	public float damageStatusEffectDuration;
	///被击移动时间
	public float hitMoveTime;
	///被击移动到指定位置，相对于释放者
	public Vector2 hitMoveTo;
	///移动曲线名
	public string hitMoveToCurveName;
	///被击移动按向量移动
	public Vector2 hitMoveVector;
	///移动曲线名
	public string hitMoveVectorCurveName;
	///被击移动速度
	public Vector2 hitMoveSpeed;
}