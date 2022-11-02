using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InTheShadow
{
    public class ShadowProjector : MonoBehaviour
    {
        public ShadowProjectorCamera projectorCamera;
        public ShadowProjectorScreen screen;
        public ShadowProjectorSpotlight spotlight;
        public ShadowCasterGroup shadowCasters;
        public SnapshotUtility snapshotUtility;

        [Header("Debug")]
        public DebugSnapshotVisualizer snapshotVisualizer;

        private void Start()
        {
            if (!projectorCamera)
            {
                Debug.LogWarning("There is no projector camera!", this);
            }
            if (!screen)
            {
                Debug.LogWarning("There is no projector screen!", this);
            }
            if (!spotlight)
            {
                Debug.LogWarning("There is no projector spotlight!", this);
            }
            if (!shadowCasters)
            {
                Debug.LogWarning("There is no projector shadow caster group!", this);
            }
        }

        public DebugSnapshotVisualizer TryGetDebugSnapshotVisualizer()
        {
            if (snapshotVisualizer)
            {
                return snapshotVisualizer;
            }

            return null;
        }
    }
}
