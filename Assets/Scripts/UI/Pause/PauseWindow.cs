using System;
using UIKit.Elements;
using UIKit.Elements.Models;

namespace UI.Pause
{
    public struct PauseWindowModel
    {
        public Action ContinuePlayCallback;
    }
    
    public class PauseWindow : UIWindow<PauseWindowModel>
    {
        [AutoSetupField] private ButtonView _continue;
        
        protected override void UpdateView(PauseWindowModel model)
        {
            var continueBtnModel = new ButtonModel();
            continueBtnModel.ClickCallback = model.ContinuePlayCallback;
            _continue.InvokeUpdateView(continueBtnModel);
        }
    }
}