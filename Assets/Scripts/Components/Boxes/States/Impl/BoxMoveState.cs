using Database;
using Input.Context;
using UnityEngine;

namespace Components.Boxes.States.Impl
{
    public class BoxMoveState : IBoxState
    {
        private readonly Camera _camera;
        private readonly GameSettingsConfig _gameSettingsConfig;
        private readonly AxisInputContext _mouseLookContext;
        
        private float _currentSpeed;
        private float _boostSpeed;
        private float _accelerationSpeed;
        private Vector3 _currentDirection;
        
        private LayerMask _groundLayer;
        private float _speed;

        public BoxMoveState(
            GameInputManager inputManager,
            GameSettingsConfig gameSettingsConfig,
            Camera camera
        )
        {
            _mouseLookContext = inputManager.GetContext<MouseLookContext>();
            _gameSettingsConfig = gameSettingsConfig;
            _camera = camera;
        }

        public void EnterState(BoxContext context)
        {
            _groundLayer = LayerMask.GetMask("Ground");
            _speed = _gameSettingsConfig.BoxMoveSpeed;
            _boostSpeed = _gameSettingsConfig.BoxBoostSpeed;
            _accelerationSpeed = _gameSettingsConfig.BoxAccelerationSpeed;
        }

        public void UpdateState(BoxContext context)
        {
            var playerView = context.BoxView;

            if (playerView.IsSpeedBoosted)
            {
                _currentSpeed = _speed + _boostSpeed;
            } 
            else if (playerView.IsAccelerationActive)
            {
                _currentSpeed = _speed + _accelerationSpeed;
            }
            else
            {
                _currentSpeed = _speed;
            }

            var mousePos = _mouseLookContext.Value;
            var ray = _camera.ScreenPointToRay(mousePos);

            if (Physics.Raycast(ray, out var hit, Mathf.Infinity, _groundLayer))
            {
                var hitPoint = hit.point;
                var movementDirection = (hitPoint - playerView.transform.position).normalized;
                movementDirection.y = 0;
                
                var movementSummary = Mathf.Abs(movementDirection.x) + Mathf.Abs(movementDirection.z);
                var thresholdValue = 1f;

                if (movementSummary > thresholdValue)
                {
                    _currentDirection = movementDirection;
                }
                
                var relatedSpeed = _currentSpeed * Time.deltaTime;
                var rb = playerView.Rigidbody;
                rb.MovePosition(rb.position + _currentDirection * relatedSpeed);

                var boxTransform = playerView.transform;
                var targetRotation = Quaternion.LookRotation(_currentDirection);
                boxTransform.rotation = Quaternion.Slerp(boxTransform.rotation, targetRotation, relatedSpeed);
            }
        }

        public void ExitState(BoxContext context)
        {
        }
    }
}