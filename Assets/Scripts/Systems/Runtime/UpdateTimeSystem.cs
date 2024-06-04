using Enums;
using Services;
using UI.Top;
using UISystem;
using UnityEngine;
using Zenject;

namespace Systems.Runtime
{
    public class UpdateTimeSystem : ITickable, IInitializable
    {
        private readonly GameMatchService _gameMatchService;
        private TopWindow _topWindow;
        private float _time;
        private TopWindowModel _topWindowModel;

        public UpdateTimeSystem(GameMatchService gameMatchService)
        {
            _gameMatchService = gameMatchService;
        }
        
        public void Initialize()
        {
            _topWindow = UIManager.Instance.GetUIElement<TopWindow>();
            _topWindowModel = new TopWindowModel();
        }

        public void Tick()
        {
            if(!_gameMatchService.IsGameRunning())
                return;
            
            _time += Time.deltaTime;
            
            _topWindowModel.CurrentTime = (int)_time;
            _topWindow.InvokeUpdateView(_topWindowModel);
        }
    }
}