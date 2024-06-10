using LocalizationSystem.Data;
using LocalizationSystem.Data.KeyGeneration;
using LocalizationSystem.Main;
using UnityEngine;

namespace LocalizationSystem.Components
{
    public abstract class LocalizationTextBase : MonoBehaviour
    {
        [SerializeField] private LocalizationKey _localizationKey;

        private static LocalizationDictionary _allLocalizationsDict;
        private static FontHolder _overrideFontHolder;
        
        private bool _inited;

        public bool Translated { get; protected set; }
        public string DefaultText { get; protected set; }
        

        private void Start()
        {
            if (!_inited) Init();
        }

        public void TranslateByKey(LocalizationKey key)
        {
            _localizationKey = key;

            if (_inited == false) 
                Init();

            SetTextByKey();
        }

        public void TranslateByKey(string key)
        {
            if (_inited == false) 
                Init();

            SetTextByKey(key);
        }

        public static bool TryGetKey(LocalizationKey localizationKey, out string key)
        {
            return LocalizationKeys.Keys.TryGetValue(localizationKey, out key);
        }


        //example: DefaultText- "Music",  str- "Off", separator- ": " -> Music: Off

        public abstract void Concatenate(string str, string separator = " ");

        public static bool TryGetTranslatedText(LocalizationKey localizationKey, out string translated)
        {
            translated = "***";
            return LocalizationKeys.Keys.TryGetValue(localizationKey, out var key) &&
                   _allLocalizationsDict.Items.TryGetValue(key, out translated);
        }

        public static void ApplyLocalizationDictionary()
        {
            _allLocalizationsDict = Localization.LocalizationDictionary;
            if (Localization.TryGetFontHolder(out var fontHolder))
            {
                _overrideFontHolder = fontHolder;
            }
        }

        protected abstract void SetTranslate(string translatedText);
        protected abstract void OnHasNotTranslated(string key);

        protected void ApplyTranslate(string translatedText)
        {
            DefaultText = translatedText;
            Translated = true;
        }
        
        private void SetTextByKey()
        {
            if (TryGetKey(_localizationKey, out var key))
                SetTextByKey(key);
            else Debug.Log($"Localization key {_localizationKey} not founded");
        }

        private void SetTextByKey(string key)
        {
            if (_allLocalizationsDict.Items.TryGetValue(key, out var translatedText))
            {
                ApplyTranslate(translatedText);
                SetTranslate(translatedText);
            }
            else OnHasNotTranslated(key);
        }

        private void Init()
        {
            ApplyLocalizationDictionary();
            
            if (_localizationKey != LocalizationKey.None)
            {
                SetTextByKey();
            }
            else
            {
                var o = gameObject;
                // Debug.Log($"The key is missing when awake is called. " +
                //           $"Need to set the key in the inspector or via code." +
                //           $" If the text {o.transform.root.name}/.../" +
                //           $"{o.transform.parent.name}/{o.name} is correct, dont worry");
            }

            if (_overrideFontHolder)
            {
                SetFont(_overrideFontHolder);
            }
            _inited = true;
        }

        protected abstract void SetFont(FontHolder overrideFontHolder);
    }
}