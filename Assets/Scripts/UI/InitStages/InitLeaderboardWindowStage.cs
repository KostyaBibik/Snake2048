using Helpers;
using Services;
using UI.Leaderboard;
using UISystem;
using UniRx;
using Zenject;

namespace UI.InitStages
{
    public class InitLeaderboardWindowStage: IInitializable
    {
        private readonly GameMatchService _gameMatchService;

        public InitLeaderboardWindowStage(GameMatchService gameMatchService)
        {
            _gameMatchService = gameMatchService;
        }
        
        public void Initialize()
        {
            var leaderboard = UIManager.Instance.GetUIElement<LeaderboardWindow>();
            
            Observable.FromCoroutine(() => UiViewHelper.ActivateHandlerOnStartGame(leaderboard, _gameMatchService)).Subscribe();
            
            leaderboard.BeginHide();
        }
    }
}