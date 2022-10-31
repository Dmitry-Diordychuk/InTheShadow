using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace InTheShadow
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private WidgetUIProgressCircle progressCircle;

        // ReSharper disable Unity.PerformanceAnalysis
        public void SetShadowComparisonProgressInPercent(float value)
        {
            if (value < 0.0)
            {
                Debug.LogWarning("Value must be > than 0.0f", this);
            }
            else if (value > 100.0f)
            {
                Debug.LogWarning("Value must be < than 100.0f", this);
            }
            progressCircle.SetProgress(value);
        }
    }
}
