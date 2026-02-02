using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SenAware.ShapeMatch
{
    public class ShapeMatchAdaptabilityManager : MonoBehaviour
    {
        private SessionManager _sessionManager;
        
        private void Awake()
        {
            _sessionManager = SessionManager.Instance;
            ShapeMatchStatic.OnShapeMatchRoundEnded += OnShapeMatchRoundEnded;
        }
        
        private void OnDestroy()
        {
            ShapeMatchStatic.OnShapeMatchRoundEnded -= OnShapeMatchRoundEnded;
        }

        private void Start()
        {
            AssessDifficultyBasedOnSessionHistory();
        }

        private async void AssessDifficultyBasedOnSessionHistory()
        {
            try
            {
                if (!_sessionManager || !_sessionManager.CurrentGameInfo)
                {
                    ShapeMatchStatic.CurrentDifficultyLevel = DifficultyLevel.Medium;
                    ShapeMatchStatic.OnDifficultyLevelSet?.Invoke(ShapeMatchStatic.CurrentDifficultyLevel);
                    return;
                }
            
                // Start with Medium by default
                var assessedDifficulty = DifficultyLevel.Medium;
            
                // Fetch session history for this game
                var gameHistory = await _sessionManager.FetchSessionHistoryByGameIDAsync(
                    _sessionManager.CurrentGameInfo.gameID);
            
                if (gameHistory.Count > 0)
                {
                    // Fetch GameAdaptiveDifficultyRules from current game info
                    var rules = _sessionManager.CurrentGameInfo.adaptiveDifficultyRules;
                
                    // Analyze past session data against the rules
                    assessedDifficulty = EvaluateDifficulty(gameHistory, rules);
                    
                    // Check if assessedDifficulty differs from the last played session
                    var lastSession = gameHistory[^1];
                    if (lastSession.generalDifficultyLevel != assessedDifficulty)
                    {
                        _sessionManager.CurrentSessionAnalytics.adaptiveChangesMade++;
                    }
                }
                
                // Set the difficulty level for upcoming rounds
                _sessionManager.CurrentSessionAnalytics.generalDifficultyLevel = assessedDifficulty;
                ShapeMatchStatic.CurrentDifficultyLevel = assessedDifficulty;
                ShapeMatchStatic.OnDifficultyLevelSet?.Invoke(assessedDifficulty);
            
                Debug.Log($"Difficulty assessed as: {assessedDifficulty} for game: {_sessionManager.CurrentGameInfo.gameTitle}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Error assessing difficulty: {e.Message}");
                ShapeMatchStatic.CurrentDifficultyLevel = DifficultyLevel.Medium;
                ShapeMatchStatic.OnDifficultyLevelSet?.Invoke(ShapeMatchStatic.CurrentDifficultyLevel);
            }
        }
        
        /// <summary>
        /// Evaluates the appropriate difficulty level based on past performance.
        /// Compassionate algorithm: starts at Medium and only increases if child demonstrates mastery.
        /// </summary>
        private DifficultyLevel EvaluateDifficulty(List<SessionAnalytics> sessionHistory, GameAdaptiveDifficultyRules rules)
        {
            if (sessionHistory.Count == 0)
                return DifficultyLevel.Medium;
            
            // Extract all completed rounds from session history
            var allRounds = new List<SingleRoundAnalytics>();
            foreach (var session in sessionHistory.Where(session => session.gameRounds != null))
            {
                allRounds.AddRange(session.gameRounds);
            }
            
            if (allRounds.Count == 0)
                return DifficultyLevel.Medium;
            
            // Calculate average metrics for the last few rounds (most relevant data)
            var recentRoundCount = Mathf.Min(5, allRounds.Count); // Look at last 5 rounds
            var recentRounds = allRounds.TakeLast(recentRoundCount).ToList();
            
            var avgTimeToSuccess = (float)recentRounds.Average(r => r.timeToSuccessfulInteraction);
            var avgMistakes = Mathf.RoundToInt((float)recentRounds.Average(r => r.numberOfMistakes));
            
            // Calculate success metrics
            var successfulRounds = recentRounds.Count(r => 
                r.timeToSuccessfulInteraction <= rules.timeToSuccessfulInteractionUpperThreshold &&
                r.numberOfMistakes <= rules.numberOfMistakesUpperThreshold);
            var successRate = (float)successfulRounds / recentRounds.Count;
            
            // Compassionate assessment logic:
            // - Hard: Only if child demonstrates mastery (90%+ success, very low mistakes, fast times)
            // - Medium: Default safe level (start here for new users, or if performing okay)
            // - Easy: If child is struggling significantly (low success rate, high mistakes)
            
            var isDemonstratingMastery = successRate >= 0.90f && 
                                         avgMistakes <= rules.numberOfMistakesLowerThreshold &&
                                         avgTimeToSuccess <= rules.timeToSuccessfulInteractionLowerThreshold;
            
            var isPerformingWell = successRate >= 0.80f && 
                                   avgMistakes <= rules.numberOfMistakesUpperThreshold;
            
            var isStruggling = successRate < 0.60f || 
                               avgMistakes > rules.numberOfMistakesUpperThreshold;
            
            if (isDemonstratingMastery)
            {
                Debug.Log($"Child demonstrating mastery (Success Rate: {successRate:P}, Avg Mistakes: {avgMistakes}) - Setting to Hard");
                return DifficultyLevel.Hard;
            }
            
            if (isStruggling)
            {
                Debug.Log($"Child struggling significantly (Success Rate: {successRate:P}, Avg Mistakes: {avgMistakes}) - Setting to Easy for extra support");
                return DifficultyLevel.Easy;
            }
            
            if (isPerformingWell)
            {
                Debug.Log($"Child performing well (Success Rate: {successRate:P}, Avg Mistakes: {avgMistakes}) - Setting to Medium");
                return DifficultyLevel.Medium;
            }
            
            // Default to Medium - moderate performance, neither struggling nor excelling
            Debug.Log($"Child at baseline performance (Success Rate: {successRate:P}, Avg Mistakes: {avgMistakes}) - Keeping at Medium");
            return DifficultyLevel.Medium;
        }
        
        /// <summary>
        /// Reassesses difficulty after each round based on current session analytics.
        /// Only downgrades difficulty (Hard → Medium or Medium → Easy) if child is struggling.
        /// Never upgrades difficulty during an ongoing session.
        /// </summary>
        private void ReassessDifficultyBasedOnCurrentSessionAnalytics()
        {
            if (!_sessionManager?.CurrentGameInfo || _sessionManager?.CurrentSessionAnalytics == null) 
                return;

            var gameHistory = _sessionManager.CurrentSessionAnalytics.gameRounds;
            if (gameHistory.Count == 0)
                return;

            var rules = _sessionManager.CurrentGameInfo.adaptiveDifficultyRules;
            var currentDifficulty = ShapeMatchStatic.CurrentDifficultyLevel;

            // Only consider the most recent rounds to assess current struggle
            var recentRoundCount = Mathf.Min(3, gameHistory.Count); // Look at last 3 rounds
            var recentRounds = gameHistory.GetRange(gameHistory.Count - recentRoundCount, recentRoundCount);

            // Calculate metrics from recent rounds
            var avgMistakes = recentRounds.Sum(round => round.numberOfMistakes);
            avgMistakes = Mathf.RoundToInt((float)avgMistakes / recentRounds.Count);

            // Calculate success rate for recent rounds
            var successfulRounds = recentRounds.Count(round => round.timeToSuccessfulInteraction <= rules.timeToSuccessfulInteractionUpperThreshold && round.numberOfMistakes <= rules.numberOfMistakesUpperThreshold);
            var successRate = (float)successfulRounds / recentRounds.Count;

            // Determine if child is struggling significantly
            var isStruggling = successRate < 0.60f || avgMistakes > rules.numberOfMistakesUpperThreshold;

            if (!isStruggling)
                return; // Child is doing fine, no need to downgrade

            // Downgrade difficulty if child is struggling
            var newDifficulty = currentDifficulty;
            
            switch (currentDifficulty)
            {
                case DifficultyLevel.Hard:
                    newDifficulty = DifficultyLevel.Medium;
                    Debug.Log($"Child struggling on Hard difficulty (Success Rate: {successRate:P}, Avg Mistakes: {avgMistakes}) - Downgrading to Medium");
                    break;
                case DifficultyLevel.Medium:
                    newDifficulty = DifficultyLevel.Easy;
                    Debug.Log($"Child struggling on Medium difficulty (Success Rate: {successRate:P}, Avg Mistakes: {avgMistakes}) - Downgrading to Easy");
                    break;
                case DifficultyLevel.Easy:
                    // Already at easiest level, cannot downgrade further
                    break;
            }

            // Update difficulty if changed
            if (newDifficulty == currentDifficulty) return;
            ShapeMatchStatic.CurrentDifficultyLevel = newDifficulty;
            ShapeMatchStatic.OnDifficultyLevelSet?.Invoke(newDifficulty);
            _sessionManager.CurrentSessionAnalytics.adaptiveChangesMade++;
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private async void OnShapeMatchRoundEnded()
        {
            try
            {
                // wait for us to record the analytics
                await Awaitable.NextFrameAsync();
                
                // Re-assess difficulty after each round
                ReassessDifficultyBasedOnCurrentSessionAnalytics();
            }
            catch (Exception e)
            {
                // Ignore errors here
            }
        }
    }
}