using Database;
using Input.Context;
using UnityEngine;

namespace Components.Boxes.States.Impl
{
    public class BoxMoveState : IBoxState
    {
        private readonly AxisInputContext _moveContext; 
        private readonly AxisInputContext _mouseLookContext;
        private readonly ButtonInputContext _mouseMovementContext;
        private readonly GameSettingsConfig _gameSettingsConfig;
        private readonly Camera _camera;
        
        private LayerMask _groundLayer;
        private float _speed;
        
        public BoxMoveState(
            GameInputManager inputManager,
            GameSettingsConfig gameSettingsConfig,
            Camera camera
        )
        {
            _moveContext = inputManager.GetContext<MovementContext>();
            _mouseMovementContext = inputManager.GetContext<MouseMovementContext>();
            _mouseLookContext = inputManager.GetContext<MouseLookContext>();
            _gameSettingsConfig = gameSettingsConfig;
            _camera = camera;
        }
    
        public void EnterState(BoxContext context)
        {
            _groundLayer = LayerMask.GetMask("Ground");
            _speed = _gameSettingsConfig.BoxMoveSpeed;
        }

        public void UpdateState(BoxContext context)
        {
            var playerView = context.BoxView;

            /*var movement = new Vector3(_moveContext.Value.x, 0, _moveContext.Value.y);
            if (movement == Vector3.zero)
            {
                return;
            }*/

            if (_mouseMovementContext.IsHold)
            {
                var mousePos = _mouseLookContext.Value;
                var ray = _camera.ScreenPointToRay(mousePos);
                
                if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _groundLayer))
                {
                    var hitPoint = hit.point;
                    var movement = (hitPoint - playerView.transform.position).normalized;
                    movement.y = 0; 
                    
                    var relatedSpeed = _speed * Time.deltaTime;
                    var rb = playerView.Rigidbody;
                    rb.MovePosition(rb.position + movement * relatedSpeed);

                    var boxTransform = playerView.transform;
                    var targetRotation = Quaternion.LookRotation(movement);
                    boxTransform.rotation = Quaternion.Slerp(boxTransform.rotation, targetRotation, relatedSpeed);
                }
            }
        }

        public void ExitState(BoxContext context)
        {
            
        }
    }
}