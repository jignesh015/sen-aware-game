using System;
using System.Collections.Generic;
using UnityEngine;

namespace SenAware.ShapeMatch
{
    [RequireComponent(typeof(CanvasGroup))]
    public class HeaderUI : MonoBehaviour
    {
        [SerializeField] private float fadeDuration = 0.5f;
        
        private CanvasGroup _canvasGroup;
        
        private void Awake()
        {
            TryGetComponent(out _canvasGroup);
            CommonMethods.ToggleCanvasGroup(_canvasGroup, false);

            ShapeMatchStatic.OnShapeMatchRoundStarted += OnShapeMatchRoundStarted;
        }

        private void OnDestroy()
        {
            ShapeMatchStatic.OnShapeMatchRoundStarted -= OnShapeMatchRoundStarted;
        }
        
        public void OnPauseButtonPressed()
        {
            GlobalStatic.OnPauseButtonPressed?.Invoke();
        }

        #region  Event Handlers
        private void OnShapeMatchRoundStarted(int roundNumber, ShapesSO arg1, List<ShapesSO> arg2)
        {
            CommonMethods.ToggleCanvasGroup(_canvasGroup, true, fadeDuration);
        }
        #endregion
    }
}