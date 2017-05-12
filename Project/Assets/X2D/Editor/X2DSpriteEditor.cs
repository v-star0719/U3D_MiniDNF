using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(X2DSprite))]
public class X2DSpriteEditor : Editor
{
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI ();
		X2DSprite sprite = target as X2DSprite;
		if(GUILayout.Button("orgTexSize", GUILayout.Width(120)))
		{
			if(sprite.textrue != null)
			{
				sprite.width = sprite.textrue.width;
				sprite.height = sprite.textrue.height;
			}
			else
				Debug.LogError("未设置文理");
		}
	}
}
