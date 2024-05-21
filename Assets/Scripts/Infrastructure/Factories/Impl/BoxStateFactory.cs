using Components.Boxes.States;
using Components.Boxes.States.Impl;
using Database;
using Enums;
using Helpers;
using Services.Impl;
using UnityEngine;
using Views.Impl;
using Zenject;

namespace Infrastructure.Factories.Impl
{
    public class BoxStateFactory : IStateFactory
    {
        private readonly GameInputManager _inputManager;
        private BoxService _boxService;
        private readonly BotService _botService;
        private readonly GameSettingsConfig _gameSettingsConfig;
        private readonly GameSceneHandler _sceneHandler;
        private readonly SignalBus _signalBus;

        private BoxEntityFactory _boxEntityFactory;

        public BoxStateFactory(
            BotService botService,
            GameInputManager inputManager,
            GameSettingsConfig gameSettingsConfig,
            GameSceneHandler sceneHandler,
            SignalBus signalBus
        )
        {
            _botService = botService;
            _inputManager = inputManager;
            _sceneHandler = sceneHandler;
            _gameSettingsConfig = gameSettingsConfig;
            _signalBus = signalBus;
        }

        [Inject]
        private void Construct(
            BoxEntityFactory boxEntityFactory,
            BoxService boxService
        )
        {
            _boxEntityFactory = boxEntityFactory;
            _boxService = boxService;
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
                _botService,
                _boxEntityFactory,
                _gameSettingsConfig,
                _signalBus,
                boxToMerge,
                targetGrade
            );
        }

        public IBoxState CreateBotMoveState()
        {
            return new BotMoveState(
                _boxService,
                _gameSettingsConfig,
                _sceneHandler
            );
        }
    }
}