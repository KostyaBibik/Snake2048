using System;
using UIKit.Elements;
using UIKit.Elements.Models;

namespace UI.Loose
{
    public struct LoseWindowModel
    {
        public Action restartCallback;
        public Action continueCallback;
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
        [AutoSetupField] private ButtonView _continue;
        [AutoSetupField] private StatsContainer _highestStatsContainer;
        [AutoSetupField] private StatsContainer _currentStatsContainer;

        protected override void UpdateView(LoseWindowModel model)
        {
            var restartModel = new ButtonModel();
            restartModel.ClickCallback = model.restartCallback;
            _restart.InvokeUpdateView(restartModel);
            
            var continueModel = new ButtonModel();
            continueModel.ClickCallback = model.continueCallback;
            _continue.InvokeUpdateView(continueModel);

            var highestContainerModel =
                CreateContainerModel(model.HighestLeaderTime, model.HighestTotalKills, model.HighestTotalScore);
            
            var currentContainerModel = CreateContainerModel(
                model.CurrentLeaderTime,
                model.CurrentTotalKills,
                model.CurrentTotalScore,
                showRecordKills: model.CurrentTotalKills > model.HighestTotalKills,
                showRecordScore: model.CurrentTotalScore > model.HighestTotalScore,
                showRecordTime: model.CurrentLeaderTime > model.HighestLeaderTime
                );
            
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
            bool showRecordTime = false,
            bool showRecordScore = false,
            bool showRecordKills = false
        )
        {
            var model = new StatsContainerModel();

            model.leaderTime = leaderTime;
            model.totalKills = totalKills;
            model.totalScore = totalScore;
            model.showRecordTime = showRecordTime;
            model.showRecordKills = showRecordKills;
            model.showRecordScore = showRecordScore;
            
            return model;
        }
    }
}