using Systems.Action;
using Components.Boxes.States;
using Components.Boxes.States.Impl;
using Components.Boxes.Views.Impl;
using Database;
using Enums;
using Helpers;
using Infrastructure.Pools.Impl;
using Services.Impl;
using UnityEngine;
using Zenject;

namespace Infrastructure.Factories.Impl
{
    public class BoxStateFactory : IStateFactory
    {
        private readonly GameInputManager _inputManager;
        private readonly BotService _botService;
        private readonly GameSettingsConfig _gameSettingsConfig;
        private readonly BotsSettingsConfig _botsSettingsConfig;
        private readonly GameSceneHandler _sceneHandler;
        private readonly Camera _camera;
        private readonly SignalBus _signalBus;

        private BoxService _boxService;
        private BoxPool _boxPool;
        private AccelerationBoxSystem _accelerationBoxSystem;

        public BoxStateFactory(
            BotService botService,
            GameInputManager inputManager,
            GameSettingsConfig gameSettingsConfig,
            BotsSettingsConfig botsSettingsConfig,
            GameSceneHandler sceneHandler,
            Camera camera,
            SignalBus signalBus
        )
        {
            _botService = botService;
            _inputManager = inputManager;
            _sceneHandler = sceneHandler;
            _gameSettingsConfig = gameSettingsConfig;
            _botsSettingsConfig = botsSettingsConfig;
            _camera = camera;
            _signalBus = signalBus;
        }

        [Inject]
        private void Construct(
            BoxService boxService,
            BoxPool boxPool,
            AccelerationBoxSystem accelerationBoxSystem)
        {
            _boxService = boxService;
            _boxPool = boxPool;
            _accelerationBoxSystem = accelerationBoxSystem;
        }
        
        public IBoxState CreateFollowState(Transform leader, float leaderMeshOffset)
        {
            return new BoxFollowState(_gameSettingsConfig, leader, leaderMeshOffset);
        }

        public IBoxState CreateIdleState()
        {
            return new BoxIdleState();
        }

        public IBoxState CreateMoveState()
        {
            return new BoxMoveState(_inputManager, _gameSettingsConfig, _camera);
        }

        public IBoxState CreateMergeState(BoxView boxToMerge, EBoxGrade targetGrade)
        {
            return new BoxMergeState(
                _boxService,
                _botService,
                _boxPool,
                _gameSettingsConfig,
                boxToMerge,
                targetGrade,
                _signalBus
            );
        }

        public IBoxState CreateBotMoveState()
        {
            return new BotMoveState(
                _boxService,
                _gameSettingsConfig,
                _sceneHandler,
                _accelerationBoxSystem,
                _botsSettingsConfig
            );
        }
    }
}