using System;
using System.IO;
using Database.Progress;
using Helpers;
using Kimicu.YandexGames;
using Signals;
using UnityEngine;
using Zenject;

namespace Services
{
    public class PlayerDataService : IInitializable
    {
        private readonly SignalBus _signalBus;
        
        private PlayerProgress _playerProgress;
        private PlayerProgress _savedProgress;
        public PlayerProgress PlayerProgress => _playerProgress;

        public PlayerDataService(SignalBus signalBus)
        {
            _signalBus = signalBus;
        }

        public void Initialize()
        {
            _playerProgress = LoadProgress();
            
            ResetRuntimeParameters();
            InitSavedData();
            
            _signalBus.Subscribe<KillTeamSignal>(OnKillTeamSignal);
        }

        private void InitSavedData()
        {
            _savedProgress = new PlayerProgress();
            _savedProgress.HighestLeaderTime = _playerProgress.HighestLeaderTime;
            _savedProgress.HighestTotalScore = _playerProgress.HighestTotalScore;
            _savedProgress.HighestTotalKills = _playerProgress.HighestTotalKills;
        }

        private void ResetRuntimeParameters()
        {
            _playerProgress.CurrentLeaderTime = 0;
            _playerProgress.CurrentTotalScore = 0;
            _playerProgress.CurrentTotalKills = 0;
        }

        private void SaveProgress(PlayerProgress progress, bool saveToCloud = false)
        {
            Cloud.SetValue(
                ConstantsDataDictionary.Names.SaveFileName,
                progress,
                saveToCloud,
                (() => { Debug.Log("success save progress");}),
                ex => Debug.LogError($"Error saving progress: {ex}"));
        }

        public void SaveToCloudProgress()
        {
            var resultProgress = new PlayerProgress();
            
            resultProgress.CurrentLeaderTime = _playerProgress.CurrentLeaderTime;
            resultProgress.CurrentTotalKills = _playerProgress.CurrentTotalKills;
            resultProgress.CurrentTotalScore = _playerProgress.CurrentTotalScore;
            resultProgress.HighestLeaderTime =
                _playerProgress.CurrentLeaderTime > _savedProgress.HighestLeaderTime
                    ? _playerProgress.CurrentLeaderTime
                    : _savedProgress.HighestLeaderTime;
            
            resultProgress.HighestTotalKills =
                _playerProgress.CurrentTotalKills > _savedProgress.HighestTotalKills
                    ? _playerProgress.CurrentTotalKills
                    : _savedProgress.HighestTotalKills;
            
            resultProgress.HighestTotalScore = 
                _playerProgress.CurrentTotalScore > _savedProgress.HighestTotalScore
                    ? _playerProgress.CurrentTotalScore
                    : _savedProgress.HighestTotalScore;

            SaveProgress(resultProgress, true);
        }
        
        public PlayerProgress GetResultPlayerProgress()
        {
            var resultProgress = new PlayerProgress();
            
            resultProgress.CurrentLeaderTime = _playerProgress.CurrentLeaderTime;
            resultProgress.CurrentTotalKills = _playerProgress.CurrentTotalKills;
            resultProgress.CurrentTotalScore = _playerProgress.CurrentTotalScore;
            resultProgress.HighestLeaderTime = _savedProgress.HighestLeaderTime;
            resultProgress.HighestTotalKills = _savedProgress.HighestTotalKills;
            resultProgress.HighestTotalScore = _savedProgress.HighestTotalScore;
            
            return resultProgress;
        }

        private PlayerProgress LoadProgress()
        {
            var progressCloud = Cloud.GetValue(ConstantsDataDictionary.Names.SaveFileName, new PlayerProgress());
            
            return progressCloud;
        }

        private void OnKillTeamSignal(KillTeamSignal signal)
        {
            if(!signal.isPlayerKill)
                return;

            var currentKills = _playerProgress.CurrentTotalKills + 1;
            UpdateProgress(ConstantsDataDictionary.Names.CurrentTotalKills, currentKills);

            if (currentKills > _playerProgress.HighestTotalKills)
            {
                UpdateProgress(ConstantsDataDictionary.Names.HighestTotalKills, currentKills);
            }
        }

        public void UpdateTime(float time)
        {
            UpdateProgress(ConstantsDataDictionary.Names.CurrentLeaderTime, (int) time);
            
            if (time > _playerProgress.HighestLeaderTime)
            {
                UpdateProgress(ConstantsDataDictionary.Names.HighestLeaderTime, (int) time);
            }
        }
        
        public void UpdateTotalScore(int resultScore)
        {
            UpdateProgress(ConstantsDataDictionary.Names.CurrentTotalScore, resultScore);
            
            if (resultScore > _playerProgress.HighestTotalScore)
            {
                UpdateProgress(ConstantsDataDictionary.Names.HighestTotalScore, resultScore);
            }
        }
        
        private void UpdateProgress(string key, object value)
        {
            var property = typeof(PlayerProgress).GetProperty(key);
            if (property != null)
            {
                var convertedValue = Convert.ChangeType(value, property.PropertyType);
                property.SetValue(_playerProgress, convertedValue);
                SaveProgress(_playerProgress);
            }
            else
            {
                Debug.LogWarning("Unknown key: " + key);
            }
        }
    }
}