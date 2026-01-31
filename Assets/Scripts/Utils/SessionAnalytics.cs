using System;
using System.Collections.Generic;
using UnityEngine;

namespace SenAware
{
    [Serializable]
    public class SessionAnalytics
    {
        public string gameID;
        public List<SingleRoundAnalytics> gameRounds = new List<SingleRoundAnalytics>();
        public int adaptiveChangesMade;
        public int inattentiveWarnings;
    }
    
    [Serializable]
    public class SingleRoundAnalytics
    {
        public int roundNumber;
        public int timeTaken;
        public int timeToFirstInteraction;
        public int timeToSuccessfulInteraction;
        public int numberOfMistakes;
        public int repeatedInteractionsWithSameObject;
        public int inactiveTime;
        public DifficultyLevel difficultyLevel;
    }
    
    [Serializable]
    public enum DifficultyLevel
    {
        Easy,
        Medium,
        Hard
    }
}
