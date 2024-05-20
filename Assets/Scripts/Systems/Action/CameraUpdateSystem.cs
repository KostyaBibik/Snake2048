using System;
using Cinemachine;
using Signals;
using Zenject;

namespace Systems.Action
{
    public class CameraUpdateSystem : IInitializable, IDisposable
    {
        private readonly CinemachineVirtualCamera _virtualCamera;
        private readonly SignalBus _signalBus;

        public CameraUpdateSystem(
            CinemachineVirtualCamera virtualCamera,
            SignalBus signalBus
        )
        {
            _virtualCamera = virtualCamera;
            _signalBus = signalBus;
        }

        public void Initialize()
        {
            _signalBus.Subscribe<CameraUpdateSignal>(OnUpdateCamera);
        }

        private void OnUpdateCamera(CameraUpdateSignal signal)
        {
            var followTarget = signal.followTarget;
            _virtualCamera.Follow = followTarget;
            _virtualCamera.LookAt = followTarget;
        }

        public void Dispose()
        {
            _signalBus.Unsubscribe<CameraUpdateSignal>(OnUpdateCamera);
        }
    }
}