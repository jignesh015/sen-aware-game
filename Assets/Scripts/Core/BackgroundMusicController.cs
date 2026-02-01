using System;
using System.Collections;
using UnityEngine;

namespace SenAware
{
    [RequireComponent(typeof(AudioSource))]
    public class BackgroundMusicController : MonoBehaviour
    {
        private AudioSource _audioSource1;
        private AudioSource _audioSource2;
        
        public static BackgroundMusicController Instance { get; private set; }

        private void Awake()
        {
            if (Instance && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            // Unparent from any parent to persist across scenes
            transform.SetParent(null);
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            TryGetComponent(out _audioSource1);
            _audioSource2 = gameObject.AddComponent<AudioSource>();
            _audioSource1.loop = true;
            _audioSource2.loop = true;
            _audioSource2.volume = _audioSource1.volume;
            _audioSource2.outputAudioMixerGroup = _audioSource1.outputAudioMixerGroup;
            GlobalStatic.OnBGMRequested += OnBGMRequested;
            GlobalStatic.OnSessionStarted += OnSessionStarted;
        }
        
        private void OnDestroy()
        {
            GlobalStatic.OnBGMRequested -= OnBGMRequested;
            GlobalStatic.OnSessionStarted -= OnSessionStarted;
        }

        private async void OnSessionStarted()
        {
            await Awaitable.WaitForSecondsAsync(GlobalStatic.NewSceneLoadDelay);
            if(SessionManager.Instance && SessionManager.Instance.CurrentGameInfo)
            {
                OnBGMRequested(SessionManager.Instance.CurrentGameInfo.gameBGM);
            }
        }

        private void OnBGMRequested(AudioClip musicClip)
        {
            CrossfadeToNewClip(musicClip, GlobalStatic.BGMCrossfadeDuration);
        }
        
        private void CrossfadeToNewClip(AudioClip newClip, float duration)
        {
            if(!_audioSource1 || !_audioSource2 || !newClip )
                return;
            
            StopAllCoroutines();
            
            // Determine which source is currently playing and which will fade in
            var fadeOutSource = _audioSource1.isPlaying ? _audioSource1 : _audioSource2;
            var fadeInSource = fadeOutSource == _audioSource1 ? _audioSource2 : _audioSource1;

            // Setup the new clip on the fade-in source
            fadeInSource.clip = newClip;
            fadeInSource.volume = 0f;
            fadeInSource.Play();

            // Only crossfade if something is currently playing
            if (fadeOutSource.isPlaying)
            {
                StartCoroutine(PerformCrossfade(fadeOutSource, fadeInSource, duration));
            }
            else
            {
                // No music playing, just start the new clip
                fadeInSource.volume = 1f;
            }
        }

        private IEnumerator PerformCrossfade(AudioSource fadeOutSource, AudioSource fadeInSource, float duration)
        {
            var elapsedTime = 0f;
            var initialVolumeOut = fadeOutSource.volume;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                var progress = elapsedTime / duration;

                fadeOutSource.volume = Mathf.Lerp(initialVolumeOut, 0f, progress);
                fadeInSource.volume = Mathf.Lerp(0f, initialVolumeOut, progress);

                yield return null;
            }

            // Ensure final values are set
            fadeOutSource.volume = 0f;
            fadeInSource.volume = initialVolumeOut;
            fadeOutSource.Stop();
        }

    }
}