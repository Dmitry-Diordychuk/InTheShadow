using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

namespace InTheShadow
{
    public class GameManager : MonoBehaviour
    {
        enum DifficultyLevel
        {
            Easy,
            Medium,
            Hard
        }

        enum GameStatus
        {
            Play,
            StartLerp,
            Lerp,
            GameOver
        }
        
        [SerializeField] private InputManager inputManager;
        [SerializeField] private UIManager uiManager;
        [SerializeField] private CameraController cameraController;
        [SerializeField] private ShadowCasterController shadowCasterController;
        [SerializeField] private RenderTexture renderTexture;
        [SerializeField] private GameObject shadowProjectionQuad;
        [SerializeField] private GameObject snapshotQuad;

        [Header("Gameplay")]
        [SerializeField] private DifficultyLevel difficultyLevel = DifficultyLevel.Easy;

        private Renderer _snapshotQuadRend;
        private Vector3 _cameraPosition;
        private Quaternion _cameraRotation;
        private Vector3 _cameraUp;
        private Vector3 _cameraRight;
        private Vector3 _shadowProjectionQuadNormal;
        private Vector3 _cameraEndGamePosition;

        private List<Texture2D> _successfulSnapshots;
        private List<Quaternion> _successfulRotations;
        

        // Start is called before the first frame update
        void Start()
        {
            Camera mainCamera = Camera.main;
            if (!mainCamera) throw new UnityException("Main camera doesn't exist");
            Transform cameraTransform = mainCamera.transform;

            _cameraPosition = cameraTransform.position;
            _cameraRotation = cameraTransform.rotation;
            _cameraUp = cameraTransform.up;
            _shadowProjectionQuadNormal = shadowProjectionQuad.GetComponent<MeshFilter>().mesh.normals[0];
            _cameraEndGamePosition = shadowProjectionQuad.transform.position + _shadowProjectionQuadNormal;
            Vector3 cameraToShadowCaster = shadowCasterController.gameObject.transform.position - _cameraPosition;
            _cameraRight = Vector3.Cross(_cameraUp, cameraToShadowCaster);
            (_successfulSnapshots, _successfulRotations) = ShadowSnapshotUtility.LoadSnapshotFromRawData(
                Path.Combine("Assets/Resources/Snapshots", $"{SceneManager.GetActiveScene().name}_snapshot"));
            _snapshotQuadRend = snapshotQuad.GetComponent<Renderer>();
        }

        private GameStatus _gameStatus = GameStatus.Play;

        private int _resultIndex;
        // Update is called once per frame
        void Update()
        {
            if (_gameStatus == GameStatus.StartLerp)
            {
                cameraController.InitLerp(_cameraPosition, _cameraEndGamePosition);
                uiManager.SetProgress(1.0f);
                _gameStatus = GameStatus.Lerp;
                return;
            }
            if (_gameStatus == GameStatus.Lerp)
            {
                float t = cameraController.FocusOnEndPosition(_cameraPosition, _cameraEndGamePosition,
                    _cameraRotation, _cameraRotation * Quaternion.Euler(0.0f, -25.0f, 0.0f));
                shadowCasterController.RotateToSnapshotShadow(shadowCasterController.gameObject.transform.rotation,
                    _successfulRotations[_resultIndex], t);
                if (t >= 1.0f)
                {
                    _gameStatus = GameStatus.GameOver;
                }
                
                return;
            }
            if (_gameStatus == GameStatus.GameOver)
            {
                Debug.Log("Game Over!");
                return;
            }
            
            InputProcessing();

            float result;
            (result, _resultIndex) = CalculateCurrentResult();

            uiManager.SetProgress(result);

            _gameStatus = CheckGameOver(result);
        }

        private void InputProcessing()
        {
            if (inputManager.IsLeftMouseDown)
            {
                if (difficultyLevel == DifficultyLevel.Hard && inputManager.IsAlterMoveKeyDown)
                {
                    shadowCasterController.Rotate(_shadowProjectionQuadNormal, 
                        -inputManager.MousePositionDelta.y * Time.deltaTime * 10.0f);
                }
                else
                {
                    shadowCasterController.Rotate(_cameraUp,
                        -inputManager.MousePositionDelta.x * Time.deltaTime * 10.0f);
                    if (difficultyLevel >= DifficultyLevel.Medium)
                    {
                        shadowCasterController.Rotate(_cameraRight,
                            inputManager.MousePositionDelta.y * Time.deltaTime * 10.0f);
                    }
                }
            }
        }

        private int _currentSnapshotOnQuad = -1;
        private (float, int) CalculateCurrentResult()
        {
            int bestResultIndex = -1;
            float bestResult = -1.0f;
            for (int i = 0; i < _successfulSnapshots.Count; i++)
            {
                float snapshotsComparisonResultPercent = ShadowSnapshotUtility.CompareSnapshots(
                    ShadowSnapshotUtility.GetShadowSnapshot(renderTexture),
                    _successfulSnapshots[i]);
                
                if (snapshotsComparisonResultPercent >= bestResult)
                {
                    bestResult = snapshotsComparisonResultPercent;
                    bestResultIndex = i;
                }
            }
            
            // Debug visualisation
            if (_currentSnapshotOnQuad != bestResultIndex)
            {
                _snapshotQuadRend.material.mainTexture = _successfulSnapshots[bestResultIndex];
                _currentSnapshotOnQuad = bestResultIndex;
            }

            return (bestResult, bestResultIndex);
        }
        
        private bool _isTimerActive = false;
        private float _startTimerTime;
        private float _timer;
        private GameStatus CheckGameOver(float currentResult)
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
                    return GameStatus.StartLerp;
                }
            }
            else
            {
                _isTimerActive = false;
            }

            return GameStatus.Play;
        }
    }
}
