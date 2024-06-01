using Signals;
using UI.Leaderboard;
using UISystem;
using Zenject;

namespace Systems.Action
{
    public class LeaderboardHandleSystem : IInitializable
    {
        private LeaderboardWindow _leaderboardWindow;
        
        private readonly SignalBus _signalBus;

        public LeaderboardHandleSystem(SignalBus signalBus)
        {
            _signalBus = signalBus;
        }
        
        public void Initialize()
        {
            _leaderboardWindow = UIManager.Instance.GetUIElement<LeaderboardWindow>();
            _signalBus.Subscribe<LeaderboardUpdateSignal>(OnUpdateLeaderboard);
        }

        private void OnUpdateLeaderboard(LeaderboardUpdateSignal signal)
        {
            _leaderboardWindow.InvokeUpdateView(new LeaderboardModel(signal.elementModels));
        }
    }
}