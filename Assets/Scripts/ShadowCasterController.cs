using System;
using System.Collections;
using System.Collections.Generic;
using InTheShadow;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ShadowCasterController : MonoBehaviour
{
	[SerializeField] private GameManager gameManager;

	public class SelectEvent : UnityEvent<GameObject> { }
	public SelectEvent selectEvent = new SelectEvent();
	
	private GameObject _currentShadowCaster;

	private void Start()
	{
		if (gameManager.difficultyLevel < GameManager.DifficultyLevel.Hard)
		{
			_currentShadowCaster = gameObject;
		}
		else
		{
			_currentShadowCaster = transform.GetChild(0).gameObject;
			selectEvent.AddListener(SetCurrentShadowCaster);
		}
	}

	private void SetCurrentShadowCaster(GameObject shadowCaster)
	{
		if (_currentShadowCaster) _currentShadowCaster.layer = LayerMask.NameToLayer("Default");
		_currentShadowCaster = shadowCaster;
		_currentShadowCaster.layer = LayerMask.NameToLayer("Outline");
	}

	public void Rotate(Vector3 axis, float value)
	{
		_currentShadowCaster.transform.Rotate(axis, value, Space.World);
	}

	public void RotateAll(Vector3 axis, float value)
	{
		transform.Rotate(axis, value, Space.World);
	}

	public void RotateToPerfect(Quaternion startRotation, Quaternion endRotation, float t)
	{
		transform.rotation = Quaternion.Slerp(startRotation, endRotation, t);
	}
}
