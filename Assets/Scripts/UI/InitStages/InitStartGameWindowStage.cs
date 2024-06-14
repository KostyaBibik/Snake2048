using System;
using Enums;
using Kimicu.YandexGames;
using Services;
using Signals;
using UI.StartWindow;
using UISystem;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace UI.InitStages
{
    public class InitStartGameWindowStage : IInitializable
    {
        private readonly SignalBus _signalBus;
        private readonly GameMatchService _matchService;
        private readonly GameSettingsService _gameSettingsService;

        private StartGameWindow _startGameWindow;

        public InitStartGameWindowStage(
            SignalBus signalBus,
            GameMatchService matchService,
            GameSettingsService gameSettingsService
        )
        {
            _signalBus = signalBus;
            _matchService = matchService;
            _gameSettingsService = gameSettingsService;
        }

        public void Initialize()
        {
            YandexGamesSdk.GameReady();
            
            _startGameWindow = UIManager.Instance.GetUIElement<StartGameWindow>();
            var savedNickname = _gameSettingsService.Data.PlayerNickname;
            var startGameModel = new StartGameModel();
            startGameModel.startPlayCallback = InitStartGame;
            startGameModel.onEditNickname = OnUpdateNickText;
            startGameModel.savedNickname = savedNickname;
            _startGameWindow.InvokeUpdateView(startGameModel);
        }
        
        private void InitStartGame()
        {
            _startGameWindow.BeginHide();
            _signalBus.Fire(new ChangeGameModeSignal{ status = EGameModeStatus.Play });
            _signalBus.Fire(new PlaySoundSignal { type = ESoundType.UiClick});
        }

        private void OnUpdateNickText(string nick)
        {
            _matchService.playerNickname.Value = string.IsNullOrEmpty(nick) 
                ? "Player"
                : nick;
            _gameSettingsService.UpdatePlayerNickname(nick);
        }
    }
}