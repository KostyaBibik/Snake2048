using System;
using UIKit.Elements;
using UIKit.Elements.Models;

namespace UI.Loose
{
    public struct LoseWindowModel
    {
        public Action restartCallback;
        public int CurrentTotalKills;
        public int HighestTotalKills;
        public int CurrentLeaderTime;
        public int HighestLeaderTime;
        public int HighestTotalScore;
        public int CurrentTotalScore;
    }
    
    public class LoseWindow : UIWindow<LoseWindowModel>
    {
        [AutoSetupField] private ButtonView _restart;
        [AutoSetupField] private StatsContainerView _highestStatsContainer;
        [AutoSetupField] private StatsContainerView _currentStatsContainer;

        protected override void UpdateView(LoseWindowModel model)
        {
            var restartModel = new ButtonModel();
            restartModel.ClickCallback = model.restartCallback;
            _restart.InvokeUpdateView(restartModel);

            var highestContainerModel =
                CreateContainerModel(model.HighestLeaderTime, model.HighestTotalKills, model.HighestTotalScore, false);
            
            var currentContainerModel =
                CreateContainerModel(model.CurrentLeaderTime, model.CurrentTotalKills, model.CurrentTotalScore, true);
            
            _highestStatsContainer.InvokeUpdateView(highestContainerModel);
            _currentStatsContainer.InvokeUpdateView(currentContainerModel);
            _highestStatsContainer.BeginShow();
            _currentStatsContainer.BeginShow();
        }

        public override void Initialization()
        {
            _highestStatsContainer.gameObject.SetActive(false);
            _currentStatsContainer.gameObject.SetActive(false);
        }

        private StatsContainerModel CreateContainerModel(
            int leaderTime,
            int totalKills,
            int totalScore,
            bool isAnimating
        )
        {
            var model = new StatsContainerModel();

            model.leaderTime = leaderTime;
            model.totalKills = totalKills;
            model.totalScore = totalScore;
            model.isAnimating = isAnimating;
            
            return model;
        }
    }
}