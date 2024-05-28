using System;
using Cinemachine;
using DG.Tweening;
using Services;
using Signals;
using UnityEngine;
using Zenject;

namespace Systems.Action
{
    public class CameraUpdateSystem : IInitializable, IDisposable
    {
        private readonly CinemachineVirtualCamera _virtualCamera;
        private readonly SignalBus _signalBus;

        private float duration = 1.8f;
        private CinemachineTransposer _transposer;
        private Vector3 _baseOffset;
        
        public CameraUpdateSystem(
            CinemachineVirtualCamera virtualCamera,
            SignalBus signalBus
        )
        {
            _virtualCamera = virtualCamera;
            _signalBus = signalBus;
        }

        public void Initialize()
        {
            _transposer = _virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
            _baseOffset = _transposer.m_FollowOffset;
            
            _signalBus.Subscribe<CameraUpdateSignal>(OnUpdateCamera);
        }

        private void OnUpdateCamera(CameraUpdateSignal signal)
        {
            var followBox = signal.followBox;
            var grade = (int)followBox.Grade;
            var targetOffset = CalculateFollowOffset(grade);
            
            DOTween.To(() => _transposer.m_FollowOffset, x => _transposer.m_FollowOffset = x, targetOffset, duration);

            var followTarget = followBox.transform;
            _virtualCamera.Follow = followTarget;
            _virtualCamera.LookAt = followTarget;
        }

        private Vector3 CalculateFollowOffset(int grade)
        {
            var baseY = _baseOffset.y;
            var baseZ = _baseOffset.z;
            var yMultiplier = .25f;
            var zMultiplier = -.1f;

            var newY = baseY + grade * yMultiplier;
            var newZ = baseZ + grade * zMultiplier;

            return new Vector3(0, newY, newZ);
        }
        
        public void Dispose()
        {
            _signalBus.Unsubscribe<CameraUpdateSignal>(OnUpdateCamera);
        }
    }
}