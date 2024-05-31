using System;
using Enums;
using Helpers;
using Signals;
using UI.Loose;
using UISystem;
using Zenject;

namespace UI.InitStages
{
    public class InitLoseWindowStage : IInitializable, IDisposable
    {
        private readonly SignalBus _signalBus;
        private readonly SceneLoader _sceneLoader;

        private LoseWindow _loseWindow;
        
        public InitLoseWindowStage(
            SignalBus signalBus,
            SceneLoader sceneLoader)
        {
            _signalBus = signalBus;
            _sceneLoader = sceneLoader;
        }

        public void Initialize()
        {
            _loseWindow = UIManager.Instance.GetUIElement<LoseWindow>();
            var loseModel = new LoseWindowModel();
            loseModel.restartCallback = _sceneLoader.RestartScene; 
            _loseWindow.InvokeUpdateView(loseModel);
            _loseWindow.BeginHide();
            
            _signalBus.Subscribe<ChangeGameModeSignal>(OnChangeGameModeSignal);
        }

        private void OnChangeGameModeSignal(ChangeGameModeSignal signal)
        {
            if(signal.status != EGameModeStatus.Lose)
                return;

            _loseWindow.BeginShow();
        }

        public void Dispose()
        {
            _signalBus.Unsubscribe<ChangeGameModeSignal>(OnChangeGameModeSignal);
        }
    }
}