using UnityEngine;
using System.Collections;

[RequireComponent(typeof(UISprite))]
public class UIIconCtrl : MonoBehaviour
{
	private UISprite _sprite;
	public UISprite sprite
	{
		get
		{
			if(_sprite == null)
				_sprite = GetComponent<UISprite>();
			return _sprite;
		}
	}

	public void Set(int id)
	{
		DBIconConf iconConf = DBIconTable.GetRecord(id);
		if(iconConf == null) return;
		sprite.spriteName = iconConf.spriteName;
	}
}
