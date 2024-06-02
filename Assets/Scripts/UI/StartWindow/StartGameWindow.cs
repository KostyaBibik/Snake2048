using System;
using TMPro;
using UIKit.Elements;
using UIKit.Elements.Models;
using UISystem;

namespace UI.StartWindow
{
    public class StartGameModel
    {
        public Action startPlayCallback;
        public Action<string> onEditNickname;
    }
    
    public class StartGameWindow : UIElementView<StartGameModel>
    {
        [AutoSetupField] private ButtonView _bg;
        [AutoSetupField] private TMP_InputField _inputNick;
        
        protected override void UpdateView(StartGameModel model)
        {
            var startPlayModel = new ButtonModel();
            startPlayModel.ClickCallback = model.startPlayCallback;
            _bg.InvokeUpdateView(startPlayModel);

            _inputNick.onValueChanged.AddListener(text => model.onEditNickname.Invoke(text));
        }
    }
}