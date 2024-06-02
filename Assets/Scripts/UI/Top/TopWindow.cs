using System;
using System.Globalization;
using TMPro;
using UIKit.Elements;
using UIKit.Elements.Models;

namespace UI.Top
{
    public struct TopWindowModel
    {
        public float CurrentTime;
        public Action PauseGameCallback;
    }
    
    public class TopWindow : UIWindow<TopWindowModel>
    {
        [AutoSetupField] private TextMeshProUGUI _time;
        [AutoSetupField] private ButtonView _pause;
        [AutoSetupField] private ContainerBoostView _containerBoostView;
        
        public override void Initialization()
        {
            _containerBoostView.BeginHide();
        }

        protected override void UpdateView(TopWindowModel model)
        {
            _time.text = model.CurrentTime.ToString(CultureInfo.InvariantCulture);

            var pauseCallback = model.PauseGameCallback;
            if(pauseCallback != null)
            {
                var pauseBtnModel = new ButtonModel();
                pauseBtnModel.ClickCallback = model.PauseGameCallback;
                _pause.InvokeUpdateView(pauseBtnModel);
            }
        }
    }
}