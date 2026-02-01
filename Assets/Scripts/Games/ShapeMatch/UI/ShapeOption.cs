using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace SenAware.ShapeMatch
{
    [RequireComponent(typeof(CanvasGroup), typeof(Button))]
    public class ShapeOption : MonoBehaviour
    {
        [SerializeField] private Image shapeImage;
        [SerializeField] private Color incorrectOptionFadeColor = Color.gray;
        [SerializeField] private GameObject extendedTouchArea;
        
        [Header("CORRECT TWEEN SETTINGS")]
        [SerializeField] private float punchScale = 0.8f;
        [SerializeField] private float punchDuration = 0.3f;
        
        [Header("INCORRECT TWEEN SETTINGS")]
        [SerializeField] private float fadeDuration = 0.25f;
        [SerializeField] private float fadeTargetAlpha = 0.7f;
        [SerializeField] private float shakeDuration = 0.5f;
        [SerializeField] private float shakeStrength = 10f;
        [SerializeField] private int shakeVibrato = 1;
        [SerializeField] private float shakeRandomness = 1f;
        
        [Header("SFX")]
        [SerializeField] private AudioClip correctSFX;
        [SerializeField] private AudioClip incorrectSFX;
        
        private CanvasGroup _canvasGroup;
        private Button _button;
        private Action<ShapesSO, bool> _onClickAction;
        
        private ShapesSO _shapeSo;
        private bool _isAlreadyClicked = false;
        private bool _isCorrectOption = false;
        private Vector3 _ogScale;
        
        private void Awake()
        {
            TryGetComponent(out _canvasGroup);
            TryGetComponent(out _button);
            CommonMethods.ToggleCanvasGroup(_canvasGroup, false);
            _button.onClick.AddListener(OnOptionClicked);
            
            _ogScale = shapeImage.transform.localScale;
            shapeImage.transform.localScale = Vector3.zero;
        }
        
        public void SetShapeOption(ShapesSO shapeSo, 
            bool isCorrectOption, 
            float spawnDelay,
            float fadeInDuration,
            bool useExtendedTouchArea,
            Action<ShapesSO, bool> onClickAction)
        {
            _shapeSo = shapeSo;
            _isCorrectOption = isCorrectOption;
            _onClickAction = onClickAction;
            shapeImage.sprite = shapeSo.shapeSprite;
            extendedTouchArea.SetActive(useExtendedTouchArea);
            
            // Fade in and scale up with delay
            var tween = DOTween.Sequence()
                .AppendInterval(spawnDelay)
                .AppendCallback(() =>
                {
                    CommonMethods.ToggleCanvasGroup(_canvasGroup, true, fadeInDuration);
                    shapeImage.transform.DOScale(_ogScale, fadeInDuration).SetEase(Ease.OutBack);
                });
        }
        
        private void OnOptionClicked()
        {
            _onClickAction?.Invoke(_shapeSo, _isCorrectOption);
            GlobalStatic.OnSFXRequested?.Invoke(_isCorrectOption ? correctSFX : incorrectSFX, true);
            
            if (_isAlreadyClicked)
            {
                return;
            }
            _isAlreadyClicked = true;

            if (_isCorrectOption)
            {
                // Do punch scale
                shapeImage.transform.DOKill();
                shapeImage.transform.DOPunchScale(_ogScale * punchScale, punchDuration, 1, 0f);
            }
            else
            {
                shapeImage.color = incorrectOptionFadeColor;
                CommonMethods.FadeCanvasGroup(_canvasGroup, 0.5f, fadeDuration);
            
                // Shake gently using DoTween
                shapeImage.transform.DOKill();
                shapeImage.transform.DOShakePosition(shakeDuration, new Vector3(shakeStrength, 0f, 0f), shakeVibrato, shakeRandomness, false, true);
            }
            
        }
    }
}