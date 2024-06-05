using TMPro;
using UISystem;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Leaderboard
{
    public class LeaderboardElementModel
    {
        public string nickname;
        public int score;
        public bool isPlayer;
    }
    
    public class LeaderboardElementView : UIElementView<LeaderboardElementModel>
    {
        [SerializeField] private Sprite playerBg;
        [SerializeField] private Sprite botBg;
        
        [AutoSetupField] private TextMeshProUGUI nickname;
        [AutoSetupField] private TextMeshProUGUI score;
        [AutoSetupField] private Image bg;
        
        protected override void UpdateView(LeaderboardElementModel model)
        {
            nickname.text = model.nickname;
            score.text = model.score.ToString();
            bg.sprite = model.isPlayer
                ? playerBg
                : botBg;
        }
    }
}