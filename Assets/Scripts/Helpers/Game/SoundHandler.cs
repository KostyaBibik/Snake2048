using Enums;
using Services;
using Signals;
using UnityEngine;
using Zenject;

namespace Helpers
{
    public class SoundHandler : MonoBehaviour
    {
        [SerializeField] private AudioSource BgSource;
        [SerializeField] private AudioSource SoundEffectsSource;

        private SignalBus _signalBus;
        private GameSettingsService _gameSettingsService;

        [Inject]
        public void Construct(
            GameSettingsService gameSettingsService,
            SignalBus signalBus
        )
        {
            _signalBus = signalBus;
            _gameSettingsService = gameSettingsService;
        }

        private void Awake()
        {
            transform.parent = null;
            InitBaseVolumes();
            
            _signalBus.Subscribe<ChangeSoundSettingsSignal>(OnChangeSoundSettingsSignal);
        }

        private void InitBaseVolumes()
        {
            var gameSettingsData = _gameSettingsService.Data;
            SoundEffectsSource.volume = gameSettingsData.SoundVolume;
            BgSource.volume = gameSettingsData.MusicVolume;
        }

        public void PlayEffectClip(AudioClip audioClip, float volume = 1)
        {
            if(SoundEffectsSource == null)
                return;
            
            SoundEffectsSource.PlayOneShot(audioClip, volume);
        }

        private void OnChangeSoundSettingsSignal(ChangeSoundSettingsSignal signal)
        {
            switch (signal.source)
            {
                case EAudioType.SoundsFX:
                {
                    SoundEffectsSource.volume = signal.volume;
                }
                    break;
                case EAudioType.Music:
                {
                    BgSource.volume = signal.volume;
                }
                    break;
            }
        }

        private void OnDestroy()
        {
            _signalBus.Unsubscribe<ChangeSoundSettingsSignal>(OnChangeSoundSettingsSignal);
        }
    }
}