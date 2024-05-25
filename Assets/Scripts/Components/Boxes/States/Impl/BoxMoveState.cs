using Database;
using UnityEngine;

namespace Components.Boxes.States.Impl
{
    public class BoxMoveState : IBoxState
    {
        private readonly AxisInputContext _moveContext;
        private readonly GameSettingsConfig _gameSettingsConfig;

        private float _speed;
        
        public BoxMoveState(
            GameInputManager inputManager,
            GameSettingsConfig gameSettingsConfig
        )
        {
            _moveContext = inputManager.GetContext<MovementContext>();
            _gameSettingsConfig = gameSettingsConfig;
        }
    
        public void EnterState(BoxContext context)
        {
            _speed = _gameSettingsConfig.BoxMoveSpeed;
        }

        public void UpdateState(BoxContext context)
        {
            var playerView = context.BoxView;
            
            var movement = new Vector3(_moveContext.Value.x, 0, _moveContext.Value.y);
            if (movement == Vector3.zero)
                return;

            var relatedSpeed = _speed * Time.deltaTime;
            var boxTransform = playerView.transform;
            boxTransform.Translate(movement * relatedSpeed, Space.World); 
            var targetRotation = Quaternion.LookRotation(movement);
            boxTransform.rotation = Quaternion.Slerp(boxTransform.rotation, targetRotation, relatedSpeed);
        }

        public void ExitState(BoxContext context)
        {
            
        }
    }
}