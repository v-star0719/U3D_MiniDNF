using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class X2DSprite : MonoBehaviour 
{
	public enum EmHorzPivot
	{
		Left,
		Center,
		Right,
	}
	public enum EmVertPivot
	{
		Top,
		Center, 
		Bottom,
	}

	public float width = 1;
	public float height = 1;
	public Texture2D textrue;
	public Shader shader;
	public EmHorzPivot horzPivot = EmHorzPivot.Center;
	public EmVertPivot vertPivot = EmVertPivot.Center;

	private MeshRenderer _myRenderer;
	private MeshRenderer myRenderer
	{
		get
		{
			if(_myRenderer != null) return _myRenderer;
			if(_myRenderer == null) _myRenderer = GetComponent<MeshRenderer>();
			if(_myRenderer == null) _myRenderer = gameObject.AddComponent<MeshRenderer>();
			return _myRenderer;
		}
	}

	private MeshFilter _filter;
	private MeshFilter filter
	{
		get
		{
			if(_filter != null) return _filter;
			if(_filter == null) _filter = GetComponent<MeshFilter>();
			if(_filter == null) _filter = gameObject.AddComponent<MeshFilter>();
			return _filter;
		}
	}

	private Mesh _mesh;
	private Mesh mesh
	{
		get
		{
			if(_mesh != null) return _mesh;

			_mesh = new Mesh();
			Vector3[] vertices = new Vector3[4];
			Vector2[] uv = new Vector2[4];

			uv[0] = new Vector2(0, 0);
			uv[1] = new Vector2(0, 1);
			uv[2] = new Vector2(1, 1);
			uv[3] = new Vector2(1, 0);

			FillMeshVertices(vertices);

			int[] indices = new int[6];
			indices[0] = 0;
			indices[1] = 1;
			indices[2] = 2;
			//
			indices[3] = 0;
			indices[4] = 2;
			indices[5] = 3;

			_mesh.vertices = vertices;
			_mesh.uv = uv;
			_mesh.triangles = indices;

			return _mesh;
		}
	}
	private void FillMeshVertices(Vector3[] vertices)
	{
		float halfW = width * 0.5f;
		float halfH = height * 0.5f;
		Vector3 pivotOffset = Vector3.zero;
		if(horzPivot == EmHorzPivot.Left) pivotOffset.x = halfW;
		else if(horzPivot == EmHorzPivot.Right) pivotOffset.x = -halfW;
		if(vertPivot == EmVertPivot.Top) pivotOffset.y = -halfH;
		else if(vertPivot == EmVertPivot.Bottom) pivotOffset.y = halfH;
		//
		vertices[0] = new Vector3(-halfW, -halfH, 0) + pivotOffset;
		vertices[1] = new Vector3(-halfW, halfH, 0) + pivotOffset;
		vertices[2] = new Vector3(halfW, halfH, 0) + pivotOffset;
		vertices[3] = new Vector3(halfW, -halfH, 0) + pivotOffset;
	}

	private Material _myMaterial;
	private Material myMaterial
	{
		get
		{
			if(_myMaterial == null && shader != null)
			{
				_myMaterial = new Material(shader);
				_myMaterial.hideFlags = HideFlags.DontSave;
				_myMaterial.mainTexture = textrue;
				myRenderer.material = _myMaterial;
			}
			return _myMaterial;
		}
	}

	private float _alpha = 1f;
	public float alpha
	{
		get{return _alpha;}
		set{
			_alpha = value;
			myMaterial.SetFloat("_Alpha", _alpha);
		}
	}

	// Use this for initialization
	void Start ()
 	{
		myRenderer.material = myMaterial;
		filter.mesh = mesh;
	}

	// Update is called once per frame
	void Update ()
	{
		#if UNITY_EDITOR
		UpdateEditor();
		#endif
	}

	///shader改变
	///材质改变
	///size改变
	void UpdateEditor()
	{
		//检查renderer
		if(myRenderer == null) return;
		//检查filter
		if(filter == null) return;

		//mesh刷新
		if(mesh != null)
		{
			Vector3[] vertices = mesh.vertices;
			FillMeshVertices(vertices);
			mesh.vertices = vertices;
			filter.mesh = mesh;
		}

		//检查材质
		if(myMaterial == null) return;

		myRenderer.material = myMaterial;

		//shader刷新
		if(myMaterial.shader.name != shader.name)
		{
			myMaterial.shader = shader;
		}

		//纹理刷新
		if(myMaterial.mainTexture != textrue)
			myMaterial.mainTexture = textrue;
	}
}
