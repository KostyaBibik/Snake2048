using System;
using Zenject;

namespace Installers
{
    public class DiContainerInstaller : MonoInstaller, IDisposable
    {
        [Inject] private DiContainer _diContainer;

        public override void InstallBindings()
        {
            DiContainerRef.Container = _diContainer;
        }

        public void Dispose()
        {
            DiContainerRef.Container = null;
        }
    }
}