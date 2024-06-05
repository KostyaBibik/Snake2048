using Enums;
using Helpers;
using Services;
using Signals;
using UI.Pause;
using UI.Top;
using UISystem;
using UniRx;
using Zenject;

namespace UI.InitStages
{
    public class InitTopWindowStage : IInitializable
    {
        private readonly SignalBus _signalBus;
        private readonly GameMatchService _gameMatchService;
        private TopWindow _topWindow;
        private PauseWindow _pauseWindow;

        public InitTopWindowStage(
            SignalBus signalBus,
            GameMatchService gameMatchService
        )
        {
            _signalBus = signalBus;
            _gameMatchService = gameMatchService;
        }
        
        public void Initialize()
        {
            _topWindow = UIManager.Instance.GetUIElement<TopWindow>();
            _pauseWindow = UIManager.Instance.GetUIElement<PauseWindow>();

            var topWindowModel = InitTopWindowModel();
            
            Observable.FromCoroutine(() => UiViewHelper.ActivateHandlerOnStartGame(_topWindow, _gameMatchService)).Subscribe();    
            
            _topWindow.InvokeUpdateView(topWindowModel);
            _topWindow.BeginHide();

            _signalBus.Subscribe<ChangeGameModeSignal>(OnChangeGameModeSignal);
        }

        private TopWindowModel InitTopWindowModel()
        {
            var topWindowModel = new TopWindowModel();
            topWindowModel.hidePause = false;
            topWindowModel.PauseGameCallback = () =>
            {
                _signalBus.Fire(new ChangeGameModeSignal { status = EGameModeStatus.Pause});
                _signalBus.Fire(new PlaySoundSignal { type = ESoundType.UiClick});
                _topWindow.BeginHide();
                _pauseWindow.BeginShow();
            };

            return topWindowModel;
        }
        
        private void OnChangeGameModeSignal(ChangeGameModeSignal signal)
        {
            if (signal.status != EGameModeStatus.Play)
            {
                var topWindowModel = new TopWindowModel();
                topWindowModel.hidePause = true;
                _topWindow.InvokeUpdateView(topWindowModel);
            }
            else
            {
                var topWindowModel = InitTopWindowModel();
                _topWindow.InvokeUpdateView(topWindowModel);             
            }
        }
    }
}