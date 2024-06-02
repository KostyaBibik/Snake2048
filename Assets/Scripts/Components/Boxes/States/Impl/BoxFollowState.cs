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
        
        private readonly GameSettingsConfig _gameSettingsConfig;
        private readonly Transform _leader;
        private readonly float _leaderMeshOffset;

        public BoxFollowState(
            GameSettingsConfig gameSettingsConfig,
            Transform leader,
            float leaderMeshOffset
        )
        {
            _leader = leader;
            _gameSettingsConfig = gameSettingsConfig;
            _leaderMeshOffset = leaderMeshOffset;
        }

        public void EnterState(BoxContext context)
        {
            _speed = _gameSettingsConfig.BoxMoveSpeed;
            _boostSpeed = _gameSettingsConfig.BoxBoostSpeed;
            _accelerationSpeed = _gameSettingsConfig.BoxAccelerationSpeed;
            _followDistance = _gameSettingsConfig.BoxFollowDistance + _leaderMeshOffset;
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
            
            var leaderPosition = _leader.position;
            var boxTransform = box.transform;
            var boxPos = boxTransform.position;
            var distanceToTarget = Vector3.Distance(boxPos, leaderPosition);
            
            var dynamicSpeed = Mathf.Lerp(0, _currentSpeed * 1.4f, distanceToTarget / _followDistance);
            
            var direction = (leaderPosition - boxPos).normalized;
            var relatedSpeed = dynamicSpeed * Time.deltaTime;
            var rb = box.Rigidbody;
            
            direction.y = 0;

            //rb.MovePosition(rb.position + direction * relatedSpeed);
            var targetVector = direction * relatedSpeed;
            targetVector.y = 0;

            rb.velocity = targetVector;

            if (direction == Vector3.zero)
                return;
            
            var targetRotation = Quaternion.LookRotation(direction);
            boxTransform.rotation = Quaternion.Slerp(boxTransform.rotation, targetRotation, relatedSpeed);
        }

        public void ExitState(BoxContext context)
        {
        }
    }
}