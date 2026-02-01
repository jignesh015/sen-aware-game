using System;
using UnityEngine;

namespace SenAware
{
    public static class GlobalStatic
    {
        // Constants
        public const string HomeScene = "HomeScene";        
        
        // Player UI interactions
        public static Action OnPauseButtonPressed;
        public static Action OnResumeButtonPressed;
        public static Action OnQuitToHomeButtonPressed;
    }
}