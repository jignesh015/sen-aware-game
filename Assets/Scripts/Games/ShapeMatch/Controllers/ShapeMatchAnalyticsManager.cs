using System;
using System.Collections.Generic;
using UnityEngine;

namespace SenAware.ShapeMatch
{
    public class ShapeMatchAnalyticsManager : MonoBehaviour
    {
        private SingleRoundAnalytics _currentRoundAnalytics;
        private float _roundStartTime;
        private bool _roundHasInteraction = false;
        private int _numberOfMistakes = 0;
        private int _repeatedInteractionsCount = 0;
        private ShapesSO _lastTappedShape = null;
        private SessionManager _sessionManager;

        private void Awake()
        {
            _sessionManager = SessionManager.Instance;
            ShapeMatchStatic.OnShapeMatchRoundStarted += OnShapeMatchRoundStarted;
            ShapeMatchStatic.OnShapeMatchRoundEnded += OnShapeMatchRoundEnded;
            ShapeMatchStatic.OnShapeOptionTapped += OnShapeOptionTapped;
        }

        private void OnDestroy()
        {
            ShapeMatchStatic.OnShapeMatchRoundStarted -= OnShapeMatchRoundStarted;
            ShapeMatchStatic.OnShapeMatchRoundEnded -= OnShapeMatchRoundEnded;
            ShapeMatchStatic.OnShapeOptionTapped -= OnShapeOptionTapped;
        }

        private void OnShapeMatchRoundStarted(int roundNumber, ShapesSO targetShape, List<ShapesSO> optionShapes)
        {
            _roundStartTime = Time.time;
            _roundHasInteraction = false;
            _numberOfMistakes = 0;
            _repeatedInteractionsCount = 0;
            _lastTappedShape = null;

            _currentRoundAnalytics = new SingleRoundAnalytics
            {
                roundNumber = roundNumber,
                timeTaken = 0,
                timeToFirstInteraction = 0,
                timeToSuccessfulInteraction = 0,
                numberOfMistakes = 0,
                repeatedInteractionsWithSameObject = 0,
                difficultyLevel = DifficultyLevel.Medium // Default, may be set elsewhere
            };
        }

        private void OnShapeMatchRoundEnded()
        {
            _currentRoundAnalytics.timeTaken = Time.time - _roundStartTime;
            _currentRoundAnalytics.numberOfMistakes = _numberOfMistakes;
            _currentRoundAnalytics.repeatedInteractionsWithSameObject = _repeatedInteractionsCount;

            // Add the round analytics to the current session
            if (_sessionManager && _sessionManager.CurrentSessionAnalytics != null)
            {
                _sessionManager.CurrentSessionAnalytics.gameRounds.Add(_currentRoundAnalytics);
            }
        }

        private void OnShapeOptionTapped(ShapesSO tappedShape, bool isCorrect)
        {
            // Track first interaction
            if (!_roundHasInteraction)
            {
                _currentRoundAnalytics.timeToFirstInteraction = Time.time - _roundStartTime;
                _roundHasInteraction = true;
            }

            // Track correct interaction
            if (isCorrect)
            {
                _currentRoundAnalytics.timeToSuccessfulInteraction = Time.time - _roundStartTime;
            }
            else
            {
                _numberOfMistakes++;
            }

            // Track repeated interactions with same object
            if (_lastTappedShape == tappedShape)
            {
                _repeatedInteractionsCount++;
            }
            _lastTappedShape = tappedShape;
        }
    }
}