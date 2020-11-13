﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;
using System.Collections;
using System.Collections.Generic;


// by arclee
public class NewBehaviourScript : MonoBehaviour
{

	public List<string> pathname = new List<string>();
	// Use this for initialization

	void tweenstart()
	{
		iTweenEvent it = (iTweenEvent)gameObject.GetComponent<iTweenEvent>();

		foreach (iTweenPath itp in it.paths)
		{
			Debug.Log(itp.pathName);

		}
		Debug.Log("tween tweenstart!!");
	}

	void tweenfinish()
	{
		iTweenEvent it = (iTweenEvent)gameObject.GetComponent<iTweenEvent>();

		foreach (iTweenPath itp in it.paths)
		{
			Debug.Log(itp.pathName);

		}

		//跑完換路徑.
		if (it.Values.ContainsKey("path"))
		{
			if (it.Values["path"].GetType() == typeof(string))
			{
				it.Values["path"] = "New Path 1";
			}
		}

		it.Play();
		Debug.Log("tween finish!!");
	}
}