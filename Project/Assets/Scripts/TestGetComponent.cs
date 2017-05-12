using UnityEngine;
using System.Collections;

public class TestGetComponent : MonoBehaviour
{

	// Use this for initialization
	void Start () {
		myTrans = transform;
		myGo = gameObject;
	}

	Transform myTrans;
	GameObject myGo;

	public bool testGetComponet = false;
	// Update is called once per frame
	void Update ()
	{
		long times = 10000*100;
		int iteration = 5;
		float sum = 0;
		if(testGetComponet)
		{
			Debug.Log("1=======================================================");
			float startTime = 0;
			bool b = false;
			for(int j=0; j<iteration; j++)
			{
				startTime = Time.realtimeSinceStartup;
				for(int i = 0; i < times; i++)
				{
					//transform.localPosition = Vector3.zero;
					b = gameObject.activeSelf;
				}
				Debug.LogFormat("第{0}次用时：{1:f5}", j, Time.realtimeSinceStartup - startTime);
				sum += Time.realtimeSinceStartup - startTime;
			}
			Debug.LogFormat("平均：{0:f5}", sum/iteration);

			Debug.Log("2=======================================================");

			sum = 0;
			for(int j = 0; j < iteration; j++)
			{
				startTime = Time.realtimeSinceStartup;
				for(int i = 0; i < times; i++)
				{
					//myTrans.localPosition = Vector3.zero;
					b = myGo.activeSelf;
				}
				Debug.LogFormat("第{0}次用时：{1:f5}", j, Time.realtimeSinceStartup - startTime);
				sum += Time.realtimeSinceStartup - startTime;
			}
			Debug.LogFormat("平均：{0:f5}", sum / iteration);

			testGetComponet = false;
			Debug.Log(b);
		}
	}

	public GameObject AvoidWarning()
	{
		if(myTrans == transform) return myTrans.gameObject;
		else return myGo;
	}
}
