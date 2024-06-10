using System.Collections.Generic;
using LocalizationSystem.Data.Extensions;

namespace LocalizationSystem.Data.KeyGeneration
{
	public static class LocalizationKeys
	{
		public static readonly Dictionary<LocalizationKey, string> Keys = new()
		{

			{LocalizationKey.None, LocalizationKey.None.ToString()},
			{LocalizationKey.Loading, LocalizationKey.Loading.ToString()},
			{LocalizationKey.EnterNickname, LocalizationKey.EnterNickname.ToString()},
			{LocalizationKey.TapToStart, LocalizationKey.TapToStart.ToString()},
			{LocalizationKey.Continue, LocalizationKey.Continue.ToString()},
			{LocalizationKey.Restart, LocalizationKey.Restart.ToString()},
			{LocalizationKey.Pause, LocalizationKey.Pause.ToString()},
			{LocalizationKey.SoundFx, LocalizationKey.SoundFx.ToString()},
			{LocalizationKey.Music, LocalizationKey.Music.ToString()},
			{LocalizationKey.GameOver, LocalizationKey.GameOver.ToString()},
			{LocalizationKey.PlayerStats, LocalizationKey.PlayerStats.ToString()},
			{LocalizationKey.BestLeaderTime, LocalizationKey.BestLeaderTime.ToString()},
			{LocalizationKey.BestTotalKills, LocalizationKey.BestTotalKills.ToString()},
			{LocalizationKey.BestTotalScore, LocalizationKey.BestTotalScore.ToString()},
			{LocalizationKey.CurrentleaderTime, LocalizationKey.CurrentleaderTime.ToString()},
			{LocalizationKey.CurrentTotalKills, LocalizationKey.CurrentTotalKills.ToString()},
			{LocalizationKey.CurrentTotalScore, LocalizationKey.CurrentTotalScore.ToString()},
			{LocalizationKey.NewRecord, LocalizationKey.NewRecord.ToString()},
		};
	}
}
