using System;
using Enums;
using Helpers;
using Kimicu.YandexGames;
using Services;
using Signals;
using UI.Loose;
using UISystem;
using UnityEngine;
using Zenject;

namespace UI.InitStages
{
    public class InitLoseWindowStage : IInitializable, IDisposable
    {
        private readonly SignalBus _signalBus;
        private readonly SceneLoader _sceneLoader;
        private readonly GameDataService _gameDataService;

        private LoseWindow _loseWindow;
        
        public InitLoseWindowStage(
            SignalBus signalBus,
            SceneLoader sceneLoader,
            GameDataService gameDataService
        )
        {
            _signalBus = signalBus;
            _sceneLoader = sceneLoader;
            _gameDataService = gameDataService;
        }

        public void Initialize()
        {
            _loseWindow = UIManager.Instance.GetUIElement<LoseWindow>();
            _loseWindow.BeginHide();

            _signalBus.Subscribe<ChangeGameModeSignal>(OnChangeGameModeSignal);
        }

        private void OnChangeGameModeSignal(ChangeGameModeSignal signal)
        {
            if(signal.status != EGameModeStatus.Lose)
                return;
            
            var progress = _gameDataService.GetResultPlayerProgress();
            var loseModel = new LoseWindowModel();
            
            loseModel.restartCallback = () =>
            {
                _signalBus.Fire(new PlaySoundSignal { type = ESoundType.UiClick});
                _sceneLoader.RestartScene();
            };
            
            loseModel.CurrentTotalKills = progress.CurrentTotalKills;
            loseModel.HighestTotalKills = progress.HighestTotalKills;
            
            loseModel.CurrentTotalScore = progress.CurrentTotalScore;
            loseModel.HighestTotalScore = progress.HighestTotalScore;
            
            loseModel.CurrentLeaderTime = progress.CurrentLeaderTime;
            loseModel.HighestLeaderTime = progress.HighestLeaderTime;
            
            _loseWindow.InvokeUpdateView(loseModel);
            _loseWindow.BeginShow();
            
            Cloud.SetValue( 
            ConstantsDataDictionary.Names.SaveFileName,
            progress, 
            true,
                () => { Debug.Log("Success Save on Cloud");}, 
                value => Debug.Log($"Failed Save on cloud: {value}"));
        }

        public void Dispose()
        {
            _signalBus.Unsubscribe<ChangeGameModeSignal>(OnChangeGameModeSignal);
        }
    }
}