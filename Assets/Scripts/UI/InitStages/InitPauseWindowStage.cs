using Enums;
using Helpers;
using Services;
using Signals;
using UI.Pause;
using UI.Top;
using UISystem;
using UniRx;
using Zenject;

namespace UI.InitStages
{
    public class InitPauseWindowStage: IInitializable
    {
        private readonly GameMatchService _gameMatchService;
        private readonly GameSettingsService _gameSettingsService;
        private readonly SignalBus _signalBus;
        
        public InitPauseWindowStage(
            GameMatchService gameMatchService,
            GameSettingsService gameSettingsService,
            SignalBus signalBus
        )
        {
            _gameMatchService = gameMatchService;
            _gameSettingsService = gameSettingsService;
            _signalBus = signalBus;
        }
        
        public void Initialize()
        {
            var pauseWindow = UIManager.Instance.GetUIElement<PauseWindow>();
            var topWindow = UIManager.Instance.GetUIElement<TopWindow>();
            
            var pauseWindowModel = new PauseWindowModel();
            pauseWindowModel.ContinuePlayCallback = () =>
            {
                _signalBus.Fire(new ChangeGameModeSignal {status = EGameModeStatus.Play});
                _signalBus.Fire(new PlaySoundSignal { type = ESoundType.UiClick});
                pauseWindow.BeginHide();
                topWindow.BeginShow();
            };

            var gameSettingsData = _gameSettingsService.Data;
            var savedMusicVolume = gameSettingsData.MusicVolume;
            var savedSoundVolume = gameSettingsData.SoundVolume;
            
            pauseWindowModel.baseMusicValue = savedMusicVolume;
            pauseWindowModel.baseSoundValue = savedSoundVolume;

            pauseWindowModel.onChangeSoundsCallback = OnChangeSoundVolume;
            pauseWindowModel.onChangeMusicCallback = OnChangeMusicVolume;
            
            Observable.FromCoroutine(() => UiViewHelper.ActivateHandlerOnStartGame(topWindow, _gameMatchService)).Subscribe();

            pauseWindow.InvokeUpdateView(pauseWindowModel);
            pauseWindow.BeginHide();
        }

        private void OnChangeSoundVolume(float volume)
        {
            _signalBus.Fire(new ChangeSoundSettingsSignal
            {
                source = EAudioType.SoundsFX,
                volume = volume
            });
            
            _gameSettingsService.UpdateSoundVolume(volume);
        }
        
        private void OnChangeMusicVolume(float volume)
        {
            _signalBus.Fire(new ChangeSoundSettingsSignal
            {
                source = EAudioType.Music,
                volume = volume
            });
            
            _gameSettingsService.UpdateMusicVolume(volume);
        }
    }
}