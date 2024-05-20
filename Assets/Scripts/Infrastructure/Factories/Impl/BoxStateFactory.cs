using Components.Boxes.States;
using Components.Boxes.States.Impl;
using Enums;
using Services.Impl;
using UnityEngine;
using Views.Impl;
using Zenject;

namespace Infrastructure.Factories.Impl
{
    public class BoxStateFactory : IStateFactory
    {
        private readonly GameInputManager _inputManager;
        private readonly BoxService _boxService;
        private readonly BoxEntityFactory _boxEntityFactory;
        private readonly SignalBus _signalBus;
        
        public BoxStateFactory(
            BoxService boxService,
            GameInputManager inputManager,
            BoxEntityFactory boxEntityFactory,
            SignalBus signalBus
            )
        {
            _boxService = boxService;
            _inputManager = inputManager;
            _boxEntityFactory = boxEntityFactory;
            _signalBus = signalBus;
        }
        
        public IBoxState CreateFollowState(Transform leader)
        {
            return new BoxFollowState(leader);
        }

        public IBoxState CreateIdleState()
        {
            return new BoxIdleState();
        }

        public IBoxState CreateMoveState()
        {
            return new BoxMoveState(_boxService, _inputManager);
        }

        public IBoxState CreateMergeState(BoxView boxToMerge, EBoxGrade targetGrade)
        {
            return new BoxMergeState(
                _boxService,
                _boxEntityFactory,
                this,
                _signalBus,
                boxToMerge,
                targetGrade
            );
        }
    }
}