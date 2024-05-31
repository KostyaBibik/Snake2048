using System;
using Enums;
using Signals;
using UnityEngine;
using Zenject;

namespace Services
{
    public class GameMatchService : IInitializable, IDisposable
    {
        private readonly SignalBus _signalBus;

        private EGameModeStatus _eGameModeStatus;
        public EGameModeStatus EGameModeStatus => _eGameModeStatus;
        
        public GameMatchService(SignalBus signalBus)
        {
            _signalBus = signalBus;
        }

        public void Initialize()
        {
            _signalBus.Subscribe<ChangeGameModeSignal>(OnChangeGameMode);
        }

        private void OnChangeGameMode(ChangeGameModeSignal signal)
        {
            _eGameModeStatus = signal.status;
        }
        
        public void Dispose()
        {
            _signalBus.Unsubscribe<ChangeGameModeSignal>(OnChangeGameMode);
        }
    }
}