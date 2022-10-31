using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InTheShadow
{
    [RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
    public class ShadowProjectorScreen : MonoBehaviour
    {
        private MeshFilter _meshFilter;
        
        private void Start()
        {
            _meshFilter = GetComponent<MeshFilter>();
        }

        public Vector3 GetPosition()
        {
            return transform.position;
        }

        public Vector3 GetNormal()
        {
            return _meshFilter.mesh.normals[0];
        }
    }
}
