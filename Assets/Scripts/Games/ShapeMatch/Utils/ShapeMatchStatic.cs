using System;
using System.Collections.Generic;
using UnityEngine;

namespace SenAware.ShapeMatch
{
    public static class ShapeMatchStatic
    {
        #region REALTIME DATA
        public static float RoundTimerDuration;
        public static bool UseExtendedTouchAreas;
        public static DifficultyLevel CurrentDifficultyLevel;
        #endregion

        #region STATIC ACTIONS
        // Game Events
        public static Action<ShapeMatchData> OnShapeMatchGameStart; // Parameter: shape match data
        public static Action OnShapeMatchGameEnd;
        public static Action<int, ShapesSO, List<ShapesSO>> OnShapeMatchRoundStarted; // Parameters: round number, target shape, list of option shapes
        public static Action OnShapeMatchRoundEnded;
        public static Action<float> OnRoundTimerUpdated; // Parameter: time left in seconds
        public static Action<DifficultyLevel> OnDifficultyLevelSet; // Parameter: difficulty level
        
        // User Interactions
        public static Action<ShapesSO, bool> OnShapeOptionTapped; // Parameter: tapped shape option, isCorrect
        public static Action OnGameCompleteContinueButtonTapped;
        #endregion
    }
}