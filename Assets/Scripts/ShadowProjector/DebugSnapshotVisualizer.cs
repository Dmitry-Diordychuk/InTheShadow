using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InTheShadow
{
    [RequireComponent(typeof(Renderer))]
    public class DebugSnapshotVisualizer : MonoBehaviour
    {
        private Renderer _snapshotRenderer;
        private int _currentSnapshotIndex = -1;

        private void Start()
        {
            _snapshotRenderer = GetComponent<Renderer>();
        }

        public void SetSnapshot(Texture texture, int index)
        {
            if (index < 0)
            {
                return;
            }
            
            if (_currentSnapshotIndex == index)
            {
                return;
            }
            _snapshotRenderer.material.mainTexture = texture;
            _currentSnapshotIndex = index;
        }
    }
}
