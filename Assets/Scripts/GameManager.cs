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
        [Header("Dependencies")]
        public InputManager inputManager;
        public UIManager uiManager;
        public CameraController cameraController;
        public ShadowProjector shadowProjector;

        [Header("Gameplay")]
        public DifficultyLevel difficultyLevel = DifficultyLevel.Easy;

        public enum DifficultyLevel
        {
            NotSelected,
            Easy,
            Medium,
            Hard
        }
        
        [Header("States")]
        public _GameState[] allGameStates;
        
        public enum GameState
        {
            Play,
            GameOverAnimation,
            GameOverShowMenu
        }
        
        private readonly Dictionary<GameState, _GameState> _gameStateDictionary = new Dictionary<GameState, _GameState>();
        
        private _GameState _activeState;

        // ReSharper disable Unity.PerformanceAnalysis
        public void SetActiveState(GameState newState)
        {
            if (!_gameStateDictionary.ContainsKey(newState))
            {
                Debug.LogWarning($"The key <b>{newState}</b> doesn't exist so you can't activate the state!");
                return;
            }

            if (_activeState)
            {
                _activeState.FinishState();
            }

            _activeState = _gameStateDictionary[newState];
            
            _activeState.StartState();
        }
        
        // Init Utilities
        [HideInInspector] public List<Texture2D> successfulSnapshots;
        [HideInInspector] public List<List<Quaternion>> successfulRotations;
        [HideInInspector] public int resultIndex;
        [HideInInspector] public float resultValue;

        // Start is called before the first frame update
        void Start()
        {
            if (!inputManager)
            {
                Debug.LogWarning("Missing input manager!", this);
            }
            if (!uiManager)
            {
                Debug.LogWarning("Missing ui manager!", this);
            }
            if (!cameraController)
            {
                Debug.LogWarning("There is no camera!", this);
            }
            if (!shadowProjector)
            {
                Debug.LogWarning("Missing projector!", this);
            }
            
            (successfulSnapshots, successfulRotations) = shadowProjector.snapshotUtility.LoadSnapshots(
                Path.Combine("Assets/Resources/Snapshots", $"{SceneManager.GetActiveScene().name}_snapshot"));

            foreach (_GameState game in allGameStates)
            {
                if (game == null)
                {
                    continue;
                }

                game.InitState(this);

                if (_gameStateDictionary.ContainsKey(game.State))
                {
                    Debug.LogWarning($"The key <b>{game.State}</b> already exists in the game state dictionary!");
                    continue;
                }
                
                _gameStateDictionary.Add(game.State, game);
            }
            
            SetActiveState(GameState.Play);
        }
        
        // Update is called once per frame
        void Update()
        {
            _activeState.UpdateState();
        }
    }
}
