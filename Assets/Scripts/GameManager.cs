using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InTheShadow
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private InputManager inputManager;
        [SerializeField] private ShadowCasterController shadowCasterController;

        private Texture2D _sceneSnapshot;

        // Start is called before the first frame update
        void Start()
        {
            //_sceneSnapshot
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
