using System;
using UIKit.Elements;
using UIKit.Elements.Models;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Pause
{
    public struct PauseWindowModel
    {
        public Action ContinuePlayCallback;
        public Action RestartCallback;
        public Action ChangeLanguageCallback;
        public Action<float> onChangeSoundsCallback;
        public Action<float> onChangeMusicCallback;

        public float baseSoundValue;
        public float baseMusicValue;
    }
    
    public class PauseWindow : UIWindow<PauseWindowModel>
    {
        [AutoSetupField] private ButtonView _continue;
        [AutoSetupField] private ButtonView _close;
        [AutoSetupField] private ButtonView _language;
        [AutoSetupField] private ButtonView _restart;
        [AutoSetupField] private Slider _soundsSlider;
        [AutoSetupField] private Slider _musicSlider;
        [AutoSetupField] private Image _soundActiveIcon;
        [AutoSetupField] private Image _soundDisableIcon;
        [AutoSetupField] private Image _musicActiveIcon;
        [AutoSetupField] private Image _musicDisableIcon;
        
        protected override void UpdateView(PauseWindowModel model)
        {
            _musicSlider.value = model.baseMusicValue;
            _soundsSlider.value = model.baseSoundValue;
            SetIcon(_musicActiveIcon.gameObject, _musicDisableIcon.gameObject, model.baseMusicValue);
            SetIcon(_soundActiveIcon.gameObject, _soundDisableIcon.gameObject, model.baseSoundValue);
            
            var continueBtnModel = new ButtonModel();
            continueBtnModel.ClickCallback = model.ContinuePlayCallback;
            _continue.InvokeUpdateView(continueBtnModel);
            _close.InvokeUpdateView(continueBtnModel);

            _soundsSlider.onValueChanged.AddListener(value =>
            {
                model.onChangeSoundsCallback?.Invoke(value);
                SetIcon(_soundActiveIcon.gameObject, _soundDisableIcon.gameObject, value);
            });
            
            
            _musicSlider.onValueChanged.AddListener(value =>
            {
                model.onChangeMusicCallback?.Invoke(value);
                SetIcon(_musicActiveIcon.gameObject, _musicDisableIcon.gameObject, value);
            });
            
            var restartBtnModel = new ButtonModel();
            restartBtnModel.ClickCallback = model.RestartCallback;
            _restart.InvokeUpdateView(restartBtnModel);
            
            var languageBtnModel = new ButtonModel();
            languageBtnModel.ClickCallback = model.ChangeLanguageCallback;
            _language.InvokeUpdateView(languageBtnModel);
        }

        private void SetIcon(GameObject enabledIcon, GameObject disabledIcon, float value)
        {
            if (value > 0f)
            {
                enabledIcon.gameObject.SetActive(true);
                disabledIcon.gameObject.SetActive(false);
            }
            else
            {
                disabledIcon.gameObject.SetActive(true);
                enabledIcon.gameObject.SetActive(false);
            }
        }
    }
}