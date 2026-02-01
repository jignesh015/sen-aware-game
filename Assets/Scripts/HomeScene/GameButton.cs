using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace SenAware.HomeScene
{
    public class GameButton : MonoBehaviour
    {
        [SerializeField] private GameInfo gameInfo;
        
        [Header("TWEEN SETTINGS")]
        [SerializeField] private Transform playButtonTransform;
        [SerializeField] private float scaleValue = 0.8f;
        [SerializeField] private float tweenDuration = 1f;
        
        private Button _gameButton;

        private void Awake()
        {
            TryGetComponent(out _gameButton);
            if (_gameButton != null)
            {
                _gameButton.onClick.AddListener(OnGameButtonClicked);
            }
            
            // Tween the playButtonTransform to scale up and down continuously
            if (playButtonTransform)
            {
                playButtonTransform.DOScale(scaleValue, tweenDuration)
                    .SetLoops(-1, LoopType.Yoyo)
                    .SetEase(Ease.InOutSine);
            }
        }
        
        private void OnGameButtonClicked()
        {
            Debug.Log($"Game Button Clicked: {gameInfo.gameTitle}");
            GlobalStatic.OnGameButtonPressed?.Invoke(gameInfo);
        }
    }
}