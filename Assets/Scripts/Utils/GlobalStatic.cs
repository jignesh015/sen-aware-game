using System;
using UnityEngine;

namespace SenAware
{
    public static class GlobalStatic
    {
        // Constants
        public const string HomeScene = "HomeScene";        
        public const float NewSceneLoadDelay = 1.5f;   
        public const float BGMCrossfadeDuration = 2f;   
        public const float SFXVolume = 1f;   
        public const string UserID = "DEMO_USER"; // Temporary user ID for demo purpose
        public static readonly string SessionHistoryFileName = $"session_history_{UserID}.json";
        
        // Player Pref Keys
        public const string MusicPrefKey = "MusicEnabled";
        public const string SoundPrefKey = "SoundEnabled";

        // Common UI interactions
        public static Action<GameInfo> OnGameButtonPressed;
        public static Action OnPauseButtonPressed;
        public static Action OnResumeButtonPressed;
        public static Action OnQuitToHomeButtonPressed;
        
        // Session Events
        public static Action OnSessionStarted;
        public static Action OnSessionEndRequested;
        
        // Music and Sound Events
        public static Action<bool> OnMusicToggleChanged;
        public static Action<bool> OnSoundToggleChanged;
        public static Action<AudioClip> OnBGMRequested;
        public static Action<AudioClip, bool> OnSFXRequested;
        public static Action<int> OnButtonClickSFXRequested;
        // bool indicates whether to randomize pitch
        
    }
}