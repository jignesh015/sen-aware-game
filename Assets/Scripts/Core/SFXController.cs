using System;
using UnityEngine;
using UnityEngine.Audio;

namespace SenAware
{
    public class SFXController : MonoBehaviour
    {
        [SerializeField] private AudioMixerGroup sfxAudioMixerGroup;
        
        [SerializeField] private AudioClip[] buttonClickSFXClips;

        public static SFXController Instance { get; private set; }
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
            
            GlobalStatic.OnSFXRequested += OnSFXRequested;
            GlobalStatic.OnButtonClickSFXRequested += OnButtonClickSFXRequested;
        }
        
        private void OnDestroy()
        {
            GlobalStatic.OnSFXRequested -= OnSFXRequested;
            GlobalStatic.OnButtonClickSFXRequested -= OnButtonClickSFXRequested;
        }

        private void OnSFXRequested(AudioClip audioClip, bool randomizePitch)
        {
            if (!audioClip) return;
            
            // Play one shot SFX
            var sfxGameObject = new GameObject("SFX_" + audioClip.name);
            var audioSource = sfxGameObject.AddComponent<AudioSource>();
            audioSource.outputAudioMixerGroup = sfxAudioMixerGroup;
            if (randomizePitch)
            {
                audioSource.pitch = UnityEngine.Random.Range(0.8f, 1.2f);
            }
            audioSource.volume = GlobalStatic.SFXVolume;
            audioSource.PlayOneShot(audioClip);
            Destroy(sfxGameObject, audioClip.length / audioSource.pitch);
        }

        private void OnButtonClickSFXRequested(int clipIndex)
        {
            if (buttonClickSFXClips.Length == 0)
                return;
            
            clipIndex = Mathf.Clamp(clipIndex, 0, buttonClickSFXClips.Length - 1);
            OnSFXRequested(buttonClickSFXClips[clipIndex], false);
        }
    }
}