using System;

namespace LocalizationSystem.Components
{
    public class FormattableLocalizationTextTMP : LocalizationTextTMP
    {
        private string _deferredValue;

        public event Action TranslatedEvent;

        //for update not static text: example- Current day {0}  SetValue(1) -> Current day 1  
        public void SetValue(string value)
        {
            if (!Translated)
            {
                _deferredValue = value;
                TranslatedEvent += SetValueWhenTranslated;
            }
            else _textElement.text = string.Format(DefaultText, value);
        }

        public void SetValue(int value) => SetValue(value.ToString());

        protected override void SetTranslate(string translatedText)
        {
            TranslatedEvent?.Invoke();
        }

        protected void SetValueWhenTranslated()
        {
            TranslatedEvent -= SetValueWhenTranslated;
            SetValue(_deferredValue);
        }
    }
}