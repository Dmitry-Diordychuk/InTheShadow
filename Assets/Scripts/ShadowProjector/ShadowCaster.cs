using System;
using UnityEngine;
using UnityEngine.Events;

namespace InTheShadow
{
    public class ShadowCaster : MonoBehaviour
    {
        public ShadowCasterOutline shadowCasterOutline;

        public class SelectEvent : UnityEvent<ShadowCaster> { }
        public readonly SelectEvent selectEvent = new SelectEvent();

        private void Start()
        {
            if (!shadowCasterOutline)
            {
                Debug.LogWarning("There is no outline script!", this);
            }
        }

        private void OnMouseDown()
        {
            selectEvent.Invoke(this);
        }
    }
}
