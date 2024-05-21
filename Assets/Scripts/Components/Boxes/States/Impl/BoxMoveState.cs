using System.Linq;
using Database;
using Services.Impl;
using UnityEngine;

namespace Components.Boxes.States.Impl
{
    public class BoxMoveState : IBoxState
    {
        private readonly BoxService _boxService;
        private readonly AxisInputContext _moveContext;
        private readonly GameSettingsConfig _gameSettingsConfig;

        private float _speed;
        
        public BoxMoveState(
            BoxService boxService,
            GameInputManager inputManager,
            GameSettingsConfig gameSettingsConfig
        )
        {
            _boxService = boxService;
            _moveContext = inputManager.GetContext<MovementContext>();
            _gameSettingsConfig = gameSettingsConfig;
        }
    
        public void EnterState(BoxContext context)
        {
            _speed = _gameSettingsConfig.BoxMoveSpeed;
        }

        public void UpdateState(BoxContext context)
        {
            var playerView = _boxService.GetAllBoxes().FirstOrDefault(box => box.isPlayer);
            if (playerView == null)
                return;
            
            var movement = new Vector3(_moveContext.Value.x, 0, _moveContext.Value.y);
            if (movement == Vector3.zero)
                return;
            
            playerView.transform.Translate(movement * (_speed * Time.deltaTime), Space.World); 
            var targetRotation = Quaternion.LookRotation(movement);
            playerView.transform.rotation = Quaternion.Slerp(playerView.transform.rotation, targetRotation, _speed * Time.deltaTime);
        }

        public void ExitState(BoxContext context)
        {
            
        }
    }
}