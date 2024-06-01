using System;
using System.Collections.Generic;
using Components.Boxes.Views.Impl;
using Database;
using DG.Tweening;
using Enums;
using Services.Impl;
using Signals;
using Zenject;

namespace Systems.Action
{
    public class BoxBoostSystem : IInitializable, IDisposable
    {
        private readonly BoxService _boxService;
        private readonly GameSettingsConfig _settingsConfig;
        private readonly SignalBus _signalBus;
        private readonly Dictionary<int, Tween> _boostTimers = new Dictionary<int, Tween>();

        public BoxBoostSystem(
            BoxService boxService,
            GameSettingsConfig gameSettingsConfig,
            SignalBus signalBus
        )
        {
            _boxService = boxService;
            _settingsConfig = gameSettingsConfig;
            _signalBus = signalBus;
        }

        public void Initialize()
        {
            _signalBus.Subscribe<BoxBoostSignal>(OnGetBoostSignal);
            _signalBus.Subscribe<AddBoxToTeamSignal>(OnAddBoxToTeam);
        }

        private void OnGetBoostSignal(BoxBoostSignal signal)
        {
            switch (signal.boostType)
            {
                case EBoxBoost.Speed:
                    OnGetBoostSpeedSignal(signal.box);
                    break;
                default:
                    break;
            }
        }

        private void OnGetBoostSpeedSignal(BoxView box)
        {
            var boxTeam = _boxService.GetTeamByMember(box);
            var idTeam = boxTeam.GetId();

            if (_boostTimers.TryGetValue(idTeam, out var existingTimer))
            {
                existingTimer.Kill();
            }

            foreach (var boxTeamMember in boxTeam.Members)
            {
                boxTeamMember.IsSpeedBoosted = true;
            }

            var newTimer = DOVirtual.DelayedCall(4f, () =>
            {
                var team = _boxService.GetTeamById(idTeam);
                if (team == null)
                    return;

                foreach (var boxTeamMember in team.Members)
                {
                    boxTeamMember.IsSpeedBoosted = false;
                }

                _boostTimers.Remove(idTeam);
            });

            _boostTimers[idTeam] = newTimer;
        }

        private void OnAddBoxToTeam(AddBoxToTeamSignal signal)
        {
            var idTeam = signal.idTeam;
            var team = _boxService.GetTeamById(idTeam);

            if (_boostTimers.ContainsKey(idTeam))
            {
                foreach (var boxTeamMember in team.Members)
                {
                    boxTeamMember.IsSpeedBoosted = true;
                }
            }
        }

        public void Dispose()
        {
            _signalBus.Unsubscribe<BoxBoostSignal>(OnGetBoostSignal);

            foreach (var timer in _boostTimers.Values)
            {
                timer.Kill();
            }
            _boostTimers.Clear();
        }
    }
}
