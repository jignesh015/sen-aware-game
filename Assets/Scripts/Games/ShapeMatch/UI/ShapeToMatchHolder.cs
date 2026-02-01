using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SenAware.ShapeMatch
{
    [RequireComponent(typeof(CanvasGroup))]
    public class ShapeToMatchHolder : MonoBehaviour
    {
        [SerializeField] private Image shapeImage;

        [Header("TWEEN SETTINGS")]
        [SerializeField] private float fadeDuration = 0.5f;
        
        private CanvasGroup _canvasGroup;

        private void Awake()
        {
            TryGetComponent(out _canvasGroup);
            CommonMethods.ToggleCanvasGroup(_canvasGroup, false);

            ShapeMatchStatic.OnShapeMatchRoundStarted += OnShapeMatchRoundStarted;
            ShapeMatchStatic.OnShapeMatchRoundEnded += OnShapeMatchRoundEnded;
        }
        
        private void OnDestroy()
        {
            ShapeMatchStatic.OnShapeMatchRoundStarted -= OnShapeMatchRoundStarted;
            ShapeMatchStatic.OnShapeMatchRoundEnded -= OnShapeMatchRoundEnded;
        }

        private void OnShapeMatchRoundStarted(int roundNumber, ShapesSO shapeToMatch, List<ShapesSO> shapeOptions)
        {
            shapeImage.sprite = shapeToMatch.shapeSprite;
            CommonMethods.ToggleCanvasGroup(_canvasGroup, true, fadeDuration);
        }

        private void OnShapeMatchRoundEnded()
        {
            CommonMethods.ToggleCanvasGroup(_canvasGroup, false, fadeDuration);
        }
    }
}