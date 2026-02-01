using System;
using System.Collections.Generic;
using UnityEngine;

namespace SenAware
{
    [Serializable]
    public class SessionAnalytics
    {
        public string sessionID;
        public DateTime sessionStartTime;
        public string gameID;
        public int adaptiveChangesMade;
        public int inattentiveWarnings;
        public List<SingleRoundAnalytics> gameRounds = new List<SingleRoundAnalytics>();
    }
    
    [Serializable]
    public class SingleRoundAnalytics
    {
        public int roundNumber;
        public float timeTaken;
        public float timeToFirstInteraction;
        public float timeToSuccessfulInteraction;
        public int numberOfMistakes;
        public int repeatedInteractionsWithSameObject;
        public DifficultyLevel difficultyLevel;
    }

    [Serializable]
    public class SessionHistory
    {
        public string userID;
        public List<SessionAnalytics> sessions = new List<SessionAnalytics>();
    }

    [Serializable]
    public enum DifficultyLevel
    {
        Easy,
        Medium,
        Hard
    }
}
