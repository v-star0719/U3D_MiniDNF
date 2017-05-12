using UnityEngine;
using System.Collections;

public class SceneLevelGate : MonoBehaviour
{
	public X2DSprite closeMark;
	public X2DSprite openMark;

	private bool isOpen = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	float timer = 0f;
	void Update ()
	{
		if(isOpen)
		{
			timer += Time.deltaTime;
			openMark.alpha = Mathf.Abs(timer);
			if(timer >= 1f)
				timer = -1f;
		}
	}

	public void OpenOrClose(bool open)
	{
		isOpen = open;
		closeMark.gameObject.SetActive(!open);
		openMark.gameObject.SetActive(open);
	}
}
