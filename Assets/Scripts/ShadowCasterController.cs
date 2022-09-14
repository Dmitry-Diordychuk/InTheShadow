using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShadowCasterController : MonoBehaviour
{
	public void Rotate(Vector3 axis, float value)
	{
		transform.Rotate(axis, value, Space.World);
	}

	public void RotateToSnapshotShadow(Quaternion startRotation, Quaternion endRotation, float t)
	{
		transform.rotation = Quaternion.Slerp(startRotation, endRotation, t);
	}
}
