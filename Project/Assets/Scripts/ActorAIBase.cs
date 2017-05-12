using UnityEngine;
using System.Collections;

public class ActorAIBase : MonoBehaviour
{
	public ActorBase actor;
	// Use this for initialization
	void Start () {
	}

	public bool doParry = false;
	// Update is called once per frame
	void Update ()
	{
		if(doParry)
		{
			actor.DoParry(false);
			doParry = false;
		}
	}
}
