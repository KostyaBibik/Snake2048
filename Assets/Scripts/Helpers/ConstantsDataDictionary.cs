namespace Helpers
{
    public class ConstantsDataDictionary
    {
        public static class PlayerProgress
        {
            public const string SaveFileName  = "PlayerProgress.json";
            public const string HighestTotalScore = "HighestTotalScore";
            public const string HighestTotalKills = "HighestTotalKills";
            public const string HighestLeaderTime = "HighestLeaderTime";
            public const string CurrentTotalScore = "CurrentTotalScore";
            public const string CurrentTotalKills = "CurrentTotalKills";
            public const string CurrentLeaderTime = "CurrentLeaderTime";
        }
        
        public static class GameSettings
        {
            public const string SaveFileName = "GameSettingsData.json";
            public const string PlayerNickname = "PlayerNickname";
            public const string MusicVolume = "MusicVolume";
            public const string SoundVolume = "SoundVolume";
            public const string CountPlays = "CountPlays";
        }
    }
}