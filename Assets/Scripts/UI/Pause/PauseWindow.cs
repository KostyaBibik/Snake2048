using System;
using UIKit.Elements;
using UIKit.Elements.Models;
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
            var continueBtnModel = new ButtonModel();
            continueBtnModel.ClickCallback = model.ContinuePlayCallback;
            _continue.InvokeUpdateView(continueBtnModel);
            _close.InvokeUpdateView(continueBtnModel);

            _soundsSlider.onValueChanged.AddListener(value =>
            {
                model.onChangeSoundsCallback?.Invoke(value);
                if (value > 0f)
                {
                    _soundActiveIcon.gameObject.SetActive(true);
                    _soundDisableIcon.gameObject.SetActive(false);
                }
                else
                {
                    _soundDisableIcon.gameObject.SetActive(true);
                    _soundActiveIcon.gameObject.SetActive(false);
                }
            });
            
            _musicSlider.onValueChanged.AddListener(value =>
            {
                model.onChangeMusicCallback?.Invoke(value);
                if (value > 0f)
                {
                    _musicActiveIcon.gameObject.SetActive(true);
                    _musicDisableIcon.gameObject.SetActive(false);
                }
                else
                {
                    _musicDisableIcon.gameObject.SetActive(true);
                    _musicActiveIcon.gameObject.SetActive(false);
                }
            });
            
            var restartBtnModel = new ButtonModel();
            restartBtnModel.ClickCallback = model.RestartCallback;
            _restart.InvokeUpdateView(restartBtnModel);
            
            var languageBtnModel = new ButtonModel();
            languageBtnModel.ClickCallback = model.ChangeLanguageCallback;
            _language.InvokeUpdateView(languageBtnModel);

            _musicSlider.value = model.baseMusicValue;
            _soundsSlider.value = model.baseSoundValue;
        }
    }
}