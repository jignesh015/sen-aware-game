using UnityEngine;
using UnityEngine.UI;

namespace SenAware
{
    [RequireComponent(typeof(Button))]
    public class ButtonClickSFX : MonoBehaviour
    {
        [SerializeField] private int buttonClickId = 0;
        
        private void Awake()
        {
            if (TryGetComponent(out Button button))
            {
                button.onClick.AddListener(PlayClickSfx);
            }
        }
        
        private void PlayClickSfx()
        {
            GlobalStatic.OnButtonClickSFXRequested?.Invoke(buttonClickId);
        }
    }
}