using UnityEngine;
using System.Collections;

public class ResourceLoader
{
	public const string x2dAnimationFolder = "Prefabs/X2DAnimations/";
	public const string x2DEffectsFolder = "Prefabs/X2DEffects/";
	public const string charactorSoundFolder = "Sounds/swordman/";
	public const string bgmSoundFolder = "Sounds/Bgm/";
	public const string sceneFolder = "Prefabs/Scenes/";

	public static X2DAnimation GetX2dAniamtion(int id, Transform parent)
	{
		return null;
	}
	
	public static X2DAnimation GetX2dAniamtion(string animationName, Transform parent)
	{
		X2DAnimation prefab = Resources.Load<X2DAnimation>(x2dAnimationFolder + animationName);
		if(prefab == null)
			Debug.LogError("X2DAnimation is not exist: " + x2dAnimationFolder + animationName);
		else
			return CreateNewX2dAnimation(prefab, parent);
		return null;
	}
	
	private static X2DAnimation CreateNewX2dAnimation(X2DAnimation org, Transform parent)
	{
		X2DAnimation anim = GameObject.Instantiate<X2DAnimation>(org);
		anim.transform.parent = parent;
		anim.transform.localPosition = Vector3.zero;
		anim.transform.localRotation = Quaternion.identity;
		anim.gameObject.SetActive(true);
		return anim;
	}

	public static X2DEffect GetX2DEffect(string animationName, Transform parent)
	{
		X2DEffect prefab = Resources.Load<X2DEffect>(x2DEffectsFolder + animationName);
		if(prefab == null)
			Debug.LogError("X2DEffect is not exist: " + x2DEffectsFolder + animationName);
		else
			return CreateNewX2DEffect(prefab, parent);
		return null;
	}
	
	private static X2DEffect CreateNewX2DEffect(X2DEffect org, Transform parent)
	{
		X2DEffect effect = GameObject.Instantiate<X2DEffect>(org);
		effect.transform.parent = parent;
		effect.transform.localPosition = Vector3.zero;
		effect.transform.localRotation= Quaternion.identity;
		effect.gameObject.SetActive(true);
		return effect;
	}

	public static AudioClip LoadSound(string soundPathName)
	{
		AudioClip clip = Resources.Load<AudioClip>(soundPathName);
		if(clip == null){
			Debug.LogError("sound is not exist: " + soundPathName);
        }
		return clip;
    }
	public static AudioClip LoadCharactorSound(string soundName)
	{
		return LoadSound(charactorSoundFolder + soundName);
    }
	public static AudioClip LoadBgmSound(string soundName)
	{
		return LoadSound(bgmSoundFolder + soundName);
	}

	public static GameObject LoadScene(string sceneName)
	{
		GameObject scene = Resources.Load<GameObject>(sceneFolder + sceneName);
		if(scene == null)
		{
			Debug.LogError("scene is not exist: " + sceneFolder + sceneName);
			return null;
		}

		GameObject go = GameObject.Instantiate(scene);
		go.SetActive(true);
		go.transform.localPosition = Vector3.zero;
		go.transform.localScale = Vector3.one;
		go.transform.localRotation = Quaternion.identity;
		return go;
	}

}
