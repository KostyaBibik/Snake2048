using System;
using DG.Tweening;
using TMPro;
using UIKit.Elements;
using UIKit.Elements.Models;
using UISystem;
using UnityEngine;

namespace UI.StartWindow
{
    public class StartGameModel
    {
        public Action startPlayCallback;
        public Action<string> onEditNickname;
        public string savedNickname;
    }

    public class StartGameWindow : UIElementView<StartGameModel>
    {
        [SerializeField] private float fadeDuration = 1.0f;
        
        [AutoSetupField] private ButtonView _bg;
        [AutoSetupField] private TMP_InputField _inputNick;
        [AutoSetupField] private TextMeshProUGUI _labelStartGame;
        
        private Sequence _fadeSequence;
        
        protected override void UpdateView(StartGameModel model)
        {
            var startPlayModel = new ButtonModel();
            startPlayModel.ClickCallback = model.startPlayCallback;
            _bg.InvokeUpdateView(startPlayModel);

            _inputNick.onValueChanged.AddListener(text => model?.onEditNickname.Invoke(text));
            _inputNick.text = model.savedNickname;

            FadeAnimationTitle();
        }

        protected override void OnHideEnd()
        {
            _fadeSequence?.Kill();
            
            base.OnHideEnd();
        }

        private void FadeAnimationTitle()
        {
            _fadeSequence = DOTween.Sequence()
                .Append(_labelStartGame.DOFade(0, fadeDuration).SetEase(Ease.InOutSine))
                .Append(_labelStartGame.DOFade(1, fadeDuration).SetEase(Ease.InOutSine))
                .SetLoops(-1, LoopType.Yoyo); 
        }
    }
}