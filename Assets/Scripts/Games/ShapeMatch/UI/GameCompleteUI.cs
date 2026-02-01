using System;
using UnityEngine;
using DG.Tweening;

namespace SenAware.ShapeMatch
{
    [RequireComponent(typeof(CanvasGroup))]
    public class GameCompleteUI : MonoBehaviour
    {
        [SerializeField] private Transform popupParent;
        [SerializeField] private float popupOpenDuration = 0.75f;
        [SerializeField] private float starScaleDuration = 0.5f;
        [SerializeField] private float starsDelay = 0.25f;
        [SerializeField] private Transform[] stars;
        
        private Vector3[] _initialStarScales;
        
        private CanvasGroup _canvasGroup;

        private void Awake()
        {
            TryGetComponent(out _canvasGroup);
            CommonMethods.ToggleCanvasGroup(_canvasGroup, false);
            ShapeMatchStatic.OnShapeMatchGameEnd += OnShapeMatchGameEnd;
            
            _initialStarScales = new Vector3[stars.Length];
            for (var i = 0; i < stars.Length; i++)
            {
                _initialStarScales[i] = stars[i].localScale;
                stars[i].localScale = Vector3.zero;
            }
        }

        private void OnDestroy()
        {
            ShapeMatchStatic.OnShapeMatchGameEnd -= OnShapeMatchGameEnd;
        }

        private void OnShapeMatchGameEnd()
        {
            CommonMethods.ToggleCanvasGroup(_canvasGroup, true, 0.1f);
            CommonMethods.OpenPopup(popupParent, popupOpenDuration, OnPopupOpened);
        }
        
        public void OnContinueButtonPressed()
        {
            CommonMethods.ClosePopup(popupParent, popupOpenDuration, OnPopupClosed);
        }
        
        private void OnPopupOpened()
        {
            // Scale up all the stars using DoTween
            for (var i = 0; i < stars.Length; i++)
            {
                stars[i].localScale  =Vector3.zero;
                stars[i].DOScale(_initialStarScales[i], starScaleDuration).SetDelay(i * starsDelay);
            }
        }
        
        private void OnPopupClosed()
        {
            CommonMethods.ToggleCanvasGroup(_canvasGroup, false);
            ShapeMatchStatic.OnGameCompleteContinueButtonTapped?.Invoke();
        }
    }
}