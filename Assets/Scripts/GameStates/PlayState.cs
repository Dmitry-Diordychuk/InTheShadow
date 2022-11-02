using UnityEngine;

namespace InTheShadow.GameStates
{
    public class PlayState : _GameState
    {
        [Range(0.0f, 1.0f)]
        [SerializeField] private float shadowPrecisionForWin;

        public override void InitState(GameManager manager)
		{
            State = GameManager.GameState.Play;
            base.InitState(manager);
        }

        private Vector3 _cameraPosition;
        private Vector3 _cameraUp;
        private Vector3 _projectorScreenNormal;

        public override void StartState()
        {
            Transform cameraTransform = gameManager.cameraController.gameObject.transform;

            _cameraPosition = cameraTransform.position;
            _cameraUp = cameraTransform.up;
            _projectorScreenNormal = gameManager.shadowProjector.screen.GetNormal();
        }

        private int _resultIndex;
		public override void UpdateState()
		{
            base.UpdateState();
            
            InputProcessing();

            (gameManager.resultValue, gameManager.resultIndex) = CalculateCurrentResult();

            gameManager.uiManager.SetShadowComparisonProgressInPercent(gameManager.resultValue);

            if (CheckGameOver(gameManager.resultValue))
            {
                gameManager.SetActiveState(GameManager.GameState.GameOverAnimation);
            }
		}

        private void InputProcessing()
        {
            if (gameManager.inputManager.IsLeftMouseDown)
            {
                ShadowCasterGroup shadowCasterGroup = gameManager.shadowProjector.shadowCasters;
                
                if (gameManager.difficultyLevel == GameManager.DifficultyLevel.Hard && gameManager.inputManager.IsShiftKeyDown)
                {
                    shadowCasterGroup.transform.Rotate(
                        _projectorScreenNormal, 
                        -gameManager.inputManager.MousePositionDelta.y * Time.deltaTime * 10.0f,
                        Space.World);
                }
                else if (gameManager.difficultyLevel > GameManager.DifficultyLevel.Easy && gameManager.inputManager.IsCtrlKeyDown)
                {
                    shadowCasterGroup.GetSelectedShadowCaster().transform.Rotate(
                        _projectorScreenNormal, 
                        -gameManager.inputManager.MousePositionDelta.y * Time.deltaTime * 10.0f,
                        Space.World);
                }
                else
                {
                    ShadowCaster shadowCaster = gameManager.shadowProjector.shadowCasters.GetSelectedShadowCaster();
                    
                    shadowCaster.transform.Rotate(
                        shadowCaster.gameObject.transform.up, 
                        -gameManager.inputManager.MousePositionDelta.x * Time.deltaTime * 10.0f,
                        Space.World);
                    
                    if (gameManager.difficultyLevel >= GameManager.DifficultyLevel.Medium)
                    {
                        Vector3 cameraToShadowCaster = gameManager.shadowProjector.shadowCasters.gameObject.transform.position - _cameraPosition;
                        Vector3 cameraUpCrossCameraToShadowCaster = Vector3.Cross(_cameraUp, cameraToShadowCaster);
                        
                        shadowCaster.transform.Rotate(
                            cameraUpCrossCameraToShadowCaster, 
                            gameManager.inputManager.MousePositionDelta.y * Time.deltaTime * 10.0f,
                            Space.World);
                    }
                }
            }
        }
        
        private (float, int) CalculateCurrentResult()
        {
            int bestResultIndex = -1;
            float bestResult = -1.0f;
            for (int i = 0; i < gameManager.successfulSnapshots.Count; i++)
            {
                float snapshotsComparisonResultPercent =
                    gameManager.shadowProjector.snapshotUtility.CompareSnapshotWithProjection(gameManager.successfulSnapshots[i]);

                if (snapshotsComparisonResultPercent >= bestResult)
                {
                    bestResult = snapshotsComparisonResultPercent;
                    bestResultIndex = i;
                }
            }
            
            // Debug visualisation
            DebugSnapshotVisualizer debugSnapshotVisualizer = gameManager.shadowProjector.TryGetDebugSnapshotVisualizer();
            if (debugSnapshotVisualizer && bestResultIndex != -1)
            {
                debugSnapshotVisualizer.SetSnapshot(gameManager.successfulSnapshots[bestResultIndex], bestResultIndex);
            }

            return (bestResult, bestResultIndex);
        }
        
        private bool _isTimerActive = false;
        private float _startTimerTime;
        private float _timer;
        private bool CheckGameOver(float currentResult)
        {
            if (currentResult > shadowPrecisionForWin)
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