using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InTheShadow
{
    [RequireComponent(typeof(Camera))]
    public class ShadowProjectorCamera : MonoBehaviour
    {
        private RenderTexture _renderTexture;
        private Camera _camera;

        private void Start()
        {
            _camera = GetComponent<Camera>();
            _renderTexture = _camera.targetTexture;
        }

        // ReSharper disable Unity.PerformanceAnalysis
        public RenderTexture GetRenderTarget()
        {
            if (!_renderTexture)
            {
                Debug.LogWarning("There is no projector camera render target!", this);
            }
            return _renderTexture;
        }
    }
}
