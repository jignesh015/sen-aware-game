using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace SenAware.ShapeMatch
{
    public class ShapeMatchController : MonoBehaviour
    {
        [SerializeField] private GameObject loadingScreen;
        [SerializeField] private List<ShapeMatchData> shapeMatchDataList;
        
        private ShapeMatchData _currentShapeMatchDataData;
        private List<ShapesSO> _availableShapes;
        private int _currentRound = 0;
        private float _roundTimer;
        private bool _isRoundActive = false;
        private bool _hasGameStarted = false;

        private void Awake()
        {
            ShapeMatchStatic.OnDifficultyLevelSet += OnDifficultyLevelSet;
            ShapeMatchStatic.OnShapeOptionTapped += OnShapeOptionTapped;
            ShapeMatchStatic.OnGameCompleteContinueButtonTapped += OnGameCompleteContinueButtonTapped;
            GlobalStatic.OnQuitToHomeButtonPressed += OnQuitToHomeButtonPressed;
        }
        
        private void OnDestroy()
        {
            ShapeMatchStatic.OnDifficultyLevelSet -= OnDifficultyLevelSet;
            ShapeMatchStatic.OnShapeOptionTapped -= OnShapeOptionTapped;
            ShapeMatchStatic.OnGameCompleteContinueButtonTapped -= OnGameCompleteContinueButtonTapped;
            GlobalStatic.OnQuitToHomeButtonPressed -= OnQuitToHomeButtonPressed;
        }

        private void Start()
        {
            if(loadingScreen) loadingScreen.SetActive(false);
        }

        private void Update()
        {
            if (_isRoundActive)
            {
                _roundTimer -= Time.deltaTime;
                ShapeMatchStatic.OnRoundTimerUpdated?.Invoke(_roundTimer);
                
                if (_roundTimer <= 0)
                {
                    EndRound();
                }
            }
        }

        private void StartGame()
        {
            _currentShapeMatchDataData = shapeMatchDataList.Find(s => s.generalDifficultyLevel == ShapeMatchStatic.CurrentDifficultyLevel);
            if (!_currentShapeMatchDataData)
            {
                Debug.LogError($"No ShapeMatchData found for difficulty level: {ShapeMatchStatic.CurrentDifficultyLevel}");
                return;
            }
            
            _currentRound = 0;
            _hasGameStarted = true;
            
            // Initialize available shapes pool
            _availableShapes = new List<ShapesSO>(_currentShapeMatchDataData.shapes);
            ShapeMatchStatic.UseExtendedTouchAreas = _currentShapeMatchDataData.generalDifficultyLevel == DifficultyLevel.Easy;
            
            ShapeMatchStatic.OnShapeMatchGameStart?.Invoke(_currentShapeMatchDataData);
            StartNewRound(ShapeMatchStatic.CurrentDifficultyLevel);
        }
        
        private void StartNewRound(DifficultyLevel difficultyLevel)
        {
            // Check if we've completed all rounds
            if (_currentRound >= _currentShapeMatchDataData.totalRounds)
            {
                ShapeMatchStatic.OnShapeMatchGameEnd?.Invoke();
                return;
            }
            
            // If we've exhausted all shapes, replenish the pool
            if (_availableShapes.Count == 0)
            {
                _availableShapes = new List<ShapesSO>(_currentShapeMatchDataData.shapes);
            }
            
            
            var roundData = _currentShapeMatchDataData.adaptiveShapeMatchData
                .FirstOrDefault(ad => ad.roundSpecificDifficultyLevel == difficultyLevel);

            if (roundData == null)
            {
                Debug.LogError("No round data found for the specified difficulty level.");
                return;
            }
            
            // Randomly select a shape to match from available shapes
            var randomIndex = Random.Range(0, _availableShapes.Count);
            var shapeToMatch = _availableShapes[randomIndex];
            
            // Remove the selected shape from available pool to avoid repetition
            _availableShapes.RemoveAt(randomIndex);
            
            // Create options list (includes the correct shape plus random others)
            var options = new List<ShapesSO> { shapeToMatch };

            // Add random shapes for the remaining options
            var optionsNeeded = roundData.numOfOptions - 1;
            var otherShapes = _currentShapeMatchDataData.shapes.Where(s => s != shapeToMatch).ToList();
            
            for (var i = 0; i < optionsNeeded && otherShapes.Count > 0; i++)
            {
                var randIndex = Random.Range(0, otherShapes.Count);
                options.Add(otherShapes[randIndex]);
                otherShapes.RemoveAt(randIndex);
            }
            
            // Shuffle the options
            ShuffleList(options);
            
            ShapeMatchStatic.RoundTimerDuration = roundData.timePerRoundInSeconds;
            _roundTimer = roundData.timePerRoundInSeconds;
            _isRoundActive = true;
            
            // Invoke the round started event
            ShapeMatchStatic.OnShapeMatchRoundStarted?.Invoke(_currentRound, shapeToMatch, options);
        }
        
        private async void EndRound()
        {
            _isRoundActive = false;
            ShapeMatchStatic.OnShapeMatchRoundEnded?.Invoke();
            _currentRound++;

            await Awaitable.WaitForSecondsAsync(_currentShapeMatchDataData.timeBetweenRounds);
            
            StartNewRound(ShapeMatchStatic.CurrentDifficultyLevel);
        }
        
        private void ShuffleList<T>(List<T> list)
        {
            for (var i = list.Count - 1; i > 0; i--)
            {
                var randomIndex = Random.Range(0, i + 1);
                (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
            }
        }
        
        #region Event Handlers

        private void OnDifficultyLevelSet(DifficultyLevel level)
        {
            if (!_hasGameStarted)
            {
                StartGame();
            }
        }
        
        private void OnShapeOptionTapped(ShapesSO shapesSo, bool isCorrect)
        {
            if (!_isRoundActive) return;
            
            if (isCorrect)
            {
                Invoke(nameof(EndRound), 0.5f);
            }
        }

        private async void OnGameCompleteContinueButtonTapped()
        {
            if(loadingScreen) loadingScreen.SetActive(true);
            await Awaitable.WaitForSecondsAsync(1f);
            GlobalStatic.OnSessionEndRequested?.Invoke();
        }

        private void OnQuitToHomeButtonPressed()
        {
            if(loadingScreen) loadingScreen.SetActive(true);
        }
        #endregion
    }
}