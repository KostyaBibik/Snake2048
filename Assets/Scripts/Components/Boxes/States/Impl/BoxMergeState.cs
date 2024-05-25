using Database;
using Enums;
using Infrastructure.Factories.Impl;
using Services.Impl;
using Signals;
using UnityEngine;
using Views.Impl;
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
        private float _distanceForMerge;

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
            _distanceForMerge = _gameSettingsConfig.DistanceForMerge;
        }

        public void UpdateState(BoxContext context)
        {
            if(_boxToMerge == null || _boxToMerge.isDestroyed)
                return;
            
            var boxTransform = context.BoxView.transform;
            var targetBoxTransform = _boxToMerge.transform;
            var distance = Vector3.Distance(boxTransform.position, targetBoxTransform.position);
            if (distance > _distanceForMerge)
            {
                var boxPos = boxTransform.position;
                var targetBoxPos = targetBoxTransform.position;
                var direction = (targetBoxPos - boxPos).normalized;

                var speedTranslate = _mergeSpeed * Time.deltaTime;
                boxPos += direction * speedTranslate;
                boxTransform.position = boxPos;

                var lookDirection = (targetBoxPos - boxPos).normalized;
                lookDirection.y = 0;
                if (lookDirection != Vector3.zero)
                {
                    var targetRotation = Quaternion.LookRotation(lookDirection);
                    boxTransform.rotation = Quaternion.Slerp(boxTransform.rotation, targetRotation, speedTranslate);
                }
            }
            else
            {
                MergeBoxes(context.BoxView, _boxToMerge, _targetGrade);
            }
        }

        private void MergeBoxes(BoxView box1, BoxView box2, EBoxGrade targetGrade)
        {
            var newGrade = (EBoxGrade)((int)targetGrade + 1);

            var newBox = _boxPool.GetBox(newGrade);
            newBox.transform.position = box2.transform.position;
            newBox.isPlayer = box1.isPlayer || box2.isPlayer;
            newBox.isBot = !newBox.isPlayer;
            
            if (newBox.isPlayer)
            {
                _signalBus.Fire(new CameraUpdateSignal
                {
                    followTarget = newBox.transform
                });
            }

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
