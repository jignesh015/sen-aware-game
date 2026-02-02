using System;
using UnityEngine;
using DG.Tweening;

namespace SenAware.HomeScene
{
    public class HomeSceneManager : MonoBehaviour
    {
        [SerializeField] private GameObject loadingScreen;
        [SerializeField] private AudioClip bgmClip;
        
        [Header("TITLE HOVER SETTINGS")]
        [SerializeField] private Transform titleTextTransform;
        [SerializeField] private float hoverDuration = 1.5f;
        [SerializeField] private float hoverAmount = 10f;
         
        private void Awake()
        {
            GlobalStatic.OnGameButtonPressed += OnGameButtonPressed;
        }

        private void OnDestroy()
        {
            GlobalStatic.OnGameButtonPressed -= OnGameButtonPressed;
        }

        private void Start()
        {
            GlobalStatic.OnBGMRequested?.Invoke(bgmClip);
            
            if (titleTextTransform)
            {
                // Using DoTween, hover titleTextTransform up and down in loop
                titleTextTransform.DOMoveY(titleTextTransform.position.y + hoverAmount, hoverDuration)
                    .SetLoops(-1, DG.Tweening.LoopType.Yoyo)
                    .SetEase(DG.Tweening.Ease.InOutSine);
            }
        }
        
        public void OnStatsButtonPressed()
        {
            GlobalStatic.OnStatsButtonPressed?.Invoke();
        }

        #region Event Handlers

        private void OnGameButtonPressed(GameInfo gameInfo)
        {
            if (loadingScreen)
            {
                loadingScreen.SetActive(true);
            }
        }
        #endregion
    }
}