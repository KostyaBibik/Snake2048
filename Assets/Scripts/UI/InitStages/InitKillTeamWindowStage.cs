using System;
using Signals;
using UISystem;
using Zenject;
using UI.KillTeamsWindow;
using UnityEngine;

namespace UI.InitStages
{
    public class InitKillTeamWindowStage : IInitializable, IDisposable
    {
        private readonly SignalBus _signalBus;
        private KillTeamWindow _killTeamWindow;

        public InitKillTeamWindowStage(SignalBus signalBus)
        {
            _signalBus = signalBus;
        }

        public void Initialize()
        {
            _killTeamWindow = UIManager.Instance.GetUIElement<KillTeamWindow>();
            _killTeamWindow.BeginShow();
            
            _signalBus.Subscribe<KillTeamSignal>(OnKillTeamSignal);
        }

        private void OnKillTeamSignal(KillTeamSignal signal)
        {
            var killTeamModel = new KillTeamsModel();
            killTeamModel.nickKiller = signal.killingTeamNickname;
            killTeamModel.nickDestroyed = signal.defeatedTeamNickname;
            _killTeamWindow.InvokeUpdateView(killTeamModel);
        }

        public void Dispose()
        {
            _signalBus.Unsubscribe<KillTeamSignal>(OnKillTeamSignal);
        }
    }
}