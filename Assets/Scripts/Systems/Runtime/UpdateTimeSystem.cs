using Services;
using Signals;
using UI.Top;
using UISystem;
using UnityEngine;
using Zenject;

namespace Systems.Runtime
{
    public class UpdateTimeSystem : ITickable, IInitializable
    {
        private readonly GameMatchService _gameMatchService;
        private readonly PlayerDataService _playerDataService;
        private readonly SignalBus _signalBus;

        private TopWindow _topWindow;
        private TopWindowModel _topWindowModel;
        private float _time;
        private float _timeSinceLastUpdate;
        private float _timePlayerLead;
        private bool _isPlayerLead;
        
        public UpdateTimeSystem(
            GameMatchService gameMatchService,
            PlayerDataService playerDataService,
            SignalBus signalBus
        )
        {
            _gameMatchService = gameMatchService;
            _playerDataService = playerDataService;
            _signalBus = signalBus;
        }
        
        public void Initialize()
        {
            _topWindow = UIManager.Instance.GetUIElement<TopWindow>();
            _topWindowModel = new TopWindowModel();
            
            _signalBus.Subscribe<LeaderboardUpdateSignal>(OnUpdateLeaderBoard);
        }

        private void OnUpdateLeaderBoard(LeaderboardUpdateSignal signal)
        {
            _isPlayerLead = signal.elementModels[0].isPlayer;
        }

        public void Tick()
        {
            if(!_gameMatchService.IsGameRunning())
                return;
            
            _time += Time.deltaTime;
            _timeSinceLastUpdate += Time.deltaTime;

            if (_isPlayerLead)
            {
                _timePlayerLead += Time.deltaTime;
            }
            
            if(_timeSinceLastUpdate > 1)
            {
                _topWindowModel.CurrentTime = (int) _time;
                _topWindow.InvokeUpdateView(_topWindowModel);
                _playerDataService.UpdateTime(_timePlayerLead);

                _timeSinceLastUpdate = 0;
            }
        }
    }
}