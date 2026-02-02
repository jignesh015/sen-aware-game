using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.SceneManagement;

namespace SenAware
{
    public class SessionManager : MonoBehaviour
    {
        public List<GameInfo> AvailableGames = new List<GameInfo>();
        
        public SessionAnalytics CurrentSessionAnalytics { get; private set; }
        public GameInfo CurrentGameInfo { get; private set; }

        public static SessionManager Instance { get; private set; }
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
            GlobalStatic.OnSessionEndRequested += HandleSessionEndRequested;
        }
        
        private void RemoveListeners()
        {
            GlobalStatic.OnGameButtonPressed -= HandleGameButtonPressed;
            GlobalStatic.OnQuitToHomeButtonPressed -= HandleQuitToHomeButtonPressed;
            GlobalStatic.OnSessionEndRequested -= HandleSessionEndRequested;
        }

        private async void StartNewSession(GameInfo gameInfo)
        {
            // Disable the screen timeout while in a game session
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            
            CurrentGameInfo = gameInfo;
            CurrentSessionAnalytics = new SessionAnalytics
            {
                sessionID = Guid.NewGuid().ToString(),
                sessionStartTime =  DateTime.UtcNow.ToString("o"),
                gameID = gameInfo.gameID,
                gameRounds = new List<SingleRoundAnalytics>(),
                adaptiveChangesMade = 0,
                inattentiveWarnings = 0
            };
            Debug.Log($"New session started for game: {gameInfo.gameTitle}");
            GlobalStatic.OnSessionStarted?.Invoke();

            var hasCamPermission = await CameraPermissionCheck();
            await Awaitable.WaitForSecondsAsync(GlobalStatic.NewSceneLoadDelay);
            await SceneManager.LoadSceneAsync(gameInfo.gameSceneName);
            if(!hasCamPermission) return;
            await SceneManager.LoadSceneAsync(GlobalStatic.FaceDetectionScene, LoadSceneMode.Additive);
        }

        private async Awaitable<bool> CameraPermissionCheck()
        {
            #if UNITY_EDITOR
                return true;
            #endif
            
            #if UNITY_ANDROID
                // Check if camera permission is already granted
                if (Permission.HasUserAuthorizedPermission(Permission.Camera))
                {
                    Debug.Log("Camera permission already granted.");
                    return true;
                }

                // Request camera permission
                Debug.Log("Requesting camera permission from user...");
                Permission.RequestUserPermission(Permission.Camera);

                // Wait for the permission request to complete
                // We need to wait a bit for the system to process the request
                var maxWaitFrames = 300; // Wait up to 5 seconds (assuming 60 FPS)
                var frameCount = 0;

                while (frameCount < maxWaitFrames)
                {
                    await Awaitable.NextFrameAsync();
                    frameCount++;

                    // Check if permission was granted
                    if (!Permission.HasUserAuthorizedPermission(Permission.Camera)) continue;
                    Debug.Log("Camera permission granted by user.");
                    return true;
                }

                Debug.LogWarning("Camera permission was not granted or request timed out.");
                return false;
            #else
                // For non-Android platforms, assume permission is granted
                return true;
            #endif
        }
        
        private async void EndCurrentSession(bool completedSession)
        {
            try
            {
                // Enable the screen timeout back to system settings
                Screen.sleepTimeout = SleepTimeout.SystemSetting;
                
                if (CurrentSessionAnalytics != null)
                {
                    CurrentSessionAnalytics.completedSession = completedSession;
                    CurrentSessionAnalytics.sessionEndTime = DateTime.UtcNow.ToString("o");
                }
                
                await SaveSessionToHistoryAsync();
            
                await Awaitable.WaitForSecondsAsync(GlobalStatic.NewSceneLoadDelay);
                SceneManager.LoadScene(GlobalStatic.HomeScene);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error ending session: {e.Message}");
                // Enable the screen timeout back to system settings
                Screen.sleepTimeout = SleepTimeout.SystemSetting;
                SceneManager.LoadScene(GlobalStatic.HomeScene);
            }
        }
        
