using System;
using Database;
using Helpers;
using Signals;
using Zenject;

namespace Systems.Action
{
    public class PlaySoundsSystem : IInitializable, IDisposable
    {
        private readonly SoundHandler _soundHandler;
        private readonly SignalBus _signalBus;
        private readonly SoundsConfig _soundsConfig;
        
        public PlaySoundsSystem(
            SoundHandler soundHandler,
            SoundsConfig soundsConfig,
            SignalBus signalBus
        )
        {
            _soundHandler = soundHandler;
            _soundsConfig = soundsConfig;
            _signalBus = signalBus;
        }
        
        public void Initialize()
        {
            _signalBus.Subscribe<PlaySoundSignal>(PlaySound);
        }

        private void PlaySound(PlaySoundSignal signal)
        {
            var clip = _soundsConfig.GetSound(signal.Type);
            _soundHandler.PlayClip(clip);
        }

        public void Dispose()
        {
            _signalBus.Unsubscribe<PlaySoundSignal>(PlaySound);
        }
    }
}