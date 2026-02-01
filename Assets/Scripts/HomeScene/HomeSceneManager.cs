using System;
using UnityEngine;

namespace SenAware.HomeScene
{
    public class HomeSceneManager : MonoBehaviour
    {
        [SerializeField] private GameObject loadingScreen;
         
        private void Awake()
        {
            GlobalStatic.OnGameButtonPressed += OnGameButtonPressed;
        }

        private void OnDestroy()
        {
            GlobalStatic.OnGameButtonPressed -= OnGameButtonPressed;
        }

        #region Event Handlers

        private void OnGameButtonPressed(GameInfo gameInfo)
        {
            if (loadingScreen)
            {
                loadingScreen.SetActive(true);
            }
        }
        #endregion
    }
}