        public string GetGameTitleByID(string gameID)
        {
            var game = AvailableGames.FirstOrDefault(g => g.gameID == gameID);
            return game ? game.gameTitle : "Unknown Game";
        }

        /// <summary>
        /// Saves the current session analytics to the session history and persists it to a JSON file.
        /// </summary>
        public async Awaitable SaveSessionToHistoryAsync()
        {
            try
            {
                if (CurrentSessionAnalytics == null)
                {
                    Debug.LogWarning("No current session to save.");
                    return;
                }

                var persistentDataPath = Application.persistentDataPath;
                var historyFilePath = Path.Combine(persistentDataPath, GlobalStatic.SessionHistoryFileName);

                SessionHistory sessionHistory = null;

                // Load existing history if it exists
                if (File.Exists(historyFilePath))
                {
                    var json = await File.ReadAllTextAsync(historyFilePath);
                    sessionHistory = JsonUtility.FromJson<SessionHistory>(json);
                }

                // Create new history if it doesn't exist
                sessionHistory ??= new SessionHistory
                {
                    userID = GlobalStatic.UserID,
                    sessions = new List<SessionAnalytics>()
                };

                // Add current session to history
                sessionHistory.sessions.Add(CurrentSessionAnalytics);

                // Save to file
                var historyJson = JsonUtility.ToJson(sessionHistory, true);
                await File.WriteAllTextAsync(historyFilePath, historyJson);

                Debug.Log($"Session saved to session history at: {historyFilePath}");
                await Awaitable.NextFrameAsync();
            }
            catch (Exception e)
            {
                Debug.LogError($"Error saving session to history: {e.Message}");
            }
        }

        /// <summary>
        /// Fetches all sessions for a given gameID from the session history.
        /// </summary>
        /// <param name="gameID">The game ID to filter sessions by</param>
        /// <returns>A list of SessionAnalytics for the specified gameID</returns>
        public async Awaitable<List<SessionAnalytics>> FetchSessionHistoryByGameIDAsync(string gameID = null)
        {
            try
            {
                var persistentDataPath = Application.persistentDataPath;
                var historyFilePath = Path.Combine(persistentDataPath, GlobalStatic.SessionHistoryFileName);

                if (!File.Exists(historyFilePath))
                {
                    Debug.LogWarning($"Session history file not found at: {historyFilePath}");
                    return new List<SessionAnalytics>();
                }

                var json = await File.ReadAllTextAsync(historyFilePath);
                var sessionHistory = JsonUtility.FromJson<SessionHistory>(json);

                if (sessionHistory?.sessions == null)
                {
                    Debug.LogWarning("Failed to deserialize session history or sessions list is null.");
                    return new List<SessionAnalytics>();
                }

                if (string.IsNullOrEmpty(gameID))
                {
                    return sessionHistory.sessions;
                }
                
                // Filter sessions by gameID
                var filteredSessions = sessionHistory.sessions.Where(
                    session => session.gameID == gameID).ToList();

                Debug.Log($"Fetched {filteredSessions.Count} sessions from session history for gameID: {gameID}");
                await Awaitable.NextFrameAsync();
                return filteredSessions;
            }
            catch (Exception e)
            {
                Debug.LogError($"Error fetching session history: {e.Message}");
                return new List<SessionAnalytics>();
            }
        }
        
        #region Event Handlers
        private void HandleGameButtonPressed(GameInfo gameInfo)
        {
            StartNewSession(gameInfo);
        }

        private void HandleQuitToHomeButtonPressed()
        {
            // Quitting game mid-way. End session marking it as incomplete.
            EndCurrentSession(false);
        }

        private void HandleSessionEndRequested()
        {
            EndCurrentSession(true);
        }
        #endregion
    }
}