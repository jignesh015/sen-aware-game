using System;
using UnityEngine;
using UnityEngine.UI;

namespace SenAware
{
    [RequireComponent(typeof(Button))]
    public class ToggleBgmButton : MonoBehaviour
    {
        [SerializeField] private GameObject bgmOffIndicator;

        private void Start()
        {
            if (TryGetComponent(out Button button))
            {
                button.onClick.AddListener(ToggleBgm);
            }

            bgmOffIndicator.SetActive(!CommonMethods.IsMusicEnabled());
        }

        private void ToggleBgm()
        {
            var bgmEnabled = CommonMethods.IsMusicEnabled();
            bgmOffIndicator.SetActive(bgmEnabled);
            GlobalStatic.OnMusicToggleChanged?.Invoke(!bgmEnabled);
        }
    }
}