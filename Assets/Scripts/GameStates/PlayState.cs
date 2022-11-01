using UnityEngine;

namespace InTheShadow.GameStates
{
    public class PlayState : _GameState
    {
        [Range(0.0f, 1.0f)]
        [SerializeField] private float shadowPrecisionForWin;
        
        [SerializeField] private ComputeShader shader;
        [SerializeField] private Material debugMaterial;
        [SerializeField] private RenderTexture cameraRT;
        
        //private RenderTexture _cameraRT;
        private ComputeBuffer _comparisonShaderResult;
        private int _kernel;
        private uint _threadsX;
        private uint _threadsY;
        private float[] _result;
        
        public override void InitState(GameManager manager)
		{
            State = GameManager.GameState.Play;
            base.InitState(manager);
            
            _kernel = shader.FindKernel ("CSMain");
            shader.GetKernelThreadGroupSizes(_kernel, out _threadsX, out _threadsY, out _);
            
            cameraRT = gameManager.shadowProjector.projectorCamera.GetRenderTarget();

            _comparisonShaderResult = new ComputeBuffer(cameraRT.width * cameraRT.height, sizeof(float));
            shader.SetBuffer(_kernel, "Result", _comparisonShaderResult);
            shader.SetInt("Size", cameraRT.width);
            
            shader.SetTexture(_kernel, "Camera", cameraRT);
            
            _result = new float[cameraRT.width * cameraRT.height];
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

        public override void FinishState()
        {
            base.FinishState();
            _comparisonShaderResult.Release();
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
                // float snapshotsComparisonResultPercent = SnapshotUtility.CompareSnapshots(
                //     SnapshotUtility.GetShadowSnapshot(gameManager.shadowProjector.projectorCamera.GetRenderTarget()),
                //     gameManager.successfulSnapshots[i]);

                
                Texture texture2 = gameManager.successfulSnapshots[i];
                shader.SetTexture(_kernel, "Sample", texture2);

                shader.Dispatch(
                    _kernel, 
                    (int)(cameraRT.width / _threadsX), 
                    (int)(cameraRT.height / _threadsY), 
                    1);
                
                
                _comparisonShaderResult.GetData(_result);

                float sum = 0.0f;
                foreach (var value in _result)
                {
                    sum += value;
                }

                float snapshotsComparisonResultPercent = 1.0f - sum / (cameraRT.width * cameraRT.height);
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