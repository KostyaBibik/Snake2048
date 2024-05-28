using Enums;
using Signals;
using UI.Pause;
using UI.Top;
using UISystem;
using Zenject;

namespace UI.InitStages
{
    public class InitPauseMenuStage: IInitializable
    {
        private readonly SignalBus _signalBus;
        
        public InitPauseMenuStage(SignalBus signalBus)
        {
            _signalBus = signalBus;
        }
        
        public void Initialize()
        {
            var pauseWindow = UIManager.Instance.GetUIElement<PauseWindow>();
            var topWindow = UIManager.Instance.GetUIElement<TopWindow>();
            var pauseWindowModel = new PauseWindowModel();
            pauseWindowModel.ContinuePlayCallback = () =>
            {
                _signalBus.Fire(new ChangeGameModeSignal {status = EGameModeStatus.Play});
                pauseWindow.BeginHide();
                topWindow.BeginShow();
            };
            pauseWindow.InvokeUpdateView(pauseWindowModel);
            pauseWindow.BeginHide();
        }
    }
}