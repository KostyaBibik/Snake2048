using Components.Boxes.Views.Impl;
using Database;
using Enums;
using Helpers;
using Services.Impl;
using UnityEngine;
using Zenject;

namespace Components.Boxes.States.Impl
{
    public class BoxMergeState : IBoxState
    {
        private readonly BoxService _boxService;
        private readonly BotService _botService;
        private readonly BoxPool _boxPool;
        private readonly GameSettingsConfig _gameSettingsConfig;
        private readonly SignalBus _signalBus;
        private readonly BoxView _boxToMerge;
        private readonly EBoxGrade _targetGrade;

        private float _mergeSpeed;
        private float _currentSpeed;
        private float _boostSpeed;
        private float _distanceForMerge;
        private float _accelerationSpeed;

        public BoxMergeState(
            BoxService boxService,
            BotService botService,
            BoxPool boxPool,
            GameSettingsConfig gameSettingsConfig,
            SignalBus signalBus,
            BoxView boxToMerge,
            EBoxGrade targetGrade
        )
        {
            _boxService = boxService;
            _botService = botService;
            _boxPool = boxPool;
            _gameSettingsConfig = gameSettingsConfig;
            _boxToMerge = boxToMerge;
            _signalBus = signalBus;
            _targetGrade = targetGrade;
        }

        public void EnterState(BoxContext context)
        {
            _mergeSpeed = _gameSettingsConfig.BoxMoveSpeedOnMerge;
            _boostSpeed = _gameSettingsConfig.BoxBoostSpeed;
            _distanceForMerge = _gameSettingsConfig.DistanceForMerge;
            _accelerationSpeed = _gameSettingsConfig.BoxAccelerationSpeed;
        }

        public void UpdateState(BoxContext context)
        {
            if(_boxToMerge == null || _boxToMerge.isDestroyed)
                return;

            var box = context.BoxView;
            var boxTransform = box.transform;
            var boxPos = boxTransform.position;
            var targetBoxTransform = _boxToMerge.transform;
            var targetBoxPos = targetBoxTransform.position;

            boxPos.y = 0;
            targetBoxPos.y = 0;
            
            var distance = Vector3.Distance(boxPos, targetBoxPos);
            
            if (box.IsSpeedBoosted)
            {
                _currentSpeed = _mergeSpeed + _boostSpeed;
            } 
            else if (box.IsAccelerationActive)
            {
                _currentSpeed = _mergeSpeed + _accelerationSpeed;
            }
            else
            {
                _currentSpeed = _mergeSpeed;
            }
            
            if (distance > _distanceForMerge)
            {
                var direction = (targetBoxPos - boxPos).normalized;
                var relatedSpeed = _currentSpeed * Time.deltaTime;
                var rb = box.Rigidbody;
                
                rb.MovePosition(rb.position + direction * relatedSpeed);
                
                var lookDirection = (targetBoxPos - boxPos).normalized;
                lookDirection.y = 0;
                if (lookDirection != Vector3.zero)
                {
                    var targetRotation = Quaternion.LookRotation(lookDirection);
                    boxTransform.rotation = Quaternion.Slerp(boxTransform.rotation, targetRotation, relatedSpeed);
                }
            }
            else
            {
                MergeBoxes(context.BoxView, _boxToMerge, _targetGrade);
            }
        }

        private void MergeBoxes(BoxView box1, BoxView box2, EBoxGrade targetGrade)
        {
            var newGrade = targetGrade.Next();

            var newBox = _boxPool.GetBox(newGrade);
            newBox.transform.position = box2.transform.position;
            newBox.isPlayer = box1.isPlayer || box2.isPlayer;
            newBox.isBot = !newBox.isPlayer;
            
            if (newBox.isBot)
            {
                _botService.AddEntityOnService(newBox);
            }

            _boxService.AddBoxToTeam(box1, newBox);

            _botService.RemoveEntity(box1);
            _botService.RemoveEntity(box2);
            
            _boxService.RemoveEntity(box1);
            _boxService.RemoveEntity(box2);
            
            _boxService.UpdateTeamStates(newBox);
        }

        public void ExitState(BoxContext context)
        {
        }
    }
}
