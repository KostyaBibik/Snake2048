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
        private readonly BoxEntityFactory _boxEntityFactory;
        private readonly BoxStateFactory _boxStateFactory;
        private readonly GameSettingsConfig _gameSettingsConfig;
        private readonly SignalBus _signalBus;
        private readonly BoxView _boxToMerge;
        private readonly EBoxGrade _targetGrade;

        private float _mergeSpeed;
        private float _distanceForMerge;

        public BoxMergeState(
            BoxService boxService,
            BoxEntityFactory boxEntityFactory,
            BoxStateFactory boxStateFactory,
            GameSettingsConfig gameSettingsConfig,
            SignalBus signalBus,
            BoxView boxToMerge,
            EBoxGrade targetGrade
        )
        {
            _boxService = boxService;
            _boxEntityFactory = boxEntityFactory;
            _boxStateFactory = boxStateFactory;
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
            var boxTransform = context.BoxView.transform;
            var targetBoxTransform = _boxToMerge.transform;
            var distance = Vector3.Distance(boxTransform.position, targetBoxTransform.position);
            if (distance > _distanceForMerge)
            {
                var boxPos = boxTransform.position;
                var targetBoxPos = targetBoxTransform.position;
                var direction = (targetBoxPos - boxPos).normalized;
                
                boxPos += direction * (_mergeSpeed * Time.deltaTime);
                boxTransform.position = boxPos;

                var lookDirection = (targetBoxPos - boxPos).normalized;
                lookDirection.y = 0;
                if (lookDirection != Vector3.zero)
                {
                    var targetRotation = Quaternion.LookRotation(lookDirection);
                    boxTransform.rotation = Quaternion.Slerp(boxTransform.rotation, targetRotation, _mergeSpeed * Time.deltaTime);
                }
            }
            else
            {
                MergeBoxes(context.BoxView, _boxToMerge, _targetGrade);
            }
        }

        public void ExitState(BoxContext context)
        {
        }

        private void MergeBoxes(BoxView box1, BoxView box2, EBoxGrade targetGrade)
        {
            var newGrade = (EBoxGrade)((int)targetGrade + 1);
            var mergePosition = (box1.transform.position + box2.transform.position) / 2;

            var newBox = _boxEntityFactory.Create(newGrade);
            newBox.transform.position = mergePosition;
            
            if (box2.isPlayer)
            {
                newBox.isPlayer = box2.isPlayer;

                _signalBus.Fire(new CameraUpdateSignal
                {
                    followTarget = newBox.transform
                });
            }
            
            var team = _boxService.GetTeam(box1);
            _boxService.AddBoxToTeam(team[0], newBox);

            _boxService.RemoveEntity(box1);
            _boxService.RemoveEntity(box2);

            team = _boxService.GetTeam(newBox);

            if (team.Count > 1)
            {
                var lastBoxInTeam = team[^2];
                var state = _boxStateFactory.CreateFollowState(lastBoxInTeam.transform);
                newBox.stateContext.SetState(state);
            }
            else
            {
                var state = _boxStateFactory.CreateMoveState();
                newBox.stateContext.SetState(state);
            }
            
            _signalBus.Fire(new MergeBoxSignal
            {
                mergingBox = newBox
            });
        }
    }
}
