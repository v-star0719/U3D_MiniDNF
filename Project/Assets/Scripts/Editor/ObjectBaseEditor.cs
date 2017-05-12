using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(ObjectBase), true)]
public class ObjectBaseEditor : Editor 
{
	public override void OnInspectorGUI()
	{
		ObjectBase ob = target as ObjectBase;
		if(!ob.editMode)
		{
			//脚本是激活的，说明进行编辑transform，可以修改2d坐标
			//ob.realPos = EditorGUILayout.Vector3Field("realPos", ob.realPos);
			ob.realLocalPos = EditorGUILayout.Vector3Field("realLocalPos", ob.realLocalPos);
			ob.renderZOffset = EditorGUILayout.FloatField("renderZOffset", ob.renderZOffset);
			ob.isFacingRight = EditorGUILayout.Toggle("isFacingRight", ob.isFacingRight);
		}
		else
		{
			EditorGUILayout.Vector3Field("realLocalPos", ob.realLocalPos);
			ob.isFacingRight = EditorGUILayout.Toggle("isFacingRight", ob.isFacingRight);
			EditorGUILayout.ObjectField(ob.mirror, typeof(ThreeeDMirror), true);
			base.OnInspectorGUI();
		}
	}

	private Vector3 DrawVector3(string name, float nameWidth, Vector3 v)
	{
		GUILayout.BeginHorizontal();
		GUILayout.Label(name, GUILayout.Width(nameWidth));
		GUILayout.Label("x", GUILayout.Width(10));
		v.x = EditorGUILayout.FloatField(v.x, GUILayout.MinWidth(40));
		GUILayout.Label("y", GUILayout.Width(10));
		v.y = EditorGUILayout.FloatField(v.y, GUILayout.MinWidth(40));
		GUILayout.Label("z", GUILayout.Width(10));
		v.z = EditorGUILayout.FloatField(v.z, GUILayout.MinWidth(40));
		GUILayout.EndHorizontal();
		return v;
	}
}
