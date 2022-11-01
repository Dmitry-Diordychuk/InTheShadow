using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace InTheShadow
{
    public class SnapshotMaker : MonoBehaviour
    {
        public ShadowProjectorCamera projectorCamera;
        public ShadowCasterGroup shadowCasterGroup;

        private void OnValidate()
        {
            if (!projectorCamera)
            {
                Debug.LogWarning("Projection camera render texture isn't set!", this);
            }

            if (!shadowCasterGroup)
            {
                Debug.LogWarning("Shadows casters missing!", this);
            }
        }

        public void MakeSnapshot()
        {
            Texture2D snapshot = SnapshotUtility.GetShadowSnapshot(projectorCamera.GetRenderTarget());

            string filename = $"{SceneManager.GetActiveScene().name}_snapshot";
            string path = "Assets/Resources/Snapshots";

            List<Quaternion> rotations = shadowCasterGroup.GetAllRotations();

            SnapshotUtility.SaveSnapshotAsRawData(snapshot, rotations, path, filename);
        }
    }
}
