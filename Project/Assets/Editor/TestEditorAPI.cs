using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.AnimatedValues;

public class TestEditorAPI : EditorWindow 
{
	//[MenuItem("Test/EditorAPI")]
		static void Init() {
			var window = GetWindow(typeof(TestEditorAPI));
			window.Show();
		}

	void OnGUI()
	{
		if(GUILayout.Button("xxxx"))
		{
			EditorGUIUtility.DrawColorSwatch(new Rect(0, 0, 500, 500), Color.red);
		}
	}


	//[MenuItem("Examples/Ping Selected")]
	static void Ping() {
		if(!Selection.activeObject) {
			Debug.LogError("Select an object to ping");
			return;
		}
		EditorGUIUtility.PingObject(Selection.activeObject);
	}
}
