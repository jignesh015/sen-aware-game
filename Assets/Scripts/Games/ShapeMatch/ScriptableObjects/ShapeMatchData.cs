using UnityEngine;

namespace SenAware
{
    [CreateAssetMenu(fileName = "Shape Match Data", menuName = "Games/Shape Match Data", order = 0)]
    public class ShapeMatchData : ScriptableObject
    {
        public DifficultyLevel generalDifficultyLevel;
        public ShapesSO[] shapes;
        public AdaptiveShapeMatchData[] adaptiveShapeMatchData;
        public int totalRounds = 5;
    }
    
    [System.Serializable]
    public class AdaptiveShapeMatchData
    {
        public DifficultyLevel roundSpecificDifficultyLevel;
        public int numOfOptions = 3;
        public int timePerRoundInSeconds = 90;
    }
}