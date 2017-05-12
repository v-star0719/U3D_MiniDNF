using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum EmMonsterType
{
	Normal,
	Elite,
	Boss,
}

[ExecuteInEditMode]
public class EnemyMark : MonoBehaviour 
{
	public int enemyId;
	public int enemyLv = 1;
	public string enemyNameForDebug;
	public EmMonsterType monsterType;
	public float heightToGround;

	public bool enterEditorMode;

	//为了能够在场景视图里拖动，已编辑位置，将ObjectBase禁用
	//要求输入Y值，自动将坐标值转换为RealPos
	public void Update()
	{
		if(Application.isPlaying) return;

		ObjectBase ob = GetComponent<ObjectBase>();
		if(ob == null)
			return;

		if(enterEditorMode)
		{
			Vector3 pos = transform.position;
			pos.y = pos.z * 0.7071f + heightToGround;
			transform.position = pos;

			ob.editMode = true;
			ob.ApplyransformPos();
		}
		else
		{
			ob.editMode = false;
		}

		
	}
}
