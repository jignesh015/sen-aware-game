using System;
using UnityEngine;

namespace SenAware
{
    [CreateAssetMenu(fileName = "New Game", menuName = "Game Info", order = 0)]
    public class GameInfo : ScriptableObject
    {
        public string gameID;
        public string gameTitle;
        [TextArea(3, 10)]
        public string gameDescription;
        public Sprite gameThumbnail;
        public string gameSceneName;
        public GameAdaptiveDifficultyRules adaptiveDifficultyRules;
    }
    
    [Serializable]
    public class GameAdaptiveDifficultyRules
    {
        public float timeToSuccessfulInteractionLowerThreshold = 15f;
        public float timeToSuccessfulInteractionUpperThreshold = 45f;
        public int numberOfMistakesLowerThreshold = 2;
        public int numberOfMistakesUpperThreshold = 4;
        public int inAttentiveWarningsLowerThreshold = 2;
        public int inAttentiveWarningsUpperThreshold = 5;
    }
}