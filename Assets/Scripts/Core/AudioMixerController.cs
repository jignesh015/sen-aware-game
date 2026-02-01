using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

namespace SenAware
{
    public class AudioMixerController : MonoBehaviour
    {
        [Header("Audio Mixer Settings")]
        [SerializeField] private AudioMixer mainAudioMixer;
        [SerializeField] private string bgmVolumeParameter = "BgmVolume"; 
        [SerializeField] private string sfxVolumeParameter = "SfxVolume"; 

        [Header("Volume Control")]
        [SerializeField] protected float fadeOutDuration = 0.1f;
        [SerializeField] protected float fadeInDuration = 2f;

        private float _currentBgmVolume = 0f;

        public static AudioMixerController Instance { get; private set; }
        protected void Awake()
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
            
            AssignListeners();
        }

        protected void Start()
        {
            SetVolume(bgmVolumeParameter, CommonMethods.IsMusicEnabled() ? 0f : -80f);
            SetVolume(sfxVolumeParameter, CommonMethods.IsSoundEnabled() ? 0f : -80f);
        }

        // Made generic: subclasses decide what to listen to
        private void AssignListeners()
        {
            GlobalStatic.OnMusicToggleChanged += OnMusicToggleChanged;
            GlobalStatic.OnSoundToggleChanged += OnSoundToggleChanged;
        }

        private void RemoveListeners()
        {
            GlobalStatic.OnMusicToggleChanged -= OnMusicToggleChanged;
            GlobalStatic.OnSoundToggleChanged -= OnSoundToggleChanged;
        }

        private void OnMusicToggleChanged(bool isEnabled)
        {
            PlayerPrefs.SetInt(GlobalStatic.MusicPrefKey, isEnabled ? 1 : 0);
            SetVolume(bgmVolumeParameter, isEnabled ? 0f : -80f);
        }

        private void OnSoundToggleChanged(bool isEnabled)
        {
            PlayerPrefs.SetInt(GlobalStatic.SoundPrefKey, isEnabled ? 1 : 0);
            SetVolume(sfxVolumeParameter, isEnabled ? 0f : -80f);
        }

        /// <summary>
        /// Set the volume of the mixer group in decibels
        /// </summary>
        /// <param name="mixerGroupParameter">The exposed parameter name in the Audio Mixer</param>
        /// <param name="volumeInDecibels">The volume level in decibels
        private void SetVolume(string volumeParameter, float volumeDb)
        {
            mainAudioMixer.SetFloat(volumeParameter, Mathf.Clamp(volumeDb, -80f, 20f));
        }

        /// <summary>
        /// Set the volume using a linear scale (0 to 1)
        /// </summary>
        /// <param name="volumeParameter">The exposed parameter name in the Audio Mixer</param>
        /// <param name="linearVolume">The volume level in linear scale (0 to 1)</param>
        private void SetVolumeLinear(string volumeParameter, float volumeLinear)
        {
            var volumeDb = LinearToDb(volumeLinear);
            SetVolume(volumeParameter, volumeDb);
        }

        /// <summary>
        /// Get the current volume in decibels
        /// </summary>
        private float GetVolume(string volumeParameter)
        {
            mainAudioMixer.GetFloat(volumeParameter, out var volumeDb);
            return volumeDb;
        }

        /// <summary>
        /// Fade volume over time
        /// </summary>
        public void FadeVolume(string volumeParameter, float targetVolumeDb, float duration)
        {
            StopAllCoroutines();
            StartCoroutine(FadeVolumeCoroutine(volumeParameter, targetVolumeDb, duration));
        }

        private IEnumerator FadeVolumeCoroutine(string volumeParameter, float targetVolumeDb, float duration)
        {
            var startVolume = GetVolume(volumeParameter);
            var elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                var t = elapsedTime / duration;
                var newVolume = Mathf.Lerp(startVolume, targetVolumeDb, t);
                SetVolume(volumeParameter, newVolume);
                yield return null;
            }

            SetVolume(volumeParameter, targetVolumeDb);
        }

        /// <summary>
        /// Convert decibels to linear scale (0-1)
        /// </summary>
        private float DbToLinear(float db)
        {
            return Mathf.Pow(10f, db / 20f);
        }

        /// <summary>
        /// Convert linear scale (0-1) to decibels
        /// </summary>
        private float LinearToDb(float linear)
        {
            if (linear <= 0f)
                return -80f;

            return 20f * Mathf.Log10(linear);
        }

        protected void OnDestroy()
        {
            RemoveListeners();
        }
    }
}