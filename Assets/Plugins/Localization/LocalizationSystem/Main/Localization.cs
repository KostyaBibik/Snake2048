using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using LocalizationSystem.Data;
using Newtonsoft.Json;
using UnityEngine;

namespace LocalizationSystem.Main
{
    public static class Localization
    {
        private static LocalizationDictionary _localization;
        private static FontHolder _fontHolder;

        public static LocalizationDictionary LocalizationDictionary
        {
            get
            {
                if (_localization == null) 
                    throw new NotImplementedException($"LocalizationDictionary is empty, check _Boot.cs_ and SetLanguage()"); // TODO: Указать Boot скрипт
                return _localization;
            }
        }
        public static bool TryGetFontHolder(out FontHolder fontHolder)
        {
            fontHolder = _fontHolder;
            return _fontHolder != null;
        }

        public static bool SetLocalization(string lang)
        {
            IEnumerable<LocalizationData> allLocalizationData = Resources.LoadAll<LocalizationData>("LocalizationData");
            
            var localizationResource = allLocalizationData
                .FirstOrDefault(l => l.Yandexi18nLang.Equals(lang));
            
            if (localizationResource != null)
            {
                var localizationDictionary = JsonConvert.DeserializeObject<LocalizationDictionary>
                    (localizationResource.LocalizationJsonFile.text);
                
                _localization = localizationDictionary;
                _fontHolder = localizationResource.OverrideFontAsset;
                return true;
            }

            Debug.Log($"{nameof(localizationResource)} not founded");
            return false;
        }

        public static bool TryGetText(string key, out string translatedText)
        {
            return LocalizationDictionary.Items.TryGetValue(key, out translatedText);
        }
    }
}