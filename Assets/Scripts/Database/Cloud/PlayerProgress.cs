namespace Database.Cloud
{
    [System.Serializable]
    public class PlayerProgress
    {
        public int HighestTotalKills { get; set; }
        public int HighestTotalScore { get; set; }
        public int HighestLeaderTime { get; set; }
        public int CurrentTotalScore { get; set; }
        public int CurrentTotalKills { get; set; }
        public int CurrentLeaderTime { get; set; }
    }
}