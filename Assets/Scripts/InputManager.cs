using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace InTheShadow
{
    public class InputManager : MonoBehaviour
    {
        public float sensitivity = 2.5f;
        private bool _leftMouseDownStarted;
        private Vector3 _prevMousePosition;
        private Vector3 _mousePositionDelta;

        public bool IsLeftMouseDown => _leftMouseDownStarted;

        public bool IsCtrlKeyDown => Input.GetKey(KeyCode.LeftControl);
        public bool IsShiftKeyDown => Input.GetKey(KeyCode.LeftShift);
        public Vector3 MousePositionDelta => _mousePositionDelta * sensitivity;

        // Start is called before the first frame update
        void Start()
        {
            _leftMouseDownStarted = false;
            _prevMousePosition = Vector3.zero;
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButton(0))
            {
                if (_leftMouseDownStarted)
                {
                    Vector3 currentMousePosition = Input.mousePosition;
                    _mousePositionDelta = currentMousePosition - _prevMousePosition;
                    _prevMousePosition = currentMousePosition;
                }
                else
                {
                    _leftMouseDownStarted = true;
                    _prevMousePosition = Input.mousePosition;
                }
            }
            else
            {
                _leftMouseDownStarted = false;
                _prevMousePosition = Vector3.zero;
            }
        }
    }
}