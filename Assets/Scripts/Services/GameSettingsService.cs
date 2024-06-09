using System;
using Database.Cloud;
using Helpers;
using Kimicu.YandexGames;
using UnityEngine;
using Zenject;

namespace Services
{
    public class GameSettingsService : IInitializable
    {
        private GameSettingsData _data;
        public GameSettingsData Data => _data ?? LoadData();
        
        public void Initialize()
        {
            _data ??= LoadData();
            SaveProgress(_data);
        }

        private GameSettingsData LoadData()
        {
            var dataCloud = Cloud.GetValue(ConstantsDataDictionary.GameSettings.SaveFileName, new GameSettingsData());
            
            return dataCloud;
        }
        
        public void UpdateSoundVolume(float value)
        {
            UpdateValue(ConstantsDataDictionary.GameSettings.SoundVolume, value);
        }
        
        public void UpdateMusicVolume(float value)
        {
            UpdateValue(ConstantsDataDictionary.GameSettings.MusicVolume, value);
        }
        
        private void UpdateValue(string key, object value)
        {
            var property = typeof(GameSettingsData).GetProperty(key);
            if (property != null)
            {
                var convertedValue = Convert.ChangeType(value, property.PropertyType);
                property.SetValue(_data, convertedValue);
                SaveProgress(_data);
            }
            else
            {
                Debug.LogWarning("Unknown key: " + key);
            }
        }
        
        private void SaveProgress(GameSettingsData data, bool saveToCloud = true)
        {
            Cloud.SetValue(
                ConstantsDataDictionary.GameSettings.SaveFileName,
                data,
                saveToCloud,
                null,
                ex => Debug.LogError($"Error saving progress: {ex}"));
        }
    }
}