using System;
using UnityEngine;
using FaceDetectionResult = Mediapipe.Tasks.Components.Containers.DetectionResult;

namespace SenAware
{
    public class AttentionDetector : MonoBehaviour
    {
        [SerializeField] private bool isUserAttentive;
        [SerializeField] private float inattentiveThreshold = 3f; // seconds
        
        private float _attentionLossTimer = 0f;
        private bool _isCurrentlyAttentive = true;
        
        private void Awake()
        {
            GlobalStatic.OnFaceDetectionResult += OnFaceDetectionResult;
        }

        private void OnDestroy()
        {
            GlobalStatic.OnFaceDetectionResult -= OnFaceDetectionResult;
        }

        private void OnFaceDetectionResult(FaceDetectionResult result)
        {
            isUserAttentive = result.detections is { Count: > 0 };
            
            if (isUserAttentive)
            {
                _attentionLossTimer = 0f;
                if (_isCurrentlyAttentive) return;
                _isCurrentlyAttentive = true;
                GlobalStatic.OnAttentionStatusChanged?.Invoke(true);
            }
            else
            {
                _attentionLossTimer += Time.deltaTime;
                if (!(_attentionLossTimer >= inattentiveThreshold) || !_isCurrentlyAttentive) return;
                _isCurrentlyAttentive = false;
                GlobalStatic.OnAttentionStatusChanged?.Invoke(false);
            }
        }
    }
}