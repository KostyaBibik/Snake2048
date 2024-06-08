using System.Collections;
using System.Globalization;
using DG.Tweening;
using TMPro;
using UISystem;
using UnityEngine;

namespace UI.Loose
{
    public class StatsContainerModel
    {
        public int totalKills;
        public int totalScore;
        public int leaderTime;
        public bool showRecordTime;
        public bool showRecordScore;
        public bool showRecordKills;
    }
    
    public abstract class StatsContainer : UIElementView<StatsContainerModel>
    {
        [AutoSetupField] protected TextMeshProUGUI totalScore;
        [AutoSetupField] protected TextMeshProUGUI totalKills;
        [AutoSetupField] protected TextMeshProUGUI leaderTime;

        protected string FormatTime(int value)
        {
            var minutes = Mathf.FloorToInt(value / 60);
            var seconds = Mathf.FloorToInt(value % 60);
            return $"{minutes:00}:{seconds:00}";
        }
    }
}