using Helpers;
using Zenject;

namespace Installers
{
    public class ProjectInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<SceneLoader>().AsSingle().NonLazy();
        }
    }
}