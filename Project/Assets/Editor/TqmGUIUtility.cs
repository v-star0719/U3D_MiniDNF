using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class TqmGUILayoutUtility
{
	public static void ColorLable(Color clr, string text, params GUILayoutOption[] options)
	{
		Color old = GUI.color;
		GUI.color = clr;
		GUILayout.Label(text, options);
		GUI.color = old;
	}

	public static Vector2 SimpleVector2(Vector2 v, float numberWidth)
	{
		GUILayout.BeginHorizontal();
		Vector2 ret;
		GUILayout.Label("x", GUILayout.Width(10));
		ret.x = EditorGUILayout.FloatField("", v.x, GUILayout.Width(numberWidth));
		GUILayout.Label("y", GUILayout.Width(10));
		ret.y = EditorGUILayout.FloatField("", v.y, GUILayout.Width(numberWidth));
		GUILayout.EndHorizontal();
		return ret;
	}
}
