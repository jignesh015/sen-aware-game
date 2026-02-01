using System;
using UnityEngine;
using UnityEngine.UI;

namespace SenAware.HomeScene
{
    public class GameButton : MonoBehaviour
    {
        [SerializeField] private GameInfo gameInfo;
        
        private Button _gameButton;

        private void Awake()
        {
            TryGetComponent(out _gameButton);
            if (_gameButton != null)
            {
                _gameButton.onClick.AddListener(OnGameButtonClicked);
            }
        }
        
        private void OnGameButtonClicked()
        {
            Debug.Log($"Game Button Clicked: {gameInfo.gameTitle}");
            GlobalStatic.OnGameButtonPressed?.Invoke(gameInfo);
        }
    }
}