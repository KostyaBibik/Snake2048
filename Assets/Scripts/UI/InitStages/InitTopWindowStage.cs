using Enums;
using Signals;
using UI.Pause;
using UI.Top;
using UISystem;
using Zenject;

namespace UI.InitStages
{
    public class InitTopWindowStage : IInitializable
    {
        private readonly SignalBus _signalBus;
        
        public InitTopWindowStage(SignalBus signalBus)
        {
            _signalBus = signalBus;
        }
        
        public void Initialize()
        {
            var topWindow = UIManager.Instance.GetUIElement<TopWindow>();
            var pauseWindow =  UIManager.Instance.GetUIElement<PauseWindow>();
            var topWindowModel = new TopWindowModel();
            topWindowModel.PauseGameCallback = () =>
            {
                _signalBus.Fire(new ChangeGameModeSignal {status = EGameModeStatus.Pause});
                topWindow.BeginHide();
                pauseWindow.BeginShow();
            };
            topWindow.InvokeUpdateView(topWindowModel);
        }
    }
}