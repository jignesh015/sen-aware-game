using System;
using UnityEngine;

namespace SenAware
{
    public static class GlobalStatic
    {
        // Constants
        public const string HomeScene = "HomeScene";        
        public const float NewSceneLoadDelay = 1.5f;        
        
        // Player UI interactions
        public static Action<GameInfo> OnGameButtonPressed;
        public static Action OnPauseButtonPressed;
        public static Action OnResumeButtonPressed;
        public static Action OnQuitToHomeButtonPressed;
    }
}