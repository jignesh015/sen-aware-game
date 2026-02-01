using System;
using System.Collections.Generic;
using UnityEngine;

namespace SenAware.ShapeMatch
{
    [CreateAssetMenu(fileName = "Shape Match Data", menuName = "Games/ShapeMatch/Shape Match Data", order = 1)]
    public class ShapeMatchData : ScriptableObject
    {
        public DifficultyLevel generalDifficultyLevel;
        public int totalRounds = 5;
        public float timeBetweenRounds = 1.5f;
        public List<ShapesSO> shapes;
        public List<AdaptiveShapeMatchData> adaptiveShapeMatchData;
    }
    
    [Serializable]
    public class AdaptiveShapeMatchData
    {
        public DifficultyLevel roundSpecificDifficultyLevel;
        public int numOfOptions = 3;
        public int timePerRoundInSeconds = 90;
    }
}