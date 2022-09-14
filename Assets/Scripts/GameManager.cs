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
        
        [SerializeField] private InputManager inputManager;
        [SerializeField] private ShadowCasterController shadowCasterController;
        [SerializeField] private RenderTexture renderTexture;
        [SerializeField] private GameObject snapshotQuad;

        [Header("Gameplay")]
        [SerializeField] private DifficultyLevel difficultyLevel = DifficultyLevel.Easy;

        private Renderer _snapshotQuadRend;
        private Vector3 _cameraUp;
        private Vector3 _cameraRight;
        private Vector3 _cameraForward;
        private List<Texture2D> _successfulSnapshots;
        private List<Quaternion> _successfulRotations;
        

        // Start is called before the first frame update
        void Start()
        {
            Camera mainCamera = Camera.main;
            if (!mainCamera) throw new UnityException("Main camera doesn't exist");
            Transform cameraTransform = mainCamera.transform;
            
            _cameraUp = cameraTransform.up;
            _cameraRight = cameraTransform.right;
            _cameraForward = cameraTransform.forward;
            (_successfulSnapshots, _successfulRotations) = ShadowSnapshotUtility.LoadSnapshotFromRawData(
                Path.Combine("Assets/Resources/Snapshots", $"{SceneManager.GetActiveScene().name}_snapshot"));
            _snapshotQuadRend = snapshotQuad.GetComponent<Renderer>();
        }

        private int _currentSnapshotOnQuad = -1;
        // Update is called once per frame
        void Update()
        {
            if (inputManager.IsLeftMouseDown)
            {
                if (difficultyLevel == DifficultyLevel.Hard && inputManager.IsAlterMoveKeyDown)
                {
                    shadowCasterController.Rotate(_cameraForward, 
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
            
            if (_currentSnapshotOnQuad != bestResultIndex)
            {
                _snapshotQuadRend.material.mainTexture = _successfulSnapshots[bestResultIndex];
                _currentSnapshotOnQuad = bestResultIndex;
            }
            
            Debug.Log(bestResultIndex + ": " + bestResult);
        }
    }
}
