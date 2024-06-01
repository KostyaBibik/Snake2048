using System.Collections.Generic;
using Database;
using UnityEngine;

namespace Components.Boxes.States.Impl
{
    public class BoxFollowState : IBoxState
    {
        private float _speed;
        private float _currentSpeed;
        private float _boostSpeed;
        private float _accelerationSpeed;
        private float _followDistance;
        private List<Vector3> _positionHistory;
        private Vector3 _velocity;
        
        private readonly GameSettingsConfig _gameSettingsConfig;
        private readonly Transform _leader;

        private const int _historyLength = 3;

        public BoxFollowState(
            GameSettingsConfig gameSettingsConfig,
            Transform leader
        )
        {
            _leader = leader;
            _gameSettingsConfig = gameSettingsConfig;
        }

        public void EnterState(BoxContext context)
        {
            _positionHistory = new List<Vector3>(_historyLength);
            _speed = _gameSettingsConfig.BoxMoveSpeed;
            _boostSpeed = _gameSettingsConfig.BoxBoostSpeed;
            _accelerationSpeed = _gameSettingsConfig.BoxAccelerationSpeed;
            _followDistance = _gameSettingsConfig.BoxFollowDistance;
            _velocity = Vector3.zero;

            for (var i = 0; i < _historyLength; i++)
            {
                _positionHistory.Add(_leader.position);
            }
        }

        public void UpdateState(BoxContext context)
        {
            if (_leader == null)
                return;

            var box = context.BoxView;
            
            if (box.IsSpeedBoosted)
            {
                _currentSpeed = _speed + _boostSpeed;
            } 
            else if (box.IsAccelerationActive)
            {
                _currentSpeed = _speed + _accelerationSpeed;
            }
            else
            {
                _currentSpeed = _speed;
            }
            
            if (_positionHistory.Count >= _historyLength)
            {
                _positionHistory.RemoveAt(0);
            }
            _positionHistory.Add(_leader.position);
            
            var targetPosition = _positionHistory[0];
            var boxTransform = box.transform;
            
            var distanceToTarget = Vector3.Distance(boxTransform.position, targetPosition);
            if (distanceToTarget > _followDistance)
            {
                var dynamicSpeed = Mathf.Lerp(_currentSpeed / 2, _currentSpeed * 2, distanceToTarget / _followDistance);

                targetPosition = Vector3.SmoothDamp(boxTransform.position, targetPosition, ref _velocity, 0.2f, dynamicSpeed);
            }
            else
            {
                targetPosition = boxTransform.position;
            }
            
            boxTransform.position = targetPosition;
            
            var direction = (_leader.position - boxTransform.position).normalized;
            direction.y = 0; 
            
            if (direction != Vector3.zero)
            {
                var targetRotation = Quaternion.LookRotation(direction);
                boxTransform.rotation = Quaternion.Slerp(boxTransform.rotation, targetRotation, _currentSpeed);
            }
        }

        public void ExitState(BoxContext context)
        {
        }
    }
}