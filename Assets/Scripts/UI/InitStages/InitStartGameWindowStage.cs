using Enums;
using Services;
using Signals;
using UI.StartWindow;
using UISystem;
using Zenject;

namespace UI.InitStages
{
    public class InitStartGameWindowStage : IInitializable
    {
        private readonly SignalBus _signalBus;
        private readonly GameMatchService _matchService;
        private StartGameWindow _startGameWindow;

        public InitStartGameWindowStage(
            SignalBus signalBus,
            GameMatchService matchService
        )
        {
            _signalBus = signalBus;
            _matchService = matchService;
        }
        
        public void Initialize()
        {
            _startGameWindow = UIManager.Instance.GetUIElement<StartGameWindow>();
            var startGameModel = new StartGameModel();
            startGameModel.startPlayCallback = InitStartGame;
            startGameModel.onEditNickname = OnUpdateNickText;
            _startGameWindow.InvokeUpdateView(startGameModel);
        }

        private void InitStartGame()
        {
            _startGameWindow.BeginHide();
            _signalBus.Fire(new ChangeGameModeSignal{ status = EGameModeStatus.Play });
        }

        private void OnUpdateNickText(string nick)
        {
            _matchService.playerNickname.Value = string.IsNullOrEmpty(nick) 
                ? "Player"
                : nick;
        }
    }
}