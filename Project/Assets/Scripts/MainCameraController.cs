using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainCameraController : MonoBehaviour 
{
	public Camera myCamera;
	public Transform flowWho;

	void Awake()
	{
		GameMain.mainCameraController = this;
	}

	// Use this for initialization
	void Start ()
 	{
	}

	// Update is called once per frame
	void Update ()
	{
		if(GameMain.curStatus == GameStatus.None || GameMain.curStatus == GameStatus.Loading) return;
		SetCameraPos(flowWho.position);
	}

	void SetCameraPos(Vector3 viewPos)
	{
		Vector2 halfMapSize = SceneInfo.instance.mapSize*0.5f;
		Vector2 halfViewSize = Vector2.zero;
		halfViewSize.y = myCamera.orthographicSize;
		halfViewSize.x = (float)Screen.width/Screen.height*halfViewSize.y;

		//水平
		if(viewPos.x - halfViewSize.x <= -halfMapSize.x)
			viewPos.x = -halfMapSize.x + halfViewSize.x;
		else if(viewPos.x + halfViewSize.x >= halfMapSize.x)
			viewPos.x = halfMapSize.x - halfViewSize.x;
		//竖直
		if(viewPos.y - halfViewSize.y <= -halfMapSize.y)
			viewPos.y = -halfMapSize.y + halfViewSize.y;
		if(viewPos.y + halfViewSize.y >= halfMapSize.y)
			viewPos.y = halfMapSize.y - halfViewSize.y;

		viewPos.z = myCamera.transform.position.z;
		myCamera.transform.position = viewPos;
	}
	bool IsCameraInMap(Vector3 viewPos)
	{
		Vector2 halfMapSize = SceneInfo.instance.mapSize*0.5f;
		Vector2 halfViewSize = Vector2.zero;
		halfViewSize.y = myCamera.orthographicSize;
		halfViewSize.x = (float)Screen.width/Screen.height*halfViewSize.y;

		//水平是否超出
		if(viewPos.x - halfViewSize.x <= -halfMapSize.x || viewPos.x + halfViewSize.x >= halfMapSize.x)
			return true;
		//垂直是否超出
		if(viewPos.y - halfViewSize.y <= -halfMapSize.y || viewPos.y + halfViewSize.y >= halfMapSize.y)
			return true;

		return false;
	}
}
