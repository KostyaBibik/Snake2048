using System;
using System.Collections;
using Enums;
using Helpers;
using Services;
using Signals;
using UI.Pause;
using UI.Top;
using UISystem;
using UniRx;
using UnityEngine;
using Zenject;

namespace UI.InitStages
{
    public class InitTopWindowStage : IInitializable
    {
        private readonly SignalBus _signalBus;
        private readonly GameMatchService _gameMatchService;

        public InitTopWindowStage(
            SignalBus signalBus,
            GameMatchService gameMatchService
        )
        {
            _signalBus = signalBus;
            _gameMatchService = gameMatchService;
        }
        
        public void Initialize()
        {
            var topWindow = UIManager.Instance.GetUIElement<TopWindow>();
            var pauseWindow =  UIManager.Instance.GetUIElement<PauseWindow>();
            var topWindowModel = new TopWindowModel();
            topWindowModel.PauseGameCallback = () =>
            {
                _signalBus.Fire(new ChangeGameModeSignal {status = EGameModeStatus.Pause});
                topWindow.BeginHide();
                pauseWindow.BeginShow();
            };
            
            Observable.FromCoroutine(() => UiViewHelper.ActivateHandlerOnStartGame(topWindow, _gameMatchService)).Subscribe();    
            
            topWindow.InvokeUpdateView(topWindowModel);
            topWindow.BeginHide();
        }
    }
}