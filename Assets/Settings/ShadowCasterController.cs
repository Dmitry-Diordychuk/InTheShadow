using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShadowCasterController : MonoBehaviour
{
	public void Rotate(Vector3 axis, float value)
	{
		gameObject.transform.Rotate(axis, value, Space.World);
	}
}
