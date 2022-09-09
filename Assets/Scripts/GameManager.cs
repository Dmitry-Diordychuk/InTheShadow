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
        [SerializeField] private InputManager inputManager;
        [SerializeField] private ShadowCasterController shadowCasterController;
        [SerializeField] private GameObject snapshotQuad;
        
        private Texture2D _sceneSnapshot;

        // Start is called before the first frame update
        void Start()
        {
            _sceneSnapshot = ShadowSnapshotUtility.LoadSnapshotFromRawData(
                Path.Combine("Assets/Resources/Snapshots", $"{SceneManager.GetActiveScene().name}_snapshot"));
            snapshotQuad.GetComponent<Renderer>().material.mainTexture = _sceneSnapshot;
        }

        // Update is called once per frame
        void Update()
        {
            if (inputManager.IsLeftMouseDown)
            {
                shadowCasterController.RotateY(inputManager.MousePositionDelta.x);
            }

            
        }
    }
}
