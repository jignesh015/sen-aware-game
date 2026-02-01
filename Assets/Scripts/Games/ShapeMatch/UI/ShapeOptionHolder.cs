using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SenAware.ShapeMatch
{
    [RequireComponent(typeof(CanvasGroup))]
    public class ShapeOptionHolder : MonoBehaviour
    {
        [SerializeField] private ShapeOption shapeOptionPrefab;
        [SerializeField] private float spawnInterval = 0.1f;
        [SerializeField] private float fadeInDuration = 0.5f;
        
        [Header("SPACING SETTINGS")]
        [SerializeField] private HorizontalLayoutGroup layoutGroup;
        [SerializeField] private Vector2Int[] spacingByOptionCount;
        
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
            // Clear existing options
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
            
            // Adjust spacing based on option count
            var spacingSetting = Array.Find(spacingByOptionCount, s => s.x == shapeOptions.Count);
            layoutGroup.spacing = Mathf.Max(spacingSetting.y, 50);

            CommonMethods.ToggleCanvasGroup(_canvasGroup, true);
            
            var childIndex = 0;
            // Instantiate new shape options
            foreach (var shapeOption in shapeOptions)
            {
                var optionInstance = Instantiate(shapeOptionPrefab, transform);
                optionInstance.SetShapeOption(shapeOption, 
                    shapeToMatch.shapeName == shapeOption.shapeName,
                    spawnInterval * childIndex,
                    fadeInDuration,
                    ShapeMatchStatic.UseExtendedTouchAreas,
                    OnShapeOptionTapped);
                childIndex++;
            }
        }
        
        private void OnShapeOptionTapped(ShapesSO tappedShape, bool isCorrect)
        {
            ShapeMatchStatic.OnShapeOptionTapped?.Invoke(tappedShape, isCorrect);
        }

        private void OnShapeMatchRoundEnded()
        {
            CommonMethods.ToggleCanvasGroup(_canvasGroup, false, fadeInDuration/4);
        }
    }
}