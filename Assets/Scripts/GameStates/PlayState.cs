using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace InTheShadow
{
	public class PlayState : _GameState
    {
        public override void InitState(GameManager gameManager)
		{
			base.InitState(gameManager);
            State = GameManager.GameState.Play;
        }
        
        private Renderer _snapshotQuadRend;
        private Vector3 _cameraPosition;
        private Vector3 _cameraUp;
        private Vector3 _shadowProjectionQuadNormal;

        public override void StartState()
        {
            Camera mainCamera = Camera.main;
            if (!mainCamera) throw new UnityException("Main camera doesn't exist");
            Transform cameraTransform = mainCamera.transform;

            _cameraPosition = cameraTransform.position;
            _cameraUp = cameraTransform.up;
            _shadowProjectionQuadNormal = gameManager.shadowProjectionQuad.GetComponent<MeshFilter>().mesh.normals[0];
            _snapshotQuadRend = gameManager.snapshotQuad.GetComponent<Renderer>();
        }

        private int _resultIndex;
		public override void StateUpdate()
		{
			InputProcessing();
            
			(gameManager.resultValue, gameManager.resultIndex) = CalculateCurrentResult();

            gameManager.uiManager.SetProgress(gameManager.resultValue);

            if (CheckGameOver(gameManager.resultValue))
            {
                gameManager.SetActiveState(GameManager.GameState.GameOverAnimation);
            }
		}
		
		private void InputProcessing()
        {
            if (gameManager.inputManager.IsLeftMouseDown)
            {
                if (gameManager.difficultyLevel == GameManager.DifficultyLevel.Hard && gameManager.inputManager.IsShiftKeyDown)
                {
                    gameManager.shadowCasterController.RotateAll(_shadowProjectionQuadNormal, 
                        -gameManager.inputManager.MousePositionDelta.y * Time.deltaTime * 10.0f);
                }
                else if (gameManager.difficultyLevel > GameManager.DifficultyLevel.Easy &&
                         gameManager.inputManager.IsCtrlKeyDown)
                {
                    gameManager.shadowCasterController.Rotate(_shadowProjectionQuadNormal, 
                        -gameManager.inputManager.MousePositionDelta.y * Time.deltaTime * 10.0f);
                }
                else
                {
                    gameManager.shadowCasterController.Rotate(_cameraUp,
                        -gameManager.inputManager.MousePositionDelta.x * Time.deltaTime * 10.0f);
                    if (gameManager.difficultyLevel >= GameManager.DifficultyLevel.Medium)
                    {
                        Vector3 cameraToShadowCaster = gameManager.shadowCasterController.gameObject.transform.position - _cameraPosition;
                        Vector3 cameraUpCrossCameraToShadowCaster = Vector3.Cross(_cameraUp, cameraToShadowCaster);
                        gameManager.shadowCasterController.Rotate(cameraUpCrossCameraToShadowCaster,
                            gameManager.inputManager.MousePositionDelta.y * Time.deltaTime * 10.0f);
                    }
                }
            }
        }

        private int _currentSnapshotOnQuad = -1;
        private (float, int) CalculateCurrentResult()
        {
            int bestResultIndex = -1;
            float bestResult = -1.0f;
            for (int i = 0; i < gameManager.successfulSnapshots.Count; i++)
            {
                float snapshotsComparisonResultPercent = ShadowSnapshotUtility.CompareSnapshots(
                    ShadowSnapshotUtility.GetShadowSnapshot(gameManager.renderTexture),
                    gameManager.successfulSnapshots[i]);
                
                if (snapshotsComparisonResultPercent >= bestResult)
                {
                    bestResult = snapshotsComparisonResultPercent;
                    bestResultIndex = i;
                }
            }
            
            // Debug visualisation
            if (_currentSnapshotOnQuad != bestResultIndex)
            {
                _snapshotQuadRend.material.mainTexture = gameManager.successfulSnapshots[bestResultIndex];
                _currentSnapshotOnQuad = bestResultIndex;
            }

            return (bestResult, bestResultIndex);
        }
        
        private bool _isTimerActive = false;
        private float _startTimerTime;
        private float _timer;
        private bool CheckGameOver(float currentResult)
        {
            if (currentResult > 0.97)
            {
                if (!_isTimerActive)
                {
                    _isTimerActive = true;
                    _startTimerTime = Time.time;
                }

                _timer = Time.time - _startTimerTime;
                if (_timer > 3.0f)
                {
                    return true;
                }
            }
            else
            {
                _isTimerActive = false;
            }

            return false;
        }
	}
}