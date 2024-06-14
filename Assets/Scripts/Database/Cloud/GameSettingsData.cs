﻿namespace Database.Cloud
{
    [System.Serializable]
    public class GameSettingsData
    {
        public float MusicVolume { get; set; }
        public float SoundVolume { get; set; }
        public int CountPlays { get; set; }
        public string PlayerNickname { get; set; }

        public GameSettingsData()
        {
            MusicVolume = 1;
            SoundVolume = 1;
            PlayerNickname = "Player";
        }
    }
}