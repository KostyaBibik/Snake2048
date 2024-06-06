using System;
using System.Globalization;
using TMPro;
using UIKit.Elements;
using UIKit.Elements.Models;
using UnityEngine;

namespace UI.Top
{
    public struct TopWindowModel
    {
        public float CurrentTime;
        public int AddKills;
        public Action PauseGameCallback;
        public bool hidePause;
    }
    
    public class TopWindow : UIWindow<TopWindowModel>
    {
        [AutoSetupField] private TextMeshProUGUI _time;
        [AutoSetupField] private TextMeshProUGUI _kills;
        [AutoSetupField] private ButtonView _pause;
        [AutoSetupField] private ContainerBoostView _containerBoostView;

        private int _countKills;
        
        public override void Initialization()
        {
            _containerBoostView.BeginHide();
        }

        protected override void UpdateView(TopWindowModel model)
        {
            _time.text = FormatTime(model.CurrentTime);
            _countKills += model.AddKills;

            _kills.text = _countKills.ToString();
            
            var pauseCallback = model.PauseGameCallback;
            if(pauseCallback != null)
            {
                var pauseBtnModel = new ButtonModel();
                pauseBtnModel.ClickCallback = model.PauseGameCallback;
                _pause.InvokeUpdateView(pauseBtnModel);
                _pause.BeginShow();
            }
            
            if(model.hidePause)
            {
                _pause.BeginHide();
            }
        }
        
        private string FormatTime(float value)
        {
            var minutes = Mathf.FloorToInt(value / 60);
            var seconds = Mathf.FloorToInt(value % 60);
            return $"{minutes:00}:{seconds:00}";
        }
    }
}