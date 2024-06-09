namespace Database.Cloud
{
    [System.Serializable]
    public class GameSettingsData
    {
        public float MusicVolume { get; set; }
        public float SoundVolume { get; set; }

        public GameSettingsData()
        {
            MusicVolume = 1;
            SoundVolume = 1;
        }
    }
}