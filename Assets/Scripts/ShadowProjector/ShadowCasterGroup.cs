using System.Collections.Generic;
using UnityEngine;

namespace InTheShadow
{
	public class ShadowCasterGroup : MonoBehaviour
	{
		[SerializeField] private List<ShadowCaster> shadowCasters;

		private ShadowCaster _currentShadowCaster;

		private void Start()
		{
			if (shadowCasters.Count == 0)
			{
				Debug.LogWarning("There is no shadow casters!", this);
			}
			else if (shadowCasters.Count == 1)
			{
				SetSelectedShadowCaster(shadowCasters[0]);
			}
			else
			{
				foreach (var caster in shadowCasters)
				{
					caster.selectEvent.AddListener(SetSelectedShadowCaster);
				}
			}
		}
		
		public ShadowCaster GetSelectedShadowCaster()
		{
			return _currentShadowCaster;
		}

		private void SetSelectedShadowCaster(ShadowCaster shadowCaster)
		{
			if (_currentShadowCaster)
			{
				_currentShadowCaster.shadowCasterOutline.TurnOff();
			}

			_currentShadowCaster = shadowCaster;
			_currentShadowCaster.shadowCasterOutline.TurnOn();
		}

		private List<Quaternion> _startRotations = new List<Quaternion>();
		public void MakePerfectShadow(List<Quaternion> rotations, float t)
		{
			if (t == 0.0f)
			{
				_startRotations.Add(transform.rotation);
				foreach (var caster in shadowCasters)
				{
					_startRotations.Add(caster.transform.rotation);
				}
			}
			
			transform.rotation = Quaternion.Slerp(_startRotations[0], rotations[0], t);
			for (int i = 1; i < rotations.Count; i++)
			{
				shadowCasters[i - 1].transform.rotation = Quaternion.Slerp(_startRotations[i], rotations[i], t);
			}
		}

		public List<Quaternion> GetAllRotations()
		{
			List<Quaternion> rotations = new List<Quaternion>();
			rotations.Add(gameObject.transform.rotation);
			foreach (var caster in shadowCasters)
			{
                rotations.Add(caster.gameObject.transform.rotation);
			}

			return rotations;
		}
	}
}
