using System;
using UnityEngine;

namespace SenAware
{
    [RequireComponent(typeof(CanvasGroup))]
    public class PauseMenu : MonoBehaviour
    {
        [SerializeField] private Transform popupParent;
        [SerializeField] private float popupOpenDuration = 0.75f;
        
        private CanvasGroup _canvasGroup;
        
        private void Awake()
        {
            TryGetComponent(out _canvasGroup);
            CommonMethods.ToggleCanvasGroup(_canvasGroup, false);
            GlobalStatic.OnPauseButtonPressed += OnPauseButtonPressed;
        }

        private void OnDestroy()
        {
            GlobalStatic.OnPauseButtonPressed -= OnPauseButtonPressed;
        }

        private void OnPauseButtonPressed()
        {
            Time.timeScale = 0f;
            CommonMethods.OpenPopup(popupParent, popupOpenDuration);
            CommonMethods.ToggleCanvasGroup(_canvasGroup, true);
        }
        
        private void OnPopupClosed()
        {
            Time.timeScale = 1f;
            CommonMethods.ToggleCanvasGroup(_canvasGroup, false);
            GlobalStatic.OnResumeButtonPressed?.Invoke();
        }
        
        public void OnResumeButtonPressed()
        {
            CommonMethods.ClosePopup(popupParent, popupOpenDuration,OnPopupClosed);
        }
        
        public void OnQuitButtonPressed()
        {
            Time.timeScale = 1f;
            GlobalStatic.OnQuitToHomeButtonPressed?.Invoke();
        }
    }
}