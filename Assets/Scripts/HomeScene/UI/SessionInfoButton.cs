using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SenAware.HomeScene
{
    [RequireComponent(typeof(Button)), RequireComponent(typeof(CanvasGroup))]
    public class SessionInfoButton : MonoBehaviour
    {
        [SerializeField] private TMP_Text sessionTimeText;
        [SerializeField] private TMP_Text sessionGameTitleText;
        [SerializeField] private Image buttonImage;
        [SerializeField] private Color highlightColor;
         
        private Button _button;
        private CanvasGroup _canvasGroup;
        private string _sessionId;
        private Action<string> _onClickAction;
        private Color _defaultColor;

        private void Awake()
        {
            TryGetComponent(out _button);
            TryGetComponent(out _canvasGroup);
            _button.onClick.AddListener(OnButtonClick);
            _defaultColor = buttonImage.color;
        }
        
        public void Initialize(string sessionId, string gameTitle, string sessionStartTime, Action<string> onClickAction)
        {
            _sessionId = sessionId;
            sessionGameTitleText.text = gameTitle;
            var dateTime = DateTime.Parse(
                sessionStartTime,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AdjustToUniversal
            );
            sessionTimeText.text = dateTime.ToString("d MMM, H:mm");
            _onClickAction = onClickAction;
        }

        public void HighlightButton(bool highlight)
        {
            buttonImage.color = highlight ? highlightColor : _defaultColor;
        }

        private void OnButtonClick()
        {
            _onClickAction?.Invoke(_sessionId);
        }
    }
}