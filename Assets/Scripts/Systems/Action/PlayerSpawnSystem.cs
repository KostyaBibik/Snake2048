using System;
using Components.Boxes.States.Impl;
using Enums;
using Infrastructure.Factories.Impl;
using Services.Impl;
using Signals;
using UnityEngine;
using Zenject;

namespace Systems.Action
{
    public class PlayerSpawnSystem : IInitializable, IDisposable
    {
        private readonly BoxEntityFactory _boxEntityFactory;
        private readonly BoxStateFactory _boxStateFactory;
        private readonly BoxService _boxService;
        private readonly SignalBus _signalBus;

        private PlayerSpawnSystem(
            BoxEntityFactory boxEntityFactory,
            BoxStateFactory boxStateFactory,
            BoxService boxService,
            SignalBus signalBus
        )
        {
            _boxEntityFactory = boxEntityFactory;
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
            var boxView = _boxEntityFactory.Create(EBoxGrade.Grade_4);
            _boxService.RegisterNewTeam(boxView);

            var state = _boxStateFactory.CreateMoveState();
            boxView.stateContext.SetState(state);
            boxView.isPlayer = true;
            boxView.name = "Player";
        }

        public void Dispose()
        {
            _signalBus.Unsubscribe<PlayerSpawnSignal>(SpawnPlayer);
        }
    }
}