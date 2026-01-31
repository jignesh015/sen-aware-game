using System;
using System.Collections.Generic;
using UnityEngine;

namespace SenAware
{
    public static class ShapeMatchStatic
    {
        // Game Events
        public static Action<ShapeMatchData> OnShapeMatchGameStart; // Parameter: shape match data
        public static Action OnShapeMatchGameEnd;
        public static Action<ShapesSO, List<ShapesSO>> OnShapeMatchRoundStarted; // Parameters: target shape, list of option shapes
        public static Action OnShapeMatchRoundEnded;
        public static Action<int> OnRoundTimerUpdated; // Parameter: time left in seconds
        
        // User Interactions
        public static Action<ShapesSO, bool> OnShapeOptionTapped; // Parameter: tapped shape option, isCorrect
        

    }
}