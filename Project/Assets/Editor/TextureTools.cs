using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class TqmTextureImporterSettings
{
	public TextureImporterFormat textureFormat;
	public TextureImporterType textureType;
	public TextureImporterNPOTScale npotScale;
	public bool mipmapEnabled;
	public TextureWrapMode wrapMode;
}

public class TextureTools 
{
	[MenuItem("TextureTools/Set ARGB32")]
	public static void SetARGB32()
	{
		Object[] textures = Selection.GetFiltered(typeof(Texture2D), SelectionMode.DeepAssets);
		if(textures.Length == 0)
		{
			Debug.Log("没有找到2D纹理");
			return;
		}

		Debug.LogFormat("找到{0}张纹理", textures.Length);

		TqmTextureImporterSettings settings = new TqmTextureImporterSettings();
		settings.textureFormat = TextureImporterFormat.ARGB32;
		settings.textureType = TextureImporterType.Advanced;
		settings.npotScale = TextureImporterNPOTScale.None;
		settings.mipmapEnabled = false;
		settings.wrapMode = TextureWrapMode.Clamp;

		EditorUtility.DisplayProgressBar("努力处理中...", "0/"+textures.Length, 0);
		long curTime = System.DateTime.Now.ToFileTime();
		for(int i=0; i<textures.Length; i++)
		{
			//一秒刷一次
			if(System.DateTime.Now.ToFileTime() - curTime > 1000 * 10000)
			{
				EditorUtility.DisplayProgressBar("努力处理中...", i + "/" + textures.Length, (float)i / textures.Length);
				curTime = System.DateTime.Now.ToFileTime();
			}
			SetTexture(textures[i], settings);
		}

		EditorUtility.ClearProgressBar();
		Debug.LogFormat("处理完成");
	}

	private static void SetTexture(Object texture, TqmTextureImporterSettings settings)
	{
		TextureImporter ti = TextureImporter.GetAtPath(AssetDatabase.GetAssetPath(texture)) as TextureImporter;
		ti.textureFormat = settings.textureFormat;
		ti.textureType = settings.textureType;
		ti.npotScale = settings.npotScale;
		ti.mipmapEnabled = settings.mipmapEnabled;
		ti.wrapMode = settings.wrapMode;
		ti.SaveAndReimport();
	}
}
