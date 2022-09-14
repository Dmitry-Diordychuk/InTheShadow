using UnityEngine;

namespace InTheShadow
{
	public class GameOverAnimation : _GameState
	{
		public override void InitState(GameManager gameManager)
		{
			base.InitState(gameManager);
			State = GameManager.GameState.GameOverAnimation;
		}

		private Vector3 _cameraStartPosition;
		private Vector3 _cameraEndGamePosition;
		private Quaternion _cameraRotation;
		private Quaternion _cameraEndRotation;
		
		public override void StartState()
		{
			_cameraStartPosition = gameManager.cameraController.gameObject.transform.position;
			Vector3 shadowProjectionQuadNormal = gameManager.shadowProjectionQuad.GetComponent<MeshFilter>().mesh.normals[0];
			_cameraEndGamePosition = gameManager.shadowProjectionQuad.transform.position + shadowProjectionQuadNormal;
			
			_cameraRotation = gameManager.cameraController.transform.rotation;
			_cameraEndRotation = _cameraRotation * Quaternion.Euler(0.0f, -25.0f, 0.0f);
			
			gameManager.cameraController.InitLerp(_cameraStartPosition, _cameraEndGamePosition);
			gameManager.uiManager.SetProgress(1.0f);
		}
		
		public override void StateUpdate()
		{
			float t = gameManager.cameraController.FocusOnEndPosition(
				_cameraStartPosition, _cameraEndGamePosition,
				_cameraRotation, _cameraEndRotation);
			gameManager.shadowCasterController.RotateToSnapshotShadow(gameManager.shadowCasterController.gameObject.transform.rotation,
				gameManager.successfulRotations[gameManager.resultIndex], t);
			if (t >= 1.0f)
			{
				gameManager.SetActiveState(GameManager.GameState.GameOverShowMenu);
			}
		}
	}
}