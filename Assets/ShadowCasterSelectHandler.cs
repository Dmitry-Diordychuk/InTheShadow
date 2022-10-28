using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InTheShadow
{
    public class ShadowCasterSelectHandler : MonoBehaviour
    {
        private ShadowCasterController _shadowCasterController;
        
        private void Start()
        {
            _shadowCasterController = GetComponentInParent<ShadowCasterController>();
            if (!_shadowCasterController)
            {
                Debug.LogWarning("ShadowCasterController is missing in ShadowCasterSelectHandler script!");
            }
        }

        private void OnMouseDown()
        {
            _shadowCasterController.selectEvent.Invoke(gameObject);
        }
    }
}
