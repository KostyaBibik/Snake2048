using System.Linq;
using Services.Impl;
using UnityEngine;

namespace Components.Boxes.States.Impl
{
    public class BoxMoveState : IBoxState
    {
        private readonly BoxService _boxService;
        private readonly AxisInputContext _moveContext;
        private float speed = 5f;
        
        public BoxMoveState(
            BoxService boxService,
            GameInputManager inputManager
        )
        {
            _boxService = boxService;
            _moveContext = inputManager.GetContext<MovementContext>();
        }
    
        public void EnterState(BoxContext context)
        {
        }

        public void UpdateState(BoxContext context)
        {
            var playerView = _boxService.Boxes.FirstOrDefault(box => box.isPlayer);
            if (playerView == null)
                return;
            
            var movement = new Vector3(_moveContext.Value.x, 0, _moveContext.Value.y);
            if (movement != Vector3.zero)
            {
                playerView.transform.Translate(movement * (speed * Time.deltaTime), Space.World); 
                var targetRotation = Quaternion.LookRotation(movement);
                playerView.transform.rotation = Quaternion.Slerp(playerView.transform.rotation, targetRotation, speed * Time.deltaTime);
            }
        }

        public void ExitState(BoxContext context)
        {
            
        }
    }
}