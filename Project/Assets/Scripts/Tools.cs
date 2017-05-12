using UnityEngine;
using System.Collections;

public class Tools
{
	public static Mesh GetQuad()
	{
		Mesh mesh = new Mesh();
		Vector3[] vertices = new Vector3[4];
		vertices[0] = new Vector3(-0.5f,  0.5f, 0);
		vertices[1] = new Vector3( 0.5f,  0.5f, 0);
		vertices[2] = new Vector3( 0.5f, -0.5f, 0);
		vertices[3] = new Vector3(-0.5f, -0.5f, 0);
		mesh.vertices = vertices;
		int[] indices = new int[6]{
			0,1,2,
			3,0,2
		};
		mesh.triangles = indices;
		Vector2[] uvs = new Vector2[4];
		uvs[0] = new Vector2(0, 1);
		uvs[1] = new Vector2(1, 1);
		uvs[2] = new Vector2(1, 0);
		uvs[3] = new Vector2(0, 0);
		mesh.uv = uvs;
		return mesh;
	}
}
