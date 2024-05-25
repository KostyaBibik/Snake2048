using System;
using Enums;
using Infrastructure.Factories.Impl;
using Services.Impl;
using Signals;
using Zenject;

namespace Systems.Action
{
    public class PlayerSpawnSystem : IInitializable, IDisposable
    {
        private readonly BoxPool _boxPool;
        private readonly BoxStateFactory _boxStateFactory;
        private readonly BoxService _boxService;
        private readonly SignalBus _signalBus;

        private PlayerSpawnSystem(
            BoxPool boxPool,
            BoxStateFactory boxStateFactory,
            BoxService boxService,
            SignalBus signalBus
        )
        {
            _boxPool = boxPool;
            _boxStateFactory = boxStateFactory;
            _boxService = boxService;
            _signalBus = signalBus;
        }

        public void Initialize()
        {
            _signalBus.Subscribe<PlayerSpawnSignal>(SpawnPlayer);
        }

        private void SpawnPlayer(PlayerSpawnSignal signal)
        {
            var boxView = _boxPool.GetBox(EBoxGrade.Grade_4);
            _boxService.RegisterNewTeam(boxView);

            var state = _boxStateFactory.CreateMoveState();
            boxView.stateContext.SetState(state);
            boxView.isPlayer = true;
            boxView.SetNickname("Player");
            boxView.name = "Player";
            
            _signalBus.Fire(new CameraUpdateSignal
            {
                followTarget = boxView.transform
            });
        }

        public void Dispose()
        {
            _signalBus.Unsubscribe<PlayerSpawnSignal>(SpawnPlayer);
        }
    }
}