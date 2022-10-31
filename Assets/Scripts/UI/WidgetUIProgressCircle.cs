using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace InTheShadow
{
    [RequireComponent(typeof(Image))]
    public class WidgetUIProgressCircle : MonoBehaviour
    {
        [SerializeField] private readonly float _redIndicationBound = 0.95f;
        [SerializeField] private readonly float _yellowIndicationBound = 0.97f;
        [SerializeField] private readonly float _greenIndicationBound = 1.0f;

        private Image _progressCircle;
        
        private void Start()
        {
            if (_redIndicationBound > _yellowIndicationBound || _yellowIndicationBound > _greenIndicationBound)
            {
                Debug.LogWarning("There is error in progress logic!", this);
            }
            
            _progressCircle = GetComponent<Image>();
        }

        public void SetProgress(float value)
        {
            if (value < _redIndicationBound)
            {
                _progressCircle.color = Color.red;
            }
            else if (value < _yellowIndicationBound)
            {
                _progressCircle.color = Color.yellow;
            }
            else if (value <= _greenIndicationBound)
            {
                _progressCircle.color = Color.green;
            }
            
            _progressCircle.fillAmount = value;
        }
    }
}
