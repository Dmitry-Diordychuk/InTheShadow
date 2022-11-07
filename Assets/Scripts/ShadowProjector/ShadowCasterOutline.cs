using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InTheShadow
{
    public class ShadowCasterOutline : MonoBehaviour
    {
        private void Start()
        {
            gameObject.layer = LayerMask.NameToLayer("ShadowCaster");
        }

        public void TurnOn()
        {
            gameObject.layer = LayerMask.NameToLayer("Outline");
        }

        public void TurnOff()
        {
            gameObject.layer = LayerMask.NameToLayer("ShadowCaster");
        }
    }
}
