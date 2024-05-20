using System.Collections.Generic;
using Database;
using UnityEngine;

namespace Components.Boxes.States.Impl
{
    public class BoxFollowState : IBoxState
    {
        private float _speed;
        private float _followDistance;
        private Queue<Vector3> _positionHistory;

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
            _positionHistory = new Queue<Vector3>();
            _speed = _gameSettingsConfig.BoxMoveSpeed;
            _followDistance = _gameSettingsConfig.BoxFollowDistance;
            
            for (var i = 0; i < _historyLength; i++)
            {
                _positionHistory.Enqueue(_leader.position);
            }
        }

        public void UpdateState(BoxContext context)
        {
            if (_leader == null)
                return;
            
            if (_positionHistory.Count >= _historyLength)
            {
                _positionHistory.Dequeue();
            }
            _positionHistory.Enqueue(_leader.position);
            
            var targetPosition = _positionHistory.Peek();
            var boxTransform = context.BoxView.transform;
            
            float distanceToTarget = Vector3.Distance(boxTransform.position, targetPosition);
            if (distanceToTarget < _followDistance)
            {
                return;
            }
            
            var newPosition = Vector3.Lerp(boxTransform.position, targetPosition, _speed * Time.deltaTime);
            boxTransform.position = newPosition;
            
            var direction = (targetPosition - newPosition).normalized;
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