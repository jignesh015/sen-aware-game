using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SenAware.ShapeMatch
{
    [RequireComponent(typeof(CanvasGroup))]
    public class RoundTimerUI : MonoBehaviour
    {
        [SerializeField] private Image timerFillImage;
            
        [SerializeField] private float fadeDuration = 0.5f;
        [SerializeField] private float fillLerpSpeed = 5f;
        
        private CanvasGroup _canvasGroup;
        private bool _isTimerRunning;

        private void Awake()
        {
            TryGetComponent(out _canvasGroup);
            CommonMethods.ToggleCanvasGroup(_canvasGroup, false);
            timerFillImage.fillAmount = 1f;

            ShapeMatchStatic.OnShapeMatchRoundStarted += OnShapeMatchRoundStarted;
            ShapeMatchStatic.OnShapeMatchRoundEnded += OnShapeMatchRoundEnded;
            ShapeMatchStatic.OnRoundTimerUpdated += OnRoundTimerUpdated;
        }
        
        private void OnDestroy()
        {
            ShapeMatchStatic.OnShapeMatchRoundStarted -= OnShapeMatchRoundStarted;
            ShapeMatchStatic.OnShapeMatchRoundEnded -= OnShapeMatchRoundEnded;
            ShapeMatchStatic.OnRoundTimerUpdated -= OnRoundTimerUpdated;
        }

        #region  Event Handlers
        private void OnShapeMatchRoundStarted(int roundNumber, ShapesSO arg1, List<ShapesSO> arg2)
        {
            timerFillImage.fillAmount = 1f;
            CommonMethods.ToggleCanvasGroup(_canvasGroup, true, fadeDuration);
        }

        private void OnShapeMatchRoundEnded()
        {
            CommonMethods.ToggleCanvasGroup(_canvasGroup, false, fadeDuration);
        }

        private void OnRoundTimerUpdated(float timeRemaining)
        {
            timerFillImage.fillAmount =  Mathf.Lerp(timerFillImage.fillAmount, 
                timeRemaining / ShapeMatchStatic.RoundTimerDuration, Time.deltaTime * fillLerpSpeed) ;
        }
        #endregion
    }
}