using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class FollowOthers : MonoBehaviour 
{
	public Vector3 offsetInWorld;
	public Transform followWho;

	// Use this for initialization
	void Start ()
 	{
	}

	// Update is called once per frame
	void Update ()
	{
		if(followWho != null)
		{
			transform.position = followWho.position + offsetInWorld;
			transform.localRotation = followWho.localRotation;
		}
	}
}
