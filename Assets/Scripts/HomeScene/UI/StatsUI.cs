using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace SenAware.HomeScene
{
    [RequireComponent(typeof(CanvasGroup))]
    public class StatsUI : MonoBehaviour
    {
        [SerializeField] private Transform popupParent;
        [SerializeField] private float popupOpenDuration = 0.75f;
        
        [Header("REFERENCES")]
        [SerializeField] private SessionInfoButton sesionInfoButtonPrefab;
        [SerializeField] private GameRoundInfo gameRoundInfoPrefab;
        [SerializeField] private Transform sessionInfoButtonParent;
        [SerializeField] private Transform gameRoundInfoParent;
        [SerializeField] private ScrollRect sessionScrollRect;
        [SerializeField] private ScrollRect gameRoundInfoScrollRect;
        [SerializeField] private GameObject noHistoryFoundUI;
        [SerializeField] private GameObject noRoundsPlayedUI;
        [SerializeField] private Button closeButton;
        
        private CanvasGroup _canvasGroup;
        private List<SessionInfoButton> _sessionButtons = new List<SessionInfoButton>();
        private string _selectedSessionId;
        private List<SessionAnalytics> _recentSessions = new List<SessionAnalytics>();

        private void Awake()
        {
            closeButton.onClick.AddListener(CloseStatsUI);
            TryGetComponent(out _canvasGroup);
            CommonMethods.ToggleCanvasGroup(_canvasGroup, false);
            GlobalStatic.OnStatsButtonPressed += OnStatsButtonPressed;
        }

        private void OnDestroy()
        {
            GlobalStatic.OnStatsButtonPressed -= OnStatsButtonPressed;
        }

        private async void ShowStatsUI()
        {
            // Fetch session history
            if(!SessionManager.Instance) return;
            
            popupParent.localScale = Vector3.zero;
            noHistoryFoundUI.SetActive(false);
            noRoundsPlayedUI.SetActive(false);
            CommonMethods.ToggleCanvasGroup(_canvasGroup, true, 0.1f);

            var sessionHistory = await SessionManager.Instance.FetchSessionHistoryByGameIDAsync();
            
            // Clear existing children
            ClearChildren(sessionInfoButtonParent);
            ClearChildren(gameRoundInfoParent);
            _sessionButtons.Clear();

            await Awaitable.NextFrameAsync();
            
            if (sessionHistory == null || sessionHistory.Count == 0)
            {
                noHistoryFoundUI.SetActive(true);
                CommonMethods.OpenPopup(popupParent, popupOpenDuration);
                return;
            }
            
            // Get the last 10 sessions (most recent first)
            _recentSessions = sessionHistory
                .OrderByDescending(s => s.sessionStartTime)
                .Take(Mathf.Min(10, sessionHistory.Count))
                .ToList();
            
            // Create session info buttons
            foreach (var session in _recentSessions)
            {
                var buttonInstance = Instantiate(sesionInfoButtonPrefab, sessionInfoButtonParent);
                buttonInstance.Initialize(session.sessionID, 
                    SessionManager.Instance.GetGameTitleByID(session.gameID),
                    session.sessionStartTime, 
                    OnSessionSelected);
                buttonInstance.gameObject.name = $"SessionButton_{session.sessionID}";
                _sessionButtons.Add(buttonInstance);
            }
            
            await Awaitable.NextFrameAsync();
            
            // Select the most recent session by default
            if (_recentSessions.Count > 0)
            {
                OnSessionSelected(_recentSessions[0].sessionID);
            }
            
            await Awaitable.NextFrameAsync();
            await Awaitable.NextFrameAsync();
            
            // Scroll to top using DoTween
            sessionScrollRect.DOKill();
            sessionScrollRect.DOVerticalNormalizedPos(1f, 0.25f).SetEase(Ease.OutQuad).SetUpdate(true);
            
            CommonMethods.OpenPopup(popupParent, popupOpenDuration);
        }
        
        private void OnSessionSelected(string sessionId)
        {
            _selectedSessionId = sessionId;
            
            // Highlight the selected session button, de-highlight others
            foreach (var button in _sessionButtons)
            {
                button.HighlightButton(false);
            }
            
            var selectedButton = _sessionButtons.FirstOrDefault(b => b.gameObject.name.Contains(sessionId));
            if (selectedButton)
            {
                selectedButton.HighlightButton(true);
            }
            
            
            // Fetch the selected session's data and populate round info
            PopulateRoundInfo(sessionId);
        }
        
        private async void PopulateRoundInfo(string sessionId)
        {
            // Clear existing round info
            ClearChildren(gameRoundInfoParent);
            noRoundsPlayedUI.SetActive(false);
            await Awaitable.NextFrameAsync();
            
            // Find the session with the given ID
            var selectedSession = _recentSessions.FirstOrDefault(s => s.sessionID == sessionId);
            if (selectedSession?.gameRounds == null || selectedSession.gameRounds.Count == 0)
            {
                noRoundsPlayedUI.SetActive(true);
                return;
            }
            
            // Create GameRoundInfo for each round
            foreach (var round in selectedSession.gameRounds)
            {
                var roundInstance = Instantiate(gameRoundInfoPrefab, gameRoundInfoParent);
                roundInstance.Initialize(
                    round.roundNumber,
                    (float)round.timeToFirstInteraction,
                    (float)round.timeToSuccessfulInteraction,
                    round.numberOfMistakes,
                    round.difficultyLevel.ToString()
                );
            }
            
            await Awaitable.NextFrameAsync();
            await Awaitable.NextFrameAsync();
            
            // Scroll to top
            gameRoundInfoScrollRect.DOKill();
            gameRoundInfoScrollRect.DOVerticalNormalizedPos(1f, 0.25f).SetEase(Ease.OutQuad).SetUpdate(true);
        }
        
        private void ClearChildren(Transform parent)
        {
            foreach (Transform child in parent)
            {
                Destroy(child.gameObject);
            }
        }

        private void OnStatsButtonPressed()
        {
            ShowStatsUI();
        }

        private void CloseStatsUI()
        {
            CommonMethods.ClosePopup(popupParent, popupOpenDuration,
                () => { CommonMethods.ToggleCanvasGroup(_canvasGroup, false); });
        }
    }
}