using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Looks at the main camera, like a billboard image
/// </summary>
public class LookAtCamera : MonoBehaviour
{
	public Transform Target;

	// Use this for initialization
	void Start ()
	{
		if(Target == null) Target = Camera.main.transform;
	}

	// Update is called once per frame
	void Update ()
	{
		transform.LookAt(Target.transform);
	}
}
