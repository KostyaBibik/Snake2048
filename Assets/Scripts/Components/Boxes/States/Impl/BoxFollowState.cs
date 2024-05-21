using System.Collections.Generic;
using Database;
using UnityEngine;

namespace Components.Boxes.States.Impl
{
    public class BoxFollowState : IBoxState
    {
        private float _speed;
        private float _followDistance;
        private List<Vector3> _positionHistory;
        private Vector3 _velocity = Vector3.zero;
        
        private readonly GameSettingsConfig _gameSettingsConfig;
        private readonly Transform _leader;

        private const int _historyLength = 10;

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
            _followDistance = _gameSettingsConfig.BoxFollowDistance;

            for (var i = 0; i < _historyLength; i++)
            {
                _positionHistory.Add(_leader.position);
            }
        }

        public void UpdateState(BoxContext context)
        {
            if (_leader == null)
                return;
            
            if (_positionHistory.Count >= _historyLength)
            {
                _positionHistory.RemoveAt(0);
            }
            _positionHistory.Add(_leader.position);
            
            var targetPosition = _positionHistory[0];
            var boxTransform = context.BoxView.transform;
            
            var distanceToTarget = Vector3.Distance(boxTransform.position, targetPosition);
            if (distanceToTarget < _followDistance)
            {
                targetPosition = Vector3.SmoothDamp(boxTransform.position, targetPosition, ref _velocity, 0.3f, _speed / 2);
            }
            else
            {
                targetPosition = Vector3.SmoothDamp(boxTransform.position, targetPosition, ref _velocity, 0.3f, _speed);
            }

            if (distanceToTarget < _followDistance)
            {
                targetPosition = boxTransform.position;
            }
            
            boxTransform.position = targetPosition;
            
            var direction = (targetPosition - boxTransform.position).normalized;
            direction.y = 0; 
            
            if (direction != Vector3.zero)
            {
                var targetRotation = Quaternion.LookRotation(direction);
                boxTransform.rotation = Quaternion.Slerp(boxTransform.rotation, targetRotation, _speed * Time.deltaTime);
            }
        }

        public void ExitState(BoxContext context)
        {
        }
    }
}