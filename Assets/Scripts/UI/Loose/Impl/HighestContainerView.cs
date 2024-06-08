using System.Globalization;

namespace UI.Loose.Impl
{
    public class HighestContainerView : StatsContainer
    {
        protected override void UpdateView(StatsContainerModel model)
        {
            totalScore.text = model.totalScore.ToString(CultureInfo.InvariantCulture);
            totalKills.text = model.totalKills.ToString(CultureInfo.InvariantCulture);
            leaderTime.text = FormatTime(model.leaderTime);
        }
    }
}