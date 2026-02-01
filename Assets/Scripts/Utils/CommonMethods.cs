using System;
using UnityEngine;
using DG.Tweening;

namespace SenAware
{
    public static class CommonMethods
    {
        public static void ToggleCanvasGroup(CanvasGroup canvasGroup, bool isVisible, float fadeDuration = 0f)
        {
            if (!canvasGroup) return;
            
            canvasGroup.interactable = isVisible;
            canvasGroup.blocksRaycasts = isVisible;
            canvasGroup.DOFade(isVisible ? 1f : 0f, fadeDuration).SetUpdate(true);
        }
        
        public static void FadeCanvasGroup(CanvasGroup canvasGroup, float targetAlpha, float fadeDuration)
        {
            if (!canvasGroup) return;
            
            canvasGroup.DOFade(targetAlpha, fadeDuration).SetUpdate(true);
        }
        
        public static void OpenPopup(Transform popupParent, float tweenDuration = 0.5f, Action onComplete = null)
        {
            if (!popupParent) return;

            popupParent.localScale = Vector3.zero;
            popupParent.DOScale(Vector3.one, tweenDuration).SetEase(Ease.OutBack).SetUpdate(true)
                .OnComplete(() => onComplete?.Invoke());
        }
        
        public static void ClosePopup(Transform popupParent, float tweenDuration = 0.5f, Action onComplete = null)
        {
            if (!popupParent) return;

            popupParent.DOScale(Vector3.zero, tweenDuration).SetEase(Ease.InBack).SetUpdate(true)
                .OnComplete(() => onComplete?.Invoke());
        }
        
        public static bool IsSoundEnabled()
        {
            return PlayerPrefs.GetInt(GlobalStatic.SoundPrefKey, 1) == 1;
        }
        
        public static bool IsMusicEnabled()
        {
            return PlayerPrefs.GetInt(GlobalStatic.MusicPrefKey, 1) == 1;
        }
    }
}