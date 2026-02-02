using System;
using UnityEngine;
using DG.Tweening;

namespace SenAware
{
    [RequireComponent(typeof(CanvasGroup))]
    public class InattentiveWarningUI : MonoBehaviour
    {
        [SerializeField] private Transform warningTile;
        [SerializeField] private float hoverDuration = 1f;
        [SerializeField] private float hoverAmount = 10f;
        
        private CanvasGroup _canvasGroup;
        private Vector3 _initialWarningTilePosition;
        private bool _attentionCheckRequired = false;

        private void Awake()
        {
            TryGetComponent(out _canvasGroup);
            CommonMethods.ToggleCanvasGroup(_canvasGroup, false);
            GlobalStatic.OnAttentionStatusChanged += OnAttentionStatusChanged;
            GlobalStatic.OnRequestAttentionCheck += OnRequestAttentionCheck;
            
            _initialWarningTilePosition = warningTile.localPosition;
        }

        private void OnDestroy()
        {
            GlobalStatic.OnAttentionStatusChanged -= OnAttentionStatusChanged;
            GlobalStatic.OnRequestAttentionCheck -= OnRequestAttentionCheck;
        }

        private void OnRequestAttentionCheck(bool attentionCheckRequired)
        {
            _attentionCheckRequired = attentionCheckRequired;
           
            // If attention check is not required, hide the warning UI
            if (attentionCheckRequired) return;
            CommonMethods.ToggleCanvasGroup(_canvasGroup, false);
            warningTile.DOKill();
            warningTile.localPosition = _initialWarningTilePosition;
        }

        private void OnAttentionStatusChanged(bool isAttentive)
        {
            if (!_attentionCheckRequired) return;
            
            CommonMethods.ToggleCanvasGroup(_canvasGroup, !isAttentive, 0.2f);
            
            // If is inattentive, tween the warning tile up and down
            warningTile.DOKill();
            warningTile.localPosition = _initialWarningTilePosition;
            if (!isAttentive)
            {
                warningTile
                    .DOLocalMoveY(_initialWarningTilePosition.y + hoverAmount, hoverDuration)
                    .SetLoops(-1, LoopType.Yoyo)
                    .SetEase(Ease.InOutSine);
            }
        }
    }
}