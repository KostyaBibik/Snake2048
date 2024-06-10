using System;
using Enums;
using Helpers;
using Kimicu.YandexGames;
using Services;
using Services.Impl;
using Signals;
using UI.Leaderboard;
using UI.Loose;
using UI.Top;
using UISystem;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace UI.InitStages
{
    public class InitLoseWindowStage : IInitializable, IDisposable
    {
        private readonly SignalBus _signalBus;
        private readonly SceneLoader _sceneLoader;
        private readonly PlayerDataService _playerDataService;
        private readonly GameSettingsService _gameSettingsService;
        private readonly BoxService _boxService;

        private LoseWindow _loseWindow;
        private TopWindow _topWindow;
        private LeaderboardWindow _leaderboardWindow;
        private bool _isShowingRewardAd;
        private bool _canShowAd = true;

        public InitLoseWindowStage(
            SignalBus signalBus,
            SceneLoader sceneLoader,
            PlayerDataService playerDataService,
            GameSettingsService gameSettingsService,
            BoxService boxService
        )
        {
            _signalBus = signalBus;
            _sceneLoader = sceneLoader;
            _playerDataService = playerDataService;
            _gameSettingsService = gameSettingsService;
            _boxService = boxService;
        }

        public void Initialize()
        {
            _loseWindow = UIManager.Instance.GetUIElement<LoseWindow>();
            _topWindow = UIManager.Instance.GetUIElement<TopWindow>();
            _leaderboardWindow = UIManager.Instance.GetUIElement<LeaderboardWindow>();

            _loseWindow.BeginHide();

            _signalBus.Subscribe<ChangeGameModeSignal>(OnChangeGameModeSignal);
        }

        private void OnChangeGameModeSignal(ChangeGameModeSignal signal)
        {
            if(signal.status != EGameModeStatus.Lose)
                return;
            
            var progress = _playerDataService.GetResultPlayerProgress();
            var loseModel = new LoseWindowModel();

            loseModel.restartCallback = Restart;
            
            loseModel.CurrentTotalKills = progress.CurrentTotalKills;
            loseModel.HighestTotalKills = progress.HighestTotalKills;
            
            loseModel.CurrentTotalScore = progress.CurrentTotalScore;
            loseModel.HighestTotalScore = progress.HighestTotalScore;
            
            loseModel.CurrentLeaderTime = progress.CurrentLeaderTime;
            loseModel.HighestLeaderTime = progress.HighestLeaderTime;

            loseModel.continueCallback = ShowRewardAd;
            
            _loseWindow.InvokeUpdateView(loseModel);
            _loseWindow.BeginShow();

            _topWindow.BeginHide();
            _leaderboardWindow.BeginHide();
            
            _playerDataService.SaveToCloudProgress();
        }

        private void ShowRewardAd()
        {
            if(!_isShowingRewardAd)
            {
                _isShowingRewardAd = true;
                
                Advertisement.ShowVideoAd(
                    OnOpenCallback,
                    OnRewardedAD,
                    OnCloseAD,
                    OnErrorAd
                );
            }
        }

        private void Restart()
        {
            if (_canShowAd && CheckForShowAD())
            {
                Advertisement.ShowInterstitialAd(
                    OnOpenCallback, 
                    OnCloseAD, 
                    OnErrorAd, 
                    OnOfflineAD
                );
                _canShowAd = false;
            }
            else
            {
                _gameSettingsService.AddCountPlays();
                _signalBus.Fire(new PlaySoundSignal { type = ESoundType.UiClick });
                _sceneLoader.RestartScene();   
            }
        }

        private bool CheckForShowAD()
        {
            var countPlays = _gameSettingsService.Data.CountPlays;

            Debug.Log($"countPlays: {countPlays}");

            return countPlays % 3 == 0;
        }
        
        private void OnRewardedAD()
        {
            var playerGrade = _boxService.GetHighestPlayerGrade();
            
            _signalBus.Fire(new ChangeGameModeSignal { status = EGameModeStatus.Play });
            _signalBus.Fire(new PlayerSpawnSignal { grade = playerGrade });
            
            _loseWindow.BeginHide();
            _topWindow.BeginShow();
            _leaderboardWindow.BeginShow();
        }

        private void OnOfflineAD()
        {
            OnCloseAD();
        }
        
        private void OnCloseAD()
        {
            _canShowAd = false;
            AudioListener.pause = false;
            Time.timeScale = 1;
            _isShowingRewardAd = false;
        }
        
        private void OnErrorAd(string ex)
        {
            Debug.Log($"OnErrorAd: {ex}");
        }
        
        private void OnOpenCallback()
        {
            EventSystem.current.SetSelectedGameObject(null);
            AudioListener.pause = true;
            Time.timeScale = 0;
        }

        public void Dispose()
        {
            _signalBus.Unsubscribe<ChangeGameModeSignal>(OnChangeGameModeSignal);
        }
    }
}