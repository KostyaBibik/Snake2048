using TMPro;
using UISystem;

namespace UI.Leaderboard
{
    public class LeaderboardElementModel
    {
        public string nickname;
        public int score;
    }
    
    public class LeaderboardElementView : UIElementView<LeaderboardElementModel>
    {
        [AutoSetupField] private TextMeshProUGUI nickname;
        [AutoSetupField] private TextMeshProUGUI score;
        
        protected override void UpdateView(LeaderboardElementModel model)
        {
            nickname.text = model.nickname;
            score.text = model.score.ToString();
        }
    }
}