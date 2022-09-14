using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace InTheShadow
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private Image progressCircle;

        private readonly float _redIndicationBound = 0.95f;
        private readonly float _yellowIndicationBound = 0.97f;
        private readonly float _greenIndicationBound = 1.0f;
        
        public void SetProgress(float value)
        {
            if (value < _redIndicationBound)
            {
                progressCircle.color = Color.red;
            }
            else if (value < _yellowIndicationBound)
            {
                progressCircle.color = Color.yellow;
            }
            else if (value <= _greenIndicationBound)
            {
                progressCircle.color = Color.green;
            }
            
            progressCircle.fillAmount = value;
        }
    }
}
