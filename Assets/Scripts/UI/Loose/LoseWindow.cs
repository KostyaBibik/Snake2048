using System;
using UIKit.Elements;
using UIKit.Elements.Models;

namespace UI.Loose
{
    public struct LoseWindowModel
    {
        public Action restartCallback;
    }
    
    public class LoseWindow : UIWindow<LoseWindowModel>
    {
        [AutoSetupField] private ButtonView _restart;
        
        protected override void UpdateView(LoseWindowModel model)
        {
            var restartModel = new ButtonModel();
            restartModel.ClickCallback = model.restartCallback;
            _restart.InvokeUpdateView(restartModel);
        }
    }
}