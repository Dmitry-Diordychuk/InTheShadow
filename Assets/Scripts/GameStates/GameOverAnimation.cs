using UnityEngine;

namespace InTheShadow
{
	public class GameOverAnimation : _GameState
	{
		// Movement speed in units per second.
		[SerializeField] private float lerpSpeed = 1.0F;
		
		// Time when the movement started.
		private float _lerpStartTime;

		// Total distance between the markers.
		private float _lerpLength;
		
		public override void InitState(GameManager manager)
		{
			base.InitState(manager);
			State = GameManager.GameState.GameOverAnimation;
		}

		private Vector3 _cameraStartPosition;
		private Vector3 _cameraEndGamePosition;
		private Quaternion _cameraStartRotation;
		private Quaternion _cameraEndRotation;
		
		public override void StartState()
		{
			_cameraStartPosition = gameManager.cameraController.gameObject.transform.position;
			Vector3 screenNormal = gameManager.shadowProjector.screen.GetNormal();
			_cameraEndGamePosition = gameManager.shadowProjector.screen.GetPosition() + screenNormal;
			
			_cameraStartRotation = gameManager.cameraController.transform.rotation;
			_cameraEndRotation = _cameraStartRotation * Quaternion.Euler(0.0f, -25.0f, 0.0f);
			
			_lerpStartTime = Time.time;
			_lerpLength = Vector3.Distance(_cameraStartPosition, _cameraEndGamePosition);
			gameManager.uiManager.SetShadowComparisonProgressInPercent(1.0f);
		}

		private float _fractionOfJourney = 0.0f;
		public override void UpdateState()
		{
			base.UpdateState();

			gameManager.cameraController.transform.position = Vector3.Lerp(_cameraStartPosition, _cameraEndGamePosition, _fractionOfJourney);
			gameManager.cameraController.transform.rotation = Quaternion.Slerp(_cameraStartRotation, _cameraEndRotation, _fractionOfJourney);
			
			gameManager.shadowProjector.shadowCasters.MakePerfectShadow(gameManager.successfulRotations, _fractionOfJourney);
			
			if (_fractionOfJourney >= 1.0f)
			{
				gameManager.SetActiveState(GameManager.GameState.GameOverShowMenu);
			}
			
			// Distance moved equals elapsed time times speed..
			float distCovered = (Time.time - _lerpStartTime) * lerpSpeed;

			// Fraction of journey completed equals current distance divided by total distance.
			_fractionOfJourney = distCovered / _lerpLength;
		}
	}
}