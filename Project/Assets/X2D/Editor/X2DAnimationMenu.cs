using UnityEngine;
using UnityEditor;
using System.Collections;

public class X2DAnimationMenu
{
	[MenuItem("X2D/CreateClip")]
	public static void CreateClip()
	{
		Debug.Log(Selection.activeGameObject);
	}
}
