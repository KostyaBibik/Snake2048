using Components.Boxes.States;
using Components.Boxes.States.Impl;
using Database;
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
        private readonly GameSettingsConfig _gameSettingsConfig;
        private readonly SignalBus _signalBus;

        private BoxEntityFactory _boxEntityFactory;

        public BoxStateFactory(
            BoxService boxService,
            GameInputManager inputManager,
            GameSettingsConfig gameSettingsConfig,
            SignalBus signalBus
        )
        {
            _boxService = boxService;
            _inputManager = inputManager;
            _signalBus = signalBus;
            _gameSettingsConfig = gameSettingsConfig;
        }

        [Inject]
        private void Construct(BoxEntityFactory boxEntityFactory)
        {
            _boxEntityFactory = boxEntityFactory;
        }
        
        public IBoxState CreateFollowState(Transform leader)
        {
            return new BoxFollowState(_gameSettingsConfig, leader);
        }

        public IBoxState CreateIdleState()
        {
            return new BoxIdleState();
        }

        public IBoxState CreateMoveState()
        {
            return new BoxMoveState(_boxService, _inputManager, _gameSettingsConfig);
        }

        public IBoxState CreateMergeState(BoxView boxToMerge, EBoxGrade targetGrade)
        {
            return new BoxMergeState(
                _boxService,
                _boxEntityFactory,
                this,
                _gameSettingsConfig,
                _signalBus,
                boxToMerge,
                targetGrade
            );
        }
    }
}