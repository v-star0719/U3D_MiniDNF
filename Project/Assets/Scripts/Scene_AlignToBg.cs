using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class Scene_AlignToBg : MonoBehaviour
{
	public enum EmMode
	{
		None = 0,
		FarGround = 1,//[700,650)
		Ground = 2,//[650,600)
		GroundPlants = 3,//[600,550)
		Skill = 4,//[550,500)
	}

	public const float Z_FAR_GROUND1 = 700f;
	public const float Z_FAR_GROUND2 = 650f;
	public const float Z_GROUND1 = 650f;
	public const float Z_GROUND2 = 600f;
	public const float Z_GROUND_PLANTS1 = 600f;
	public const float Z_GROUND_PLANTS2 = 550;
	public const float Z_SKILL1 = 550;
	public const float Z_SKILL2 = 500;

	public EmMode alignMode;
	public float z;
	public bool runInPlaying;

	// Use this for initialization
	void Start () {
	
	}

//#if UNITY_EDITOR
	// Update is called once per frame
	void Update ()
	{
		if(!runInPlaying) return;
		Vector3 v = transform.position;
		float z1 = 0, z2 = 0;
		switch(alignMode)
		{
		case EmMode.FarGround: 		z1 = Z_FAR_GROUND1; 	z2 = Z_FAR_GROUND2; 	break;
		case EmMode.Ground: 		z1 = Z_GROUND1; 		z2 = Z_GROUND2;			break;
		case EmMode.GroundPlants: 	z1 = Z_GROUND_PLANTS1; 	z2 = Z_GROUND_PLANTS2;	break;
		case EmMode.Skill: 			z1 = Z_SKILL1;			z2 = Z_SKILL2; 			break;
		}
		z += z2;
		if(z > z1) z = z1;
		if(z < z2) z = z2;
		v.z = z;
		z = z - z2;
		transform.position = v;
	}
//#endif
}
