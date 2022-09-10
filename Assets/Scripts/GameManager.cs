using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
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

        private Vector3 _cameraUp;
        private Vector3 _cameraRight;
        private Texture2D _sceneSnapshot;

        // Start is called before the first frame update
        void Start()
        {
            Camera mainCamera = Camera.main;
            if (!mainCamera) throw new UnityException("Main camera doesn't exist");
            Transform cameraTransform = mainCamera.transform;
            _cameraUp = cameraTransform.up;
            _cameraRight = cameraTransform.right;
            _sceneSnapshot = ShadowSnapshotUtility.LoadSnapshotFromRawData(
                Path.Combine("Assets/Resources/Snapshots", $"{SceneManager.GetActiveScene().name}_snapshot"));
            snapshotQuad.GetComponent<Renderer>().material.mainTexture = _sceneSnapshot;
        }

        // Update is called once per frame
        void Update()
        {
            if (inputManager.IsLeftMouseDown)
            {
                shadowCasterController.Rotate(_cameraUp, -inputManager.MousePositionDelta.x * Time.deltaTime * 10.0f);
                if (difficultyLevel >= DifficultyLevel.Medium)
                {
                    shadowCasterController.Rotate(_cameraRight,
                        inputManager.MousePositionDelta.y * Time.deltaTime * 10.0f);
                }
            }
            
            float snapshotsComparisonResultPercent = ShadowSnapshotUtility.CompareSnapshots(
                ShadowSnapshotUtility.GetShadowSnapshot(renderTexture),
                _sceneSnapshot);
            Debug.Log(snapshotsComparisonResultPercent);
        }
    }
}
