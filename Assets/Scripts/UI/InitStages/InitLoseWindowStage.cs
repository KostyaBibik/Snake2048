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
        private readonly BoxService _boxService;

        private LoseWindow _loseWindow;
        private TopWindow _topWindow;
        private LeaderboardWindow _leaderboardWindow;
        private bool _isShowingAd;
        
        public InitLoseWindowStage(
            SignalBus signalBus,
            SceneLoader sceneLoader,
            PlayerDataService playerDataService,
            BoxService boxService
        )
        {
            _signalBus = signalBus;
            _sceneLoader = sceneLoader;
            _playerDataService = playerDataService;
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

            loseModel.continueCallback = ShowAd;
            
            _loseWindow.InvokeUpdateView(loseModel);
            _loseWindow.BeginShow();

            _topWindow.BeginHide();
            _leaderboardWindow.BeginHide();
            
            _playerDataService.SaveToCloudProgress();
        }

        private void ShowAd()
        {
            if(!_isShowingAd)
            {
                _isShowingAd = true;
                
                Advertisement.ShowVideoAd(
                    OnOpenCallback,
                    OnRewardedAD,
                    OnCloseAD,
                    OnErrorAd
                );
            }
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

        private void OnCloseAD()
        {
            AudioListener.pause = false;
            Time.timeScale = 1;
            _isShowingAd = false;
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