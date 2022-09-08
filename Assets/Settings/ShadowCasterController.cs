using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShadowCasterController : MonoBehaviour
{
	public void RotateY(float value)
	{
		gameObject.transform.Rotate(Vector3.up, value);
	}
}
