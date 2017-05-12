using UnityEngine;
using System.Collections;

//处理玩家输入，实现角色控制
public class ActorController : MonoBehaviour
{
	public ActorBase actor;
	public KeyCode curPressKey = KeyCode.None;
	private KeyCode prePressKey = KeyCode.None;
	private float prePressKeyTime;

	public KeyCode curUpKey = KeyCode.None;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		curPressKey = GetDownKey();
		curUpKey = GetUpKey();
		EmActorStatus actorStatus = actor.actorCurStatus;
		Vector3 moveDir;
		switch(actorStatus)
		{
		case EmActorStatus.Idle:
			//首先判断攻击
			if(DoAttack()) break;
			//后跳
			if(DoJumpBack()) break;
			//跳跃
			if(DoJump()) break;
			//格挡
			if(DoParry()) break;

			//左右移动控制
			if(curPressKey == KeyCode.LeftArrow || curPressKey == KeyCode.RightArrow || curPressKey == KeyCode.UpArrow || curPressKey == KeyCode.DownArrow)
			{
				//连续两次按同一个方向键就可以开跑，如果没跑，那就是走
				if(curPressKey == prePressKey && Time.realtimeSinceStartup - prePressKeyTime < 0.5f)
				{
					prePressKey = KeyCode.None;
					actor.Run(GetMoveDir());
				}
				else{
					actor.Walk(GetMoveDir());
				}
			}
			break;

		case EmActorStatus.Walk:
			//首先判断普通攻击
			if(DoAttack()) break;
			//后跳
			if(DoJumpBack()) break;
			//跳跃
			if(DoJump()) break;
			//格挡
			if(DoParry()) break;

			moveDir = GetMoveDir();
			if(moveDir.x != 0 || moveDir.z != 0)
				actor.Walk(moveDir);
			else
				actor.Idle();
			break;

		case EmActorStatus.Run:
			//首先判断普通攻击
			if(DoAttack()) return;
			//后跳
			if(DoJumpBack()) return;
			//跳跃
			if(DoJump()) return;
			//格挡
			if(DoParry()) break;

			moveDir = GetMoveDir();
			if(moveDir.x != 0 || moveDir.z != 0)
				actor.Run(moveDir);
			else
				actor.Idle();
			break;

		case EmActorStatus.DoAttack:
			DoAttack();
			break;

		case EmActorStatus.Parry:
			CancelParry();
			break;
		}

		if(curPressKey != KeyCode.None){
			prePressKey = curPressKey;
			prePressKeyTime = Time.realtimeSinceStartup;
		}
	}

	private bool DoParry()
	{
		if(curPressKey == KeyCode.F)
		{
			actor.DoParry(false);
			return true;
		}
		return false;
	}

	private void CancelParry()
	{
		if(curUpKey == KeyCode.F)
			actor.DoParry(true);
	}

	private bool DoAttack()
	{
		//连击：
		//三段斩的连击是方向键
		if(actor.GetCurSkillID() == (int)EmSkillIDSet.TripleSlash)
		{
			if(curPressKey == KeyCode.LeftArrow || curPressKey == KeyCode.RightArrow)
			{
				actor.DoAttack((int)EmSkillIDSet.TripleSlash);
				return true;
			}
			return false;
		}

		//判断组合
		if(Time.realtimeSinceStartup - prePressKeyTime < 0.1f)
		{
			if(prePressKey == KeyCode.LeftArrow || prePressKey == KeyCode.RightArrow)
			{
				if(curPressKey == KeyCode.A)
				{
					actor.DoAttack((int)EmSkillIDSet.TripleSlash);
					return true;
				}
			}
		}

		//判断单键
		if(curPressKey == KeyCode.S)
		{
			actor.DoAttack((int)EmSkillIDSet.NormalAttack);
			return true;
		}
		else if(curPressKey == KeyCode.R)
		{
			actor.DoAttack((int)EmSkillIDSet.GoreCross_Berserker);
			return true;
		}
		else if(curPressKey == KeyCode.A)
		{
			actor.DoAttack((int)EmSkillIDSet.UpStrike);
			return true;
		}
		else if(curPressKey == KeyCode.W)
		{
			actor.DoAttack((int)EmSkillIDSet.VaneSlash);
			return true;
		}
		else if(curPressKey == KeyCode.E)
		{
			actor.DoAttack((int)EmSkillIDSet.BlastBlood);
			return true;
		}
		else if(curPressKey == KeyCode.T)
		{
			actor.DoAttack((int)EmSkillIDSet.OutrageBreak);
			return true;
		}
		else if(curPressKey == KeyCode.Q)
		{
			actor.DoAttack((int)EmSkillIDSet.GrabBlast);
			return true;
		}
		else if(curPressKey == KeyCode.B)
		{
			//actor.DoAttack((int)EmSkillIDSet.HellBenter);
			return true;
		}
		else if(curPressKey == KeyCode.V)
		{
			//actor.DoAttack((int)EmSkillIDSet.GiveBlood);
			return true;
		}

		return false;
	}

	private bool DoJumpBack()
	{
		//if(curPressKey == KeyCode.D && prePressKey == KeyCode.DownArrow && Time.realtimeSinceStartup - prePressKeyTime < 0.5f)
		if(curPressKey == KeyCode.D && Input.GetKey(KeyCode.DownArrow))
		{
			actor.JumpBack();
			return true;
		}
		return false;
	}

	private bool DoJump()
	{
		if(curPressKey == KeyCode.D)
		{
			actor.Jump();
			return true;
		}
		return false;
	}

	private Vector3 GetMoveDir()
	{
		Vector3 dir = Vector3.zero;
		if(Input.GetKey(KeyCode.LeftArrow)){
			dir.x = -1;
		}
		else if(Input.GetKey(KeyCode.RightArrow)){
			dir.x = 1;
		}
		if(Input.GetKey(KeyCode.UpArrow)){
			dir.z = 1;
		}
		else if(Input.GetKey(KeyCode.DownArrow)){
			dir.z = -1;
		}
		return dir;
	}

	//可能按下的键，只处理这些键
	private static KeyCode[] possibleKeys = new KeyCode[]{
		KeyCode.A, KeyCode.B, KeyCode.C, KeyCode.D, KeyCode.E, KeyCode.F, KeyCode.G,
		KeyCode.H, KeyCode.I, KeyCode.J, KeyCode.K, KeyCode.L, KeyCode.M, KeyCode.N,
		KeyCode.O, KeyCode.P, KeyCode.Q, KeyCode.R, KeyCode.S, KeyCode.T,
		KeyCode.U, KeyCode.V, KeyCode.W, KeyCode.X, KeyCode.Y, KeyCode.Z,
		KeyCode.LeftArrow, KeyCode.RightArrow, KeyCode.UpArrow, KeyCode.DownArrow,
	};

	public static KeyCode GetDownKey()
	{
		for(int i=0; i<possibleKeys.Length; i++)
		{
			if(Input.GetKeyDown(possibleKeys[i]))
			   return possibleKeys[i];
		}
		return KeyCode.None;
	}

	public static KeyCode GetUpKey()
	{
		for(int i=0; i<possibleKeys.Length; i++)
		{
			if(Input.GetKeyUp(possibleKeys[i]))
				return possibleKeys[i];
		}
		return KeyCode.None;
	}

	public void OnEnterCollier(ObjectCollider target)
	{
		if(target.name.Contains("Gate"))
		{
			SceneManagerBattle battle = GameMain.curSceneManager as SceneManagerBattle;
			battle.OnPlayerEnterDoor();
		}
	}
}

