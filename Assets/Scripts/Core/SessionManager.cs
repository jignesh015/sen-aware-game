using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SenAware
{
    public class SessionManager : MonoBehaviour
    {
        public static SessionManager Instance { get; private set; }
        
        public SessionAnalytics CurrentSessionAnalytics;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            // Unparent from any parent to persist across scenes
            transform.SetParent(null);
            Instance = this;
            DontDestroyOnLoad(gameObject);
            AssignListeners();
        }

        private void OnDestroy()
        {
            RemoveListeners();
        }

        private void AssignListeners()
        {
            GlobalStatic.OnGameButtonPressed += HandleGameButtonPressed;
            GlobalStatic.OnQuitToHomeButtonPressed += HandleQuitToHomeButtonPressed;
        }
        
        private void RemoveListeners()
        {
            GlobalStatic.OnGameButtonPressed -= HandleGameButtonPressed;
            GlobalStatic.OnQuitToHomeButtonPressed -= HandleQuitToHomeButtonPressed;
        }

        private async void StartNewSession(GameInfo gameInfo)
        {
            CurrentSessionAnalytics = new SessionAnalytics
            {
                gameID = gameInfo.gameID,
                gameRounds = new List<SingleRoundAnalytics>(),
                adaptiveChangesMade = 0,
                inattentiveWarnings = 0
            };
            Debug.Log($"New session started for game: {gameInfo.gameTitle}");
            
            await Awaitable.WaitForSecondsAsync(GlobalStatic.NewSceneLoadDelay);
            SceneManager.LoadScene(gameInfo.gameSceneName);
        }
        
        #region Event Handlers
        

        private void HandleGameButtonPressed(GameInfo gameInfo)
        {
            StartNewSession(gameInfo);
        }

        private async void HandleQuitToHomeButtonPressed()
        {
            await Awaitable.WaitForSecondsAsync(GlobalStatic.NewSceneLoadDelay);
            SceneManager.LoadScene(GlobalStatic.HomeScene);
        }
        #endregion
    }
}