using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class X2DTools 
{
	public static Mesh posMarkMesh;
	public static Material posMarkMaterial;
	public static void DrawPosMark(GameObject go, Vector3 pos, int layer)
	{
		#if UNITY_EDITOR
		if(go.transform.parent != null && go.transform.parent.GetComponentInParent<X2DEffect>() != null) return;
		if(posMarkMesh == null)
			CreatePosMarkMesh();

		Graphics.DrawMesh(posMarkMesh, pos, Quaternion.identity, null, layer);
		#endif
	}

	//todo: 画线还是得用Mesh，还有就是effect下面有很多的animation，这些也会画坐标轴
	public static void CreatePosMarkMesh()
	{
		if(posMarkMesh != null) return;

		posMarkMesh = new Mesh();

		//=====================================
		Vector2[] uv = new Vector2[8];
		//横
		uv[0] = new Vector2(0, 0);
		uv[1] = new Vector2(0, 1);
		uv[2] = new Vector2(1, 1);
		uv[3] = new Vector2(1, 0);
		//竖
		uv[4] = new Vector2(0, 0);
		uv[5] = new Vector2(0, 1);
		uv[6] = new Vector2(1, 1);
		uv[7] = new Vector2(1, 0);

		//=====================================
		Vector3[] vertices = new Vector3[8];
		float height = 130;
		float width = 100;
		//横
		float halfW = width*0.5f;
		vertices[0] = new Vector3(-halfW, -1, 0);
		vertices[1] = new Vector3(-halfW, 1, 0);
		vertices[2] = new Vector3(halfW, 1, 0);
		vertices[3] = new Vector3(halfW, -1, 0);
		//竖
		vertices[4] = new Vector3(-1, 0, 0);
		vertices[5] = new Vector3(-1, height, 0);
		vertices[6] = new Vector3(1, height, 0);
		vertices[7] = new Vector3(1, 0, 0);

		//=====================================
		int[] indices = new int[12];
		//横
		indices[0] = 0;
		indices[1] = 1;
		indices[2] = 2;
		//
		indices[3] = 0;
		indices[4] = 2;
		indices[5] = 3;
		//竖
		indices[6] = 4;
		indices[7] = 5;
		indices[8] = 6;
		//
		indices[9] = 4;
		indices[10] = 6;
		indices[11] = 7;

		posMarkMesh.vertices = vertices;
		posMarkMesh.uv = uv;
		posMarkMesh.triangles = indices;
		posMarkMesh.hideFlags = HideFlags.DontSave;

		posMarkMaterial = new Material(Shader.Find(X2DAnimationClip.DEFAULT_SHADER_NAME));
		posMarkMaterial.hideFlags = HideFlags.DontSave;
	}
}
