using System;
using Cinemachine;
using Components.Boxes.Views.Impl;
using DG.Tweening;
using Enums;
using Signals;
using UnityEngine;
using Zenject;

namespace Systems.Action
{
    public class CameraUpdateSystem : IInitializable, IDisposable, ITickable
    {
        private readonly CinemachineVirtualCamera _virtualCamera;
        private readonly SignalBus _signalBus;

        private float duration = 1.8f;
        private CinemachineTransposer _transposer;
        private Vector3 _baseOffset;
        private BoxView _followBox;
        
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
            _followBox = followBox;
            
            if(followBox == null || followBox.isDestroyed)
            {
                _virtualCamera.Follow = null;
                _virtualCamera.LookAt = null;
                _virtualCamera.enabled = false;
                return;
            }

            _virtualCamera.enabled = true;
            _followBox = followBox;
            
            var targetOffset = CalculateFollowOffset(followBox.Grade);
            
            DOTween.To(() => _transposer.m_FollowOffset, 
                x => _transposer.m_FollowOffset = x,
                targetOffset,
                duration);

            var followTarget = followBox.transform;
            _virtualCamera.Follow = followTarget;
            _virtualCamera.LookAt = followTarget;
        }

        private Vector3 CalculateFollowOffset(EBoxGrade grade)
        {
            var baseY = _baseOffset.y;
            var baseZ = _baseOffset.z;
            
            var yMultiplier = .5f;
            var zMultiplier = -.5f;

            var grades = Enum.GetValues(typeof(EBoxGrade));
            var gradeIndex = Array.IndexOf(grades, grade);
            
            var newY = baseY + gradeIndex * yMultiplier;
            var newZ = baseZ + gradeIndex * zMultiplier;

            return new Vector3(0, newY, newZ);
        }
        
        public void Dispose()
        {
            _signalBus.Unsubscribe<CameraUpdateSignal>(OnUpdateCamera);
        }

        public void Tick()
        {
            if(_virtualCamera.Follow == null 
               || _followBox == null
               || _followBox.isDestroyed
               )
            {
                _virtualCamera.Follow = null;
                _virtualCamera.LookAt = null;
                _virtualCamera.enabled = false;
            }
        }
    }
}