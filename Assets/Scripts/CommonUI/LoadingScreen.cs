using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SenAware
{
    public class LoadingScreen : MonoBehaviour
    {
        [SerializeField] private float animationDuration = 0.3f;
        [SerializeField] private float timeoutDuration = 12f;
        [SerializeField] private GameObject[] loadingAnimationFrames;

        private Coroutine _animationRoutine;
        private Coroutine _timeoutRoutine;

        private void OnEnable()
        {
            if (loadingAnimationFrames == null || loadingAnimationFrames.Length == 0)
                return;

            _animationRoutine = StartCoroutine(PlayLoadingAnimation());
            _timeoutRoutine = StartCoroutine(TimeoutWatcher());
        }

        private void OnDisable()
        {
            if (_animationRoutine != null)
            {
                StopCoroutine(_animationRoutine);
                _animationRoutine = null;
            }

            if (_timeoutRoutine != null)
            {
                StopCoroutine(_timeoutRoutine);
                _timeoutRoutine = null;
            }

            SetAllFramesInactive();
        }

        private IEnumerator PlayLoadingAnimation()
        {
            var frameDuration = animationDuration / loadingAnimationFrames.Length;
            var index = 0;

            SetAllFramesInactive();

            while (true)
            {
                SetAllFramesInactive();
                loadingAnimationFrames[index].SetActive(true);

                index = (index + 1) % loadingAnimationFrames.Length;
                yield return new WaitForSeconds(frameDuration);
            }
            // ReSharper disable once IteratorNeverReturns
        }

        private IEnumerator TimeoutWatcher()
        {
            yield return new WaitForSeconds(timeoutDuration);
            LoadingScreenTimedOut();
        }

        private void LoadingScreenTimedOut()
        {
            Debug.LogWarning("Loading screen timed out.");

            SceneManager.LoadScene(GlobalStatic.HomeScene);
        }

        private void SetAllFramesInactive()
        {
            foreach (var frame in loadingAnimationFrames)
            {
                frame.SetActive(false);
            }
        }
    }
}